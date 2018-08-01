/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 * 
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 * 
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 * 
 *      You should have received a copy of the GNU General Public License
 */

using Orion.Crypto.Common;
using Orion.Crypto.Stream;
using Orion.Crypto.Stream.zlib;
using System;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace Orion.Crypto
{
    public class CryptoMan
    {
        public class BufferManipulation
        {
            public const uint 
                /*
                 * Standard crypto: Base64 Encoded + AES Encrypted buffers.
                 * 
                 * mov  [dwBufferFlag], 0
                 * mov  byte ptr [dwBufferFlag+3], 0xEE
                */
                AES         = 0xEE000000,
                /*
                 * AES buffers who have been additionally compressed with zlib.
                 * NOTE: The first bit is the level of compression used.
                 * 
                 * mov  [dwBufferFlag], 0
                 * mov  byte ptr [dwBufferFlag], 9
                 * mov  byte ptr [dwBufferFlag+3], 0xEE
                */
                AES_ZLIB    = AES | 9,
                /*
                 * Alternative crypto: XOR Encrypted buffers.
                 * 
                 * mov  [dwBufferFlag], 0
                 * mov  byte ptr [dwBufferFlag+3], 0xFF
                */
                XOR         = 0xFF000000,
                /*
                 * XOR buffers who have been additionally compressed with zlib.
                 * NOTE: The first bit is the level of compression used.
                 * 
                 * mov  [dwBufferFlag], 0
                 * mov  byte ptr [dwBufferFlag], 9
                 * mov  byte ptr [dwBufferFlag+3], 0xFF
                */
                XOR_ZLIB    = XOR | 9
            ;
        }

        public static byte[] DecryptFileString(PackStreamVerBase pStream, System.IO.Stream pBuffer)
        {
            if (pStream.GetCompressedHeaderSize() > 0 && pStream.GetEncodedHeaderSize() > 0 && pStream.GetHeaderSize() > 0)
            {
                byte[] pSrc = new byte[pStream.GetEncodedHeaderSize()];

                if ((ulong)pBuffer.Read(pSrc, 0, (int)pStream.GetEncodedHeaderSize()) == pStream.GetEncodedHeaderSize())
                {
                    return Decrypt(pStream.GetVer(), (uint)pStream.GetEncodedHeaderSize(), (uint)pStream.GetCompressedHeaderSize(), BufferManipulation.AES_ZLIB, pSrc);
                }
            }

            throw new Exception("ERROR decrypting file list: the size of the list is invalid.");
        }

        public static byte[] DecryptFileTable(PackStreamVerBase pStream, System.IO.Stream pBuffer)
        {
            if (pStream.GetCompressedDataSize() > 0 && pStream.GetEncodedDataSize() > 0 && pStream.GetDataSize() > 0)
            {
                byte[] pSrc = new byte[pStream.GetEncodedDataSize()];

                if ((ulong)pBuffer.Read(pSrc, 0, (int)pStream.GetEncodedDataSize()) == pStream.GetEncodedDataSize())
                {
                    return Decrypt(pStream.GetVer(), (uint)pStream.GetEncodedDataSize(), (uint)pStream.GetCompressedDataSize(), BufferManipulation.AES_ZLIB, pSrc);
                }
            }

            throw new Exception("ERROR decrypting file table: the size of the table is invalid.");
        }

        public static byte[] DecryptData(PackFileHeaderVerBase pHeader, MemoryMappedFile pData)
        {

            if (pHeader.GetCompressedFileSize() > 0 && pHeader.GetEncodedFileSize() > 0 && pHeader.GetFileSize() > 0)
            {
                using (MemoryMappedViewStream pBuffer = pData.CreateViewStream((long)pHeader.GetOffset(), pHeader.GetEncodedFileSize()))
                {
                    byte[] pSrc = new byte[pHeader.GetEncodedFileSize()];

                    if (pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) == pHeader.GetEncodedFileSize())
                    {
                        return Decrypt(pHeader.GetVer(), pHeader.GetEncodedFileSize(), (uint)pHeader.GetCompressedFileSize(), pHeader.GetBufferFlag(), pSrc);
                    }
                }
            }

            throw new Exception("ERROR decrypting data file segment: the size of the block is invalid.");
        }

        // Decryption Routine: Base64 -> AES -> Zlib
        private static byte[] Decrypt(uint uVer, uint uLen, uint uLenCompressed, uint dwBufferFlag, byte[] pSrc)
        {
            byte[] bits = BitConverter.GetBytes(dwBufferFlag);

            if (!((bits[3] & 1) != 0))
            {
                byte[] aKey;
                byte[] aIV;
                // Get the AES Key/IV for transformation
                CipherKeys.GetKeyAndIV(uVer, uLenCompressed, out aKey, out aIV);

                // Decode the base64 encoded string
                pSrc = Convert.FromBase64String(Encoding.UTF8.GetString(pSrc));

                // Decrypt the AES encrypted block
                AESCipher pCipher = new AESCipher(aKey, aIV);
                pCipher.TransformBlock(pSrc, 0, uLen, pSrc, 0);
            } else
            {
                // Decrypt the XOR encrypted block
                pSrc = EncryptXOR(uVer, pSrc, uLen, uLenCompressed);
            }

            if (bits[0] != 0)
            {
                return ZlibStream.UncompressBuffer(pSrc);
            }

            return pSrc;
        }

        // Encryption Routine: Zlib -> AES -> Base64
        public static byte[] Encrypt(uint uVer, byte[] pSrc, uint dwBufferFlag, out uint uLen, out uint uLenCompressed, out uint uLenEncoded)
        {
            byte[] bits = BitConverter.GetBytes(dwBufferFlag);

            byte[] pEncrypted;
            if (bits[0] != 0)
            {
                pEncrypted = ZlibStream.CompressBuffer(pSrc);
            } else
            {
                pEncrypted = new byte[pSrc.Length];
                Buffer.BlockCopy(pSrc, 0, pEncrypted, 0, pSrc.Length);
            }

            uLen = (uint) pSrc.Length;
            uLenCompressed = (uint) pEncrypted.Length;

            if (!((bits[3] & 1) != 0))
            {
                byte[] aKey;
                byte[] aIV;
                // Get the AES Key/IV for transformation
                CipherKeys.GetKeyAndIV(uVer, uLenCompressed, out aKey, out aIV);

                // Perform AES block encryption
                AESCipher pCipher = new AESCipher(aKey, aIV);
                pCipher.TransformBlock(pEncrypted, 0, uLen, pEncrypted, 0);

                // Encode the encrypted data into a base64 encoded string
                pEncrypted = Encoding.UTF8.GetBytes(Convert.ToBase64String(pEncrypted));
            } else
            {
                // Perform XOR block encryption
                pEncrypted = EncryptXOR(uVer, pEncrypted, uLen, uLenCompressed);
            }
            
            uLenEncoded = (uint) pEncrypted.Length;

            return pEncrypted;
        }

        private static byte[] EncryptXOR(uint uVer, byte[] pSrc, uint uLen, uint uLenCompressed)
        {
            byte[] aKey;

            CipherKeys.GetXORKey(uVer, out aKey);

            uint uBlock = uLen >> 2;
            uint uBlockOffset = 0;
            int nKeyOffset = 0;

            if (uBlock != 0)
            {
                while (uBlockOffset < uBlock)
                {
                    /*
                     *  _begin:
                     *      mov     eax, [ebp+pKey]
                     *      mov     eax, [eax+edx*4]
                     *      xor     [ebx+ecx*4], eax
                     *      inc     edx
                     *      inc     ecx
                     *      and     edx, 1FFh
                     *      cmp     ecx, esi
                     *      jb _begin
                     * _end:
                     *      mov     eax, [ebp+uLen]
                    */

                    uint pBlockData = BitConverter.ToUInt32(pSrc, (int)(4 * uBlockOffset)) ^ BitConverter.ToUInt32(aKey, 4 * nKeyOffset);
                    Buffer.BlockCopy(BitConverter.GetBytes(pBlockData), 0, pSrc, (int)(4 * uBlockOffset), sizeof(uint));

                    nKeyOffset = ((ushort)nKeyOffset + 1) & 0x1FF;
                    uBlockOffset++;
                }
            }

            uBlock = (uLen & 3);
            if (uBlock != 0)
            {
                int nStart = (int)(4 * uBlockOffset);

                uBlockOffset = 0;
                nKeyOffset = 0;

                while (uBlockOffset < uBlock)
                {
                    pSrc[nStart + uBlockOffset++] ^= (byte)(aKey[nKeyOffset]);

                    nKeyOffset = ((ushort)nKeyOffset + 1) & 0x7FF;
                }
            }

            return pSrc;
        }
    }
}

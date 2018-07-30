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

        public static byte[] DecryptFileList(PackStreamVerBase pStream, System.IO.Stream pBuffer)
        {
            if (pStream.GetCompressedHeaderSize() > 0 && pStream.GetEncodedHeaderSize() > 0 && pStream.GetHeaderSize() > 0)
            {
                byte[] pSrc = new byte[pStream.GetEncodedHeaderSize()];

                if ((ulong)pBuffer.Read(pSrc, 0, (int)pStream.GetEncodedHeaderSize()) == pStream.GetEncodedHeaderSize())
                {
                    bool bCompressed = pStream.GetEncodedHeaderSize() != pStream.GetCompressedHeaderSize();

                    return Decrypt(pStream.GetVer(), (uint)pStream.GetEncodedHeaderSize(), (uint)pStream.GetCompressedHeaderSize(), bCompressed, pSrc);
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
                    bool bCompressed = pStream.GetEncodedDataSize() != pStream.GetCompressedDataSize();

                    return Decrypt(pStream.GetVer(), (uint)pStream.GetEncodedDataSize(), (uint)pStream.GetCompressedDataSize(), bCompressed, pSrc);
                }
            }

            throw new Exception("ERROR decrypting file table: the size of the table is invalid.");
        }

        public static byte[] DecryptData(PackFileHeaderVerBase pHeader, MemoryMappedFile pData)
        {

            if (pHeader.GetCompressedFileSize() > 0 && pHeader.GetEncodedFileSize() > 0 && pHeader.GetFileSize() > 0)
            {
                using (MemoryMappedViewStream pBuffer = pData.CreateViewStream((long)pHeader.GetOffset(), (long)pHeader.GetEncodedFileSize()))
                {
                    byte[] pSrc = new byte[pHeader.GetEncodedFileSize()];

                    if ((ulong)pBuffer.Read(pSrc, 0, (int)pHeader.GetEncodedFileSize()) == pHeader.GetEncodedFileSize())
                    {
                        return Decrypt(pHeader.GetVer(), (uint)pHeader.GetEncodedFileSize(), (uint)pHeader.GetCompressedFileSize(), pHeader.GetCompressionFlag() == 0xEE000009, pSrc);
                    }
                }
            }

            throw new Exception("ERROR decrypting data file segment: the size of the block is invalid.");
        }

        // Decryption Routine: Base64 -> AES -> Zlib
        private static byte[] Decrypt(uint uVer, uint uLen, uint uLenCompressed, bool bCompressed, byte[] pSrc)
        {
            byte[] aKey;
            byte[] aIV;

            CipherKeys.GetKeyAndIV(uVer, uLenCompressed, out aKey, out aIV);

            AESCipher pCipher = new AESCipher(aKey, aIV);

            byte[] pDecrypted = Convert.FromBase64String(Encoding.UTF8.GetString(pSrc));
            pCipher.TransformBlock(pDecrypted, 0, uLen, pDecrypted, 0);

            if (bCompressed)
            {
                return ZlibStream.UncompressBuffer(pDecrypted);
            }
            else
            {
                return pDecrypted;
            }
        }

        // Encryption Routine: Zlib -> AES -> Base64
        public static byte[] Encrypt(uint uVer, byte[] pSrc, bool bCompressed, out uint uLen, out uint uLenCompressed, out uint uLenEncoded)
        {
            byte[] aKey;
            byte[] aIV;

            uLen = (uint) pSrc.Length;

            byte[] pEncrypted;
            if (bCompressed)
            {
                pEncrypted = ZlibStream.CompressBuffer(pSrc);
            } else
            {
                pEncrypted = new byte[uLen];
                Buffer.BlockCopy(pSrc, 0, pEncrypted, 0, (int)uLen);
            }
            uLenCompressed = (uint) pEncrypted.Length;

            CipherKeys.GetKeyAndIV(uVer, uLenCompressed, out aKey, out aIV);
            AESCipher pCipher = new AESCipher(aKey, aIV);
            pCipher.TransformBlock(pEncrypted, 0, uLen, pEncrypted, 0);

            pEncrypted = Encoding.UTF8.GetBytes(Convert.ToBase64String(pEncrypted));
            uLenEncoded = (uint) pEncrypted.Length;

            return pEncrypted;
        }
    }
}

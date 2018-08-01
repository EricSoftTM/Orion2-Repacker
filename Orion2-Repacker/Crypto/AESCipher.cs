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

using System.Security.Cryptography;

namespace Orion.Crypto
{
    public class AESCipher
    {
        private readonly byte[] aCounter;
        private readonly SymmetricAlgorithm pAlgorithm;
        private readonly ICryptoTransform pCounterEncryptor;

        public int BlockSize { get { return pAlgorithm.BlockSize / 8; } }

        /*
         * Constructs a new AES-CTR cipher.
         * 
         * @param aUserKey A 32-byte User Key
         * @param aIV A 16-byte IV Chain
         * 
        */
        public AESCipher(byte[] aUserKey, byte[] aIV)
        {
            this.aCounter = aIV;

            this.pAlgorithm = new AesManaged
            {
                Mode = CipherMode.ECB,
                Padding = PaddingMode.None
            };

            this.pCounterEncryptor = pAlgorithm.CreateEncryptor(aUserKey, new byte[BlockSize]);
        }

        /*
         * Transforms a block, encrypting/decrypting the specified data.
         * 
         * @param pSrc The raw buffer (of either encrypted or decrypted data)
         * @param uOffset The initial offset of the source buffer
         * @param uLen The length of the block (in bytes) to be transformed
         * @param pDest The destination buffer (of now-decrypted or now-encrypted data)
         * @param Dst The initial offset of the destination buffer
         * 
         * @return The length of the block that was transformed
         * 
        */
        public uint TransformBlock(byte[] pSrc, int uOffset, uint uLen, byte[] pDest, int Dst)
        {
            for (int i = 0; i < uLen; i += BlockSize)
            {
                byte[] pXORBlock = new byte[BlockSize];
                pCounterEncryptor.TransformBlock(aCounter, 0, aCounter.Length, pXORBlock, 0);
                IncrementCounter();

                for (int j = 0; j < pXORBlock.Length; j++)
                {
                    if ((i + j) >= pDest.Length)
                    {
                        break;
                    }
                    pDest[Dst + i + j] = (byte)(pSrc[uOffset + i + j] ^ pXORBlock[j]);
                }
            }

            return uLen;
        }

        /*
         * Increments the XOR block counter.
         * 
        */
        private void IncrementCounter()
        {
            for (int i = aCounter.Length - 1; i >= 0; i--)
            {
                if (++aCounter[i] != 0)
                    break;
            }
        }
    }
}

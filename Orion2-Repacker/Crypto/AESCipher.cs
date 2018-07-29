using System.Security.Cryptography;

namespace Orion.Crypto
{
    public class AESCipher
    {
        private readonly byte[] aCounter;
        private readonly SymmetricAlgorithm pAlgorithm;
        private readonly ICryptoTransform pCounterEncryptor;

        public int BlockSize { get { return pAlgorithm.BlockSize / 8; } }

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

        public uint TransformBlock(byte[] pSrc, int uOffset, uint uLen, byte[] pDest, int Dst)
        {

            for (int i = 0; i < uLen; i += BlockSize)
            {
                byte[] xorBlock = new byte[BlockSize];
                pCounterEncryptor.TransformBlock(aCounter, 0, aCounter.Length, xorBlock, 0);
                IncrementCounter();

                for (int j = 0; j < xorBlock.Length; j++)
                {
                    if ((i + j) >= pDest.Length)
                    {
                        break;
                    }
                    pDest[i + j] = (byte)(pSrc[i + j] ^ xorBlock[j]);
                }
            }

            return uLen;
        }

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

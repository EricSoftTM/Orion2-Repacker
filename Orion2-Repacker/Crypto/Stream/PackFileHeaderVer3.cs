using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackFileHeaderVer3 : PackFileHeaderVerBase
    {
        private readonly uint uVer;
        private uint dwCompressionFlag;
        private int dwFileIndex;
        private uint dwEncodedFileSize;
        private int dwReserved;
        private ulong dwCompressedFileSize;
        private ulong dwFileSize;
        private ulong dwOffset;

        private PackFileHeaderVer3(uint uVer)
        {
            this.uVer = uVer;
        }

        public static PackFileHeaderVer3 CreateHeader(uint uVer, int nIndex, uint dwCompression, ulong uOffset, byte[] pData)
        {
            uint uLen, uCompressedLen, uEncodedLen;

            CryptoMan.Encrypt(uVer, pData, dwCompression == 0xEE000009, out uLen, out uCompressedLen, out uEncodedLen);

            return new PackFileHeaderVer3(uVer)
            {
                dwCompressionFlag = dwCompression,
                dwFileIndex = nIndex,
                dwEncodedFileSize = uEncodedLen,
                dwReserved = 0,
                dwCompressedFileSize = uCompressedLen,
                dwFileSize = uLen,
                dwOffset = uOffset
            };
        }

        public static PackFileHeaderVer3 ParseHeader(BinaryReader pReader, uint uVer)
        {
            return new PackFileHeaderVer3(uVer)
            {
                dwCompressionFlag = pReader.ReadUInt32(),
                dwFileIndex = pReader.ReadInt32(),
                dwEncodedFileSize = pReader.ReadUInt32(),
                dwReserved = pReader.ReadInt32(),
                dwCompressedFileSize = pReader.ReadUInt64(),
                dwFileSize = pReader.ReadUInt64(),
                dwOffset = pReader.ReadUInt64()
            };
        }

        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(this.dwCompressionFlag);
            pWriter.Write(this.dwFileIndex);
            pWriter.Write(this.dwEncodedFileSize);
            pWriter.Write(this.dwReserved);
            pWriter.Write(this.dwCompressedFileSize);
            pWriter.Write(this.dwFileSize);
            pWriter.Write(this.dwOffset);
        }

        public uint GetVer()
        {
            return uVer;
        }

        public int GetFileIndex()
        {
            return dwFileIndex;
        }

        public uint GetCompressionFlag()
        {
            return dwCompressionFlag;
        }

        public ulong GetOffset()
        {
            return dwOffset;
        }

        public ulong GetEncodedFileSize()
        {
            return dwEncodedFileSize;
        }

        public ulong GetCompressedFileSize()
        {
            return dwCompressedFileSize;
        }

        public ulong GetFileSize()
        {
            return dwFileSize;
        }

        public void SetFileIndex(int nIndex)
        {
            this.dwFileIndex = nIndex;
        }

        public void SetOffset(ulong dwOffset)
        {
            this.dwOffset = dwOffset;
        }

        public void SetEncodedFileSize(ulong uEncoded)
        {
            this.dwEncodedFileSize = (uint) uEncoded;
        }

        public void SetCompressedFileSize(ulong uCompressed)
        {
            this.dwCompressedFileSize = uCompressed;
        }

        public void SetFileSize(ulong uSize)
        {
            this.dwFileSize = uSize;
        }
    }
}

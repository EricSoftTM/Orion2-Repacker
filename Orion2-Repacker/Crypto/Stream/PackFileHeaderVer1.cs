using Orion.Crypto.Common;
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackFileHeaderVer1 : PackFileHeaderVerBase
    {
        private int dwReserved;
        private int dwFileIndex;
        private uint dwCompressionFlag;
        private int dwReserved_;
        private ulong dwOffset;
        private ulong dwEncodedFileSize;
        private ulong dwCompressedFileSize;
        private ulong dwFileSize;

        private PackFileHeaderVer1()
        {

        }

        public static PackFileHeaderVer1 CreateHeader(int nIndex, uint dwCompression, ulong uOffset, byte[] pData)
        {
            uint uLen, uCompressedLen, uEncodedLen;

            CryptoMan.Encrypt(PackVer.MS2F, pData, dwCompression == 0xEE000009, out uLen, out uCompressedLen, out uEncodedLen);

            return new PackFileHeaderVer1
            {
                dwReserved = 0,
                dwFileIndex = nIndex,
                dwCompressionFlag = dwCompression,
                dwReserved_ = 0,
                dwOffset = uOffset,
                dwEncodedFileSize = uEncodedLen,
                dwCompressedFileSize = uCompressedLen,
                dwFileSize = uLen
            };
        }

        public static PackFileHeaderVer1 ParseHeader(BinaryReader pReader)
        {
            return new PackFileHeaderVer1
            {
                dwReserved = pReader.ReadInt32(),
                dwFileIndex = pReader.ReadInt32(),
                dwCompressionFlag = pReader.ReadUInt32(),
                dwReserved_ = pReader.ReadInt32(),
                dwOffset = pReader.ReadUInt64(),
                dwEncodedFileSize = pReader.ReadUInt64(),
                dwCompressedFileSize = pReader.ReadUInt64(),
                dwFileSize = pReader.ReadUInt64()
            };
        }

        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(this.dwReserved);
            pWriter.Write(this.dwFileIndex);
            pWriter.Write(this.dwCompressionFlag);
            pWriter.Write(this.dwReserved_);
            pWriter.Write(this.dwOffset);
            pWriter.Write(this.dwEncodedFileSize);
            pWriter.Write(this.dwCompressedFileSize);
            pWriter.Write(this.dwFileSize);
        }

        public uint GetVer()
        {
            return PackVer.MS2F;
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
            this.dwEncodedFileSize = uEncoded;
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

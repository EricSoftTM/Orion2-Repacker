using Orion.Crypto.Common;
using System.Collections.Generic;
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackStreamVer1 : PackStreamVerBase
    {
        private uint uReserved;
        private ulong dwCompressedDataSize;
        private ulong dwEncodedDataSize;
        private ulong dwHeaderSize;
        private ulong dwCompressedHeaderSize;
        private ulong dwEncodedHeaderSize;
        private ulong dwFileListCount;
        private ulong dwDataSize;
        private readonly List<PackFileEntry> aFileList;

        private PackStreamVer1()
        {
            this.aFileList = new List<PackFileEntry>();
        }

        public static PackStreamVer1 ParseHeader(BinaryReader pReader)
        {
            return new PackStreamVer1
            {
                uReserved = pReader.ReadUInt32(),
                dwCompressedDataSize = pReader.ReadUInt64(),
                dwEncodedDataSize = pReader.ReadUInt64(),
                dwHeaderSize = pReader.ReadUInt64(),
                dwCompressedHeaderSize = pReader.ReadUInt64(),
                dwEncodedHeaderSize = pReader.ReadUInt64(),
                dwFileListCount = pReader.ReadUInt64(),
                dwDataSize = pReader.ReadUInt64()
            };
        }
        
        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(this.uReserved);
            pWriter.Write(this.dwCompressedDataSize);
            pWriter.Write(this.dwEncodedDataSize);
            pWriter.Write(this.dwHeaderSize);
            pWriter.Write(this.dwCompressedHeaderSize);
            pWriter.Write(this.dwEncodedHeaderSize);
            pWriter.Write(this.dwFileListCount);
            pWriter.Write(this.dwDataSize);
        }

        public uint GetVer()
        {
            return PackVer.MS2F;
        }

        public ulong GetCompressedHeaderSize()
        {
            return dwCompressedHeaderSize;
        }

        public ulong GetEncodedHeaderSize()
        {
            return dwEncodedHeaderSize;
        }

        public ulong GetHeaderSize()
        {
            return dwHeaderSize;
        }

        public ulong GetCompressedDataSize()
        {
            return dwCompressedDataSize;
        }

        public ulong GetEncodedDataSize()
        {
            return dwEncodedDataSize;
        }

        public ulong GetDataSize()
        {
            return dwDataSize;
        }

        public ulong GetFileListCount()
        {
            return dwFileListCount;
        }

        public List<PackFileEntry> GetFileList()
        {
            return aFileList;
        }

        public void SetCompressedHeaderSize(ulong uCompressed)
        {
            this.dwCompressedHeaderSize = uCompressed;
        }

        public void SetEncodedHeaderSize(ulong uEncoded)
        {
            this.dwEncodedHeaderSize = uEncoded;
        }

        public void SetHeaderSize(ulong uSize)
        {
            this.dwHeaderSize = uSize;
        }

        public void SetCompressedDataSize(ulong uCompressed)
        {
            this.dwCompressedDataSize = uCompressed;
        }

        public void SetEncodedDataSize(ulong uEncoded)
        {
            this.dwEncodedDataSize = uEncoded;
        }

        public void SetDataSize(ulong uSize)
        {
            this.dwDataSize = uSize;
        }

        public void SetFileListCount(ulong uCount)
        {
            this.dwFileListCount = uCount;
        }
    }
}

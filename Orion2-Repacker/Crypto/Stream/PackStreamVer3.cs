using Orion.Crypto.Common;
using System.Collections.Generic;
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackStreamVer3 : PackStreamVerBase
    {
        private readonly uint uVer;
        private uint dwFileListCount;
        private uint dwReserved;
        private ulong dwCompressedDataSize;
        private ulong dwEncodedDataSize;
        private ulong dwHeaderSize;
        private ulong dwCompressedHeaderSize;
        private ulong dwEncodedHeaderSize;
        private ulong dwDataSize;
        private readonly List<PackFileEntry> aFileList;

        private PackStreamVer3(uint uVer)
        {
            this.uVer = uVer;
            this.aFileList = new List<PackFileEntry>();
        }

        public static PackStreamVer3 ParseHeader(BinaryReader pReader, uint uVer)
        {
            return new PackStreamVer3(uVer)
            {
                dwFileListCount = pReader.ReadUInt32(),
                dwReserved = pReader.ReadUInt32(),
                dwCompressedDataSize = pReader.ReadUInt64(),
                dwEncodedDataSize = pReader.ReadUInt64(),
                dwCompressedHeaderSize = pReader.ReadUInt64(),
                dwEncodedHeaderSize = pReader.ReadUInt64(),
                dwDataSize = pReader.ReadUInt64(),
                dwHeaderSize = pReader.ReadUInt64()
            };
        }

        public uint GetVer()
        {
            return uVer;//OS2F/PS2F
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
    }
}

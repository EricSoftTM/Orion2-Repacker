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
    }
}

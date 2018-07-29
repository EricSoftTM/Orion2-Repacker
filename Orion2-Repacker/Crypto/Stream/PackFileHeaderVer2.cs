using Orion.Crypto.Common;
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackFileHeaderVer2 : PackFileHeaderVerBase
    {
        private uint dwCompressionFlag;
        private int dwFileIndex;
        private uint dwEncodedFileSize;
        private ulong dwCompressedFileSize;
        private ulong dwFileSize;
        private ulong dwOffset;

        private PackFileHeaderVer2()
        {

        }

        public static PackFileHeaderVer2 ParseHeader(BinaryReader pReader)
        {
            return new PackFileHeaderVer2
            {
                dwCompressionFlag = pReader.ReadUInt32(),
                dwFileIndex = pReader.ReadInt32(),
                dwEncodedFileSize = pReader.ReadUInt32(),
                dwCompressedFileSize = pReader.ReadUInt64(),
                dwFileSize = pReader.ReadUInt64(),
                dwOffset = pReader.ReadUInt64()
            };
        }

        public uint GetVer()
        {
            return PackVer.NS2F;
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

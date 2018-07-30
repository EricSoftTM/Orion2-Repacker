using System.IO;

namespace Orion.Crypto.Stream
{
    public interface PackFileHeaderVerBase
    {
        uint GetVer();

        int GetFileIndex();

        uint GetCompressionFlag();

        ulong GetOffset();

        ulong GetEncodedFileSize();
        ulong GetCompressedFileSize();
        ulong GetFileSize();

        //bool IsCompressed();

        void Encode(BinaryWriter pWriter);

        void SetFileIndex(int nIndex);

        void SetOffset(ulong uOffset);

        void SetEncodedFileSize(ulong uEncoded);
        void SetCompressedFileSize(ulong uCompressed);
        void SetFileSize(ulong uSize);
    }
}

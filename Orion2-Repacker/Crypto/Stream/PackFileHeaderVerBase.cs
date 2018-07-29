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
    }
}

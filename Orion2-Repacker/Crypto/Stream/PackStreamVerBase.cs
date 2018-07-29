using Orion.Crypto.Common;
using System.Collections.Generic;

namespace Orion.Crypto.Stream
{
    public interface PackStreamVerBase
    {

        uint GetVer(); // Represents the format of the packed stream (MS2F/NS2F/etc)

        ulong GetCompressedHeaderSize(); // The total compressed size of the (raw) file list
        ulong GetEncodedHeaderSize(); // The total (base64) encoded size of the file list
        ulong GetHeaderSize(); // The total size of the raw (decoded, decompressed) file list

        ulong GetCompressedDataSize(); // The total compressed size of the (raw) file table
        ulong GetEncodedDataSize(); // The total (base64) encoded size of the file table
        ulong GetDataSize(); // The total size of the raw (decoded, decompressed) file table

        ulong GetFileListCount(); // The total count of files within the data file

        List<PackFileEntry> GetFileList(); // Represents a list of fileinfo containers (<Index>,<Hash>,<Name>)
    }
}

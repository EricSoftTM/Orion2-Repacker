/*
 *      This file is part of Orion2, a MapleStory2 Packaging Library Project.
 *      Copyright (C) 2018 Eric Smith <notericsoft@gmail.com>
 * 
 *      This program is free software: you can redistribute it and/or modify
 *      it under the terms of the GNU General Public License as published by
 *      the Free Software Foundation, either version 3 of the License, or
 *      (at your option) any later version.
 * 
 *      This program is distributed in the hope that it will be useful,
 *      but WITHOUT ANY WARRANTY; without even the implied warranty of
 *      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *      GNU General Public License for more details.
 * 
 *      You should have received a copy of the GNU General Public License
 */

using Orion.Crypto.Common;
using System.Collections.Generic;
using System.IO;

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

        void Encode(BinaryWriter pWriter); // Encodes the header/data pack sizes to stream

        void SetCompressedHeaderSize(ulong uCompressed); // Updates the compressed file string size
        void SetEncodedHeaderSize(ulong uEncoded); // Updates the base64 encoded file string length
        void SetHeaderSize(ulong uSize); // Updates the raw (uncompressed) file string size

        void SetCompressedDataSize(ulong uCompressed); // Updates the compressed file table size
        void SetEncodedDataSize(ulong uEncoded); // Updates the base64 encoded file table length
        void SetDataSize(ulong uSize); // Updates the raw (uncompressed) file table size

        void SetFileListCount(ulong uCount); // Updates the total count of files within this stream
    }
}

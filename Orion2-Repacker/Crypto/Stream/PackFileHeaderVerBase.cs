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

using System.IO;

namespace Orion.Crypto.Stream
{
    public interface PackFileHeaderVerBase
    {
        uint GetVer(); // Represents the format of the packed stream (MS2F/NS2F/etc)

        int GetFileIndex(); // The index of this file located within the lookup table

        uint GetBufferFlag(); // The flag that determines buffer manipulation

        ulong GetOffset(); // The start offset of this file's data within the m2d file

        uint GetEncodedFileSize(); // The total (base64) encoded size of the file
        ulong GetCompressedFileSize(); // The total compressed size of the (raw) file
        ulong GetFileSize(); // The total size of the raw (decoded, decompressed) file

        void Encode(BinaryWriter pWriter); // Encodes the contents of this file to stream

        void SetFileIndex(int nIndex); // Updates this file's index within the lookup table

        void SetOffset(ulong uOffset); // Updates this file's initial offset within the m2d file

        void SetEncodedFileSize(uint uEncoded); // Updates this file's encoded base64 length
        void SetCompressedFileSize(ulong uCompressed); // Updates this file's compressed file size
        void SetFileSize(ulong uSize); // Updates this file's raw (uncompressed) file size
    }
}

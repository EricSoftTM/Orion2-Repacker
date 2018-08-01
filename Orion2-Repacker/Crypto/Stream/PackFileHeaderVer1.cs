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
using System.IO;

namespace Orion.Crypto.Stream
{
    public class PackFileHeaderVer1 : PackFileHeaderVerBase
    {
        private byte[] aPackingDef; //A "Packing Definition", unused.
        private int nFileIndex;
        private uint dwBufferFlag;
        private int[] Reserved;
        private ulong uOffset;
        private uint uEncodedFileSize;
        private ulong uCompressedFileSize;
        private ulong uFileSize;

        private PackFileHeaderVer1()
        {
            this.aPackingDef = new byte[4];
            this.Reserved = new int[2];
        }

        public PackFileHeaderVer1(BinaryReader pReader)
            : this()
        {
            this.aPackingDef = pReader.ReadBytes(4);         //[ecx+16]
            this.nFileIndex = pReader.ReadInt32();           //[ecx+20]
            this.dwBufferFlag = pReader.ReadUInt32();        //[ecx+24]
            this.Reserved[0] = pReader.ReadInt32();          //[ecx+28]
            this.uOffset = pReader.ReadUInt64();             //[ecx+32] | [ecx+36]
            this.uEncodedFileSize = pReader.ReadUInt32();    //[ecx+40]
            this.Reserved[1] = pReader.ReadInt32();          //[ecx+44]
            this.uCompressedFileSize = pReader.ReadUInt64(); //[ecx+48] | [ecx+52]
            this.uFileSize = pReader.ReadUInt64();           //[ecx+56] | [ecx+60]
        }

        public static PackFileHeaderVer1 CreateHeader(int nIndex, uint dwFlag, ulong uOffset, byte[] pData)
        {
            uint uLen, uCompressedLen, uEncodedLen;

            CryptoMan.Encrypt(PackVer.MS2F, pData, dwFlag, out uLen, out uCompressedLen, out uEncodedLen);

            return new PackFileHeaderVer1
            {
                nFileIndex = nIndex,
                dwBufferFlag = dwFlag,
                uOffset = uOffset,
                uEncodedFileSize = uEncodedLen,
                uCompressedFileSize = uCompressedLen,
                uFileSize = uLen
            };
        }

        public void Encode(BinaryWriter pWriter)
        {
            pWriter.Write(this.aPackingDef);
            pWriter.Write(this.nFileIndex);
            pWriter.Write(this.dwBufferFlag);
            pWriter.Write(this.Reserved[0]);
            pWriter.Write(this.uOffset);
            pWriter.Write(this.uEncodedFileSize);
            pWriter.Write(this.Reserved[1]);
            pWriter.Write(this.uCompressedFileSize);
            pWriter.Write(this.uFileSize);
        }

        public uint GetVer()
        {
            return PackVer.MS2F;
        }

        public int GetFileIndex()
        {
            return nFileIndex;
        }

        public uint GetBufferFlag()
        {
            return dwBufferFlag;
        }

        public ulong GetOffset()
        {
            return uOffset;
        }

        public uint GetEncodedFileSize()
        {
            return uEncodedFileSize;
        }

        public ulong GetCompressedFileSize()
        {
            return uCompressedFileSize;
        }

        public ulong GetFileSize()
        {
            return uFileSize;
        }

        public void SetFileIndex(int nIndex)
        {
            this.nFileIndex = nIndex;
        }

        public void SetOffset(ulong uOffset)
        {
            this.uOffset = uOffset;
        }

        public void SetEncodedFileSize(uint uEncoded)
        {
            this.uEncodedFileSize = uEncoded;
        }

        public void SetCompressedFileSize(ulong uCompressed)
        {
            this.uCompressedFileSize = uCompressed;
        }

        public void SetFileSize(ulong uSize)
        {
            this.uFileSize = uSize;
        }
    }
}

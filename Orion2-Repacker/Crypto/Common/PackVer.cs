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

using Orion.Crypto.Stream;
using System;
using System.IO;

namespace Orion.Crypto.Common
{
    public class PackVer
    {
        public const uint
            MS2F = 0x4632534D,  //Ver1
            NS2F = 0x4632534E,  //Ver2
            OS2F = 0x4632534F,  //Ver3
            PS2F = 0x46325350   //Ver3
        ;

        /*
         * Creates a new packed stream based on the type of version.
         * 
         * @param pHeader The stream to read the pack version from
         * 
         * @return A packed stream with header sizes decoded
         * 
        */
        public static PackStreamVerBase CreatePackVer(BinaryReader pHeader)
        {
            uint uVer = pHeader.ReadUInt32();
            switch (uVer)
            {
                case PackVer.MS2F:
                    return PackStreamVer1.ParseHeader(pHeader);
                case PackVer.NS2F:
                    return PackStreamVer2.ParseHeader(pHeader);
                case PackVer.OS2F:
                case PackVer.PS2F:
                    return PackStreamVer3.ParseHeader(pHeader, uVer);
            }

            throw new Exception(string.Format("Unknown file version read from stream ({0})", uVer));
        }
    }
}

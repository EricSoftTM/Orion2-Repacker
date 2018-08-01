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
using System.Collections.Generic;

namespace Orion.Crypto.Common
{
    public class PackFileEntry : IComparable<PackFileEntry>
    {
        public int Index { get; set; } // The index of the file in the lookup table
        public string Hash { get; set; } // A hash assigned to all files in the directory
        public string Name { get; set; } // The full name of the file (path/name.ext)
        public string TreeName { get; set; } // The visual name displayed in the tree (name.ext)
        public PackFileHeaderVerBase FileHeader { get; set; } // The file information (size, offset, etc.)
        public byte[] Data { get; set; } // The raw, decrypted, and current data buffer of the file
        public bool Changed { get; set; } // If the data has been modified in the repacker

        public int CompareTo(PackFileEntry pObj)
        {
            if (this.Index == pObj.Index) return 0;
            if (this.Index > pObj.Index) return 1;
            return -1;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Hash))
            {
                return string.Format("{0},{1}\r\n", Index, Name);
            }
            return string.Format("{0},{1},{2}\r\n", Index, Hash, Name);
        }

        /*
         * Creates a collection of pack file entries from the file string.
         * 
         * @param sFileString The string containing a table of of files
         * 
         * @return A list of file entries with their index/hash/name loaded
         * 
        */
        public static List<PackFileEntry> CreateFileList(string sFileString)
        {
            List<PackFileEntry> aFileList = new List<PackFileEntry>();

            string[] aEntries = sFileString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string sEntry in aEntries)
            {
                int nProperties = 0;
                foreach (char c in sEntry)
                {
                    if (c == ',')
                        ++nProperties;
                }

                string sIndex, sName;
                if (nProperties == 1)
                {
                    sIndex = sEntry.Split(',')[0]; //strtok(pStr, ",")
                    sName = sEntry.Split(',')[1]; //strtok(pStr, ",")

                    aFileList.Add(new PackFileEntry
                    {
                        Index = int.Parse(sIndex), //atoi(sIndex)
                        Name = sName
                    });
                } else if (nProperties == 2)
                {
                    sIndex = sEntry.Split(',')[0]; //strtok(pStr, ",")
                    sName = sEntry.Split(',')[2]; //if (nPropertyIdx == 1)

                    aFileList.Add(new PackFileEntry
                    {
                        Index = int.Parse(sIndex), //atoi(sIndex)
                        Hash = sEntry.Split(',')[1], //if (!nPropertyIdx)
                        Name = sName
                    });
                }
            }

            return aFileList;
        }
    }
}

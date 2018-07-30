using Orion.Crypto.Stream;
using System;
using System.Collections.Generic;

namespace Orion.Crypto.Common
{
    public class PackFileEntry : IComparable<PackFileEntry>
    {
        public int Index { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string TreeName { get; set; }
        public PackFileHeaderVerBase FileHeader { get; set; }
        public byte[] Data { get; set; }
        public bool Changed { get; set; }

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

        public static List<PackFileEntry> CreateFileList(string sFileList)
        {
            List<PackFileEntry> aFileList = new List<PackFileEntry>();

            string[] aEntries = sFileList.Split('\n');
            foreach (string sEntry in aEntries)
            {
                if (!string.IsNullOrWhiteSpace(sEntry))
                {
                    string[] aProperties = sEntry.Replace("\r", "").Replace("\n", "").Split(',');

                    aFileList.Add(new PackFileEntry
                    {
                        Index = int.Parse(aProperties[0]),
                        Hash = aProperties.Length == 3 ? aProperties[1] : "",
                        Name = aProperties.Length == 3 ? aProperties[2] : aProperties[1]
                    });
                }
            }

            return aFileList;
        }
    }
}

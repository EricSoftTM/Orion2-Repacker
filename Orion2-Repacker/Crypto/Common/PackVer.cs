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

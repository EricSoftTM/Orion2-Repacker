using System;
using System.Drawing.Imaging;
using System.IO;

namespace Orion.Crypto.Stream.DDS
{
    public class DDSImage : IDisposable
    {
        private System.Drawing.Bitmap _bitmap;
        private bool _isValid;
        private bool _alpha;

        public System.Drawing.Bitmap BitmapImage
        {
            get { return _bitmap; }
        }

        public bool IsValid
        {
            get { return _isValid; }
        }

        public bool PreserveAlpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        public DDSImage(byte[] ddsImage, bool preserveAlpha = true)
        {
            if (ddsImage == null)
                return;

            if (ddsImage.Length == 0)
                return;

            _alpha = preserveAlpha;

            using (MemoryStream stream = new MemoryStream(ddsImage.Length))
            {
                stream.Write(ddsImage, 0, ddsImage.Length);
                stream.Seek(0, SeekOrigin.Begin);

                using (BinaryReader reader = new BinaryReader(stream))
                {
                    Parse(reader);
                }
            }
        }

        public DDSImage(System.IO.Stream ddsImage, bool preserveAlpha = true)
        {
            if (ddsImage == null)
                return;

            if (!ddsImage.CanRead)
                return;

            _alpha = preserveAlpha;

            using (BinaryReader reader = new BinaryReader(ddsImage))
            {
                Parse(reader);
            }
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        private void Parse(BinaryReader reader)
        {
            DDSStruct header = new DDSStruct();
            PixelFormat pixelFormat = PixelFormat.UNKNOWN;
            byte[] data = null;

            if (ReadHeader(reader, ref header))
            {
                _isValid = true;
                // patches for stuff
                if (header.depth == 0) header.depth = 1;

                uint blocksize = 0;
                pixelFormat = GetFormat(header, ref blocksize);
                if (pixelFormat == PixelFormat.UNKNOWN)
                {
                    throw new InvalidFileHeaderException();
                }

                data = ReadData(reader, header);
                if (data != null)
                {
                    byte[] rawData = Decompressor.Expand(header, data, pixelFormat);
                    _bitmap = CreateBitmap((int)header.width, (int)header.height, rawData);
                }
            }
        }

        private byte[] ReadData(BinaryReader reader, DDSStruct header)
        {
            byte[] compdata = null;
            uint compsize = 0;

            if ((header.flags & Helper.DDSD_LINEARSIZE) > 1)
            {
                compdata = reader.ReadBytes((int)header.sizeorpitch);
                compsize = (uint)compdata.Length;
            }
            else
            {
                uint bps = header.width * header.pixelformat.rgbbitcount / 8;
                compsize = bps * header.height * header.depth;
                compdata = new byte[compsize];

                MemoryStream mem = new MemoryStream((int)compsize);

                byte[] temp;
                for (int z = 0; z < header.depth; z++)
                {
                    for (int y = 0; y < header.height; y++)
                    {
                        temp = reader.ReadBytes((int)bps);
                        mem.Write(temp, 0, temp.Length);
                    }
                }
                mem.Seek(0, SeekOrigin.Begin);

                mem.Read(compdata, 0, compdata.Length);
                mem.Close();
            }

            return compdata;
        }

        private System.Drawing.Bitmap CreateBitmap(int width, int height, byte[] rawData)
        {
            var pxFormat = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            if (_alpha)
                pxFormat = System.Drawing.Imaging.PixelFormat.Format32bppArgb;

            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height, pxFormat);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height)
                , ImageLockMode.WriteOnly, pxFormat);
            IntPtr scan = data.Scan0;
            int size = bitmap.Width * bitmap.Height * 4;

            unsafe
            {
                byte* p = (byte*)scan;
                for (int i = 0; i < size; i += 4)
                {
                    // iterate through bytes.
                    // Bitmap stores it's data in RGBA order.
                    // DDS stores it's data in BGRA order.
                    p[i] = rawData[i + 2]; // blue
                    p[i + 1] = rawData[i + 1]; // green
                    p[i + 2] = rawData[i];   // red
                    p[i + 3] = rawData[i + 3]; // alpha
                }
            }

            bitmap.UnlockBits(data);
            return bitmap;
        }

        private bool ReadHeader(BinaryReader reader, ref DDSStruct header)
        {
            byte[] signature = reader.ReadBytes(4);
            if (!(signature[0] == 'D' && signature[1] == 'D' && signature[2] == 'S' && signature[3] == ' '))
                return false;

            header.size = reader.ReadUInt32();
            if (header.size != 124)
                return false;

            //convert the data
            header.flags = reader.ReadUInt32();
            header.height = reader.ReadUInt32();
            header.width = reader.ReadUInt32();
            header.sizeorpitch = reader.ReadUInt32();
            header.depth = reader.ReadUInt32();
            header.mipmapcount = reader.ReadUInt32();
            header.alphabitdepth = reader.ReadUInt32();

            header.reserved = new uint[10];
            for (int i = 0; i < 10; i++)
            {
                header.reserved[i] = reader.ReadUInt32();
            }

            //pixelfromat
            header.pixelformat.size = reader.ReadUInt32();
            header.pixelformat.flags = reader.ReadUInt32();
            header.pixelformat.fourcc = reader.ReadUInt32();
            header.pixelformat.rgbbitcount = reader.ReadUInt32();
            header.pixelformat.rbitmask = reader.ReadUInt32();
            header.pixelformat.gbitmask = reader.ReadUInt32();
            header.pixelformat.bbitmask = reader.ReadUInt32();
            header.pixelformat.alphabitmask = reader.ReadUInt32();

            //caps
            header.ddscaps.caps1 = reader.ReadUInt32();
            header.ddscaps.caps2 = reader.ReadUInt32();
            header.ddscaps.caps3 = reader.ReadUInt32();
            header.ddscaps.caps4 = reader.ReadUInt32();
            header.texturestage = reader.ReadUInt32();

            return true;
        }

        private PixelFormat GetFormat(DDSStruct header, ref uint blocksize)
        {
            PixelFormat format = PixelFormat.UNKNOWN;
            if ((header.pixelformat.flags & Helper.DDPF_FOURCC) == Helper.DDPF_FOURCC)
            {
                blocksize = ((header.width + 3) / 4) * ((header.height + 3) / 4) * header.depth;

                switch (header.pixelformat.fourcc)
                {
                    case Helper.FOURCC_DXT1:
                        format = PixelFormat.DXT1;
                        blocksize *= 8;
                        break;

                    case Helper.FOURCC_DXT2:
                        format = PixelFormat.DXT2;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_DXT3:
                        format = PixelFormat.DXT3;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_DXT4:
                        format = PixelFormat.DXT4;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_DXT5:
                        format = PixelFormat.DXT5;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_ATI1:
                        format = PixelFormat.ATI1N;
                        blocksize *= 8;
                        break;

                    case Helper.FOURCC_ATI2:
                        format = PixelFormat.THREEDC;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_RXGB:
                        format = PixelFormat.RXGB;
                        blocksize *= 16;
                        break;

                    case Helper.FOURCC_DOLLARNULL:
                        format = PixelFormat.A16B16G16R16;
                        blocksize = header.width * header.height * header.depth * 8;
                        break;

                    case Helper.FOURCC_oNULL:
                        format = PixelFormat.R16F;
                        blocksize = header.width * header.height * header.depth * 2;
                        break;

                    case Helper.FOURCC_pNULL:
                        format = PixelFormat.G16R16F;
                        blocksize = header.width * header.height * header.depth * 4;
                        break;

                    case Helper.FOURCC_qNULL:
                        format = PixelFormat.A16B16G16R16F;
                        blocksize = header.width * header.height * header.depth * 8;
                        break;

                    case Helper.FOURCC_rNULL:
                        format = PixelFormat.R32F;
                        blocksize = header.width * header.height * header.depth * 4;
                        break;

                    case Helper.FOURCC_sNULL:
                        format = PixelFormat.G32R32F;
                        blocksize = header.width * header.height * header.depth * 8;
                        break;

                    case Helper.FOURCC_tNULL:
                        format = PixelFormat.A32B32G32R32F;
                        blocksize = header.width * header.height * header.depth * 16;
                        break;

                    default:
                        format = PixelFormat.UNKNOWN;
                        blocksize *= 16;
                        break;
                }
            }
            else
            {
                // uncompressed image
                if ((header.pixelformat.flags & Helper.DDPF_LUMINANCE) == Helper.DDPF_LUMINANCE)
                {
                    if ((header.pixelformat.flags & Helper.DDPF_ALPHAPIXELS) == Helper.DDPF_ALPHAPIXELS)
                    {
                        format = PixelFormat.LUMINANCE_ALPHA;
                    }
                    else
                    {
                        format = PixelFormat.LUMINANCE;
                    }
                }
                else
                {
                    if ((header.pixelformat.flags & Helper.DDPF_ALPHAPIXELS) == Helper.DDPF_ALPHAPIXELS)
                    {
                        format = PixelFormat.RGBA;
                    }
                    else
                    {
                        format = PixelFormat.RGB;
                    }
                }

                blocksize = (header.width * header.height * header.depth * (header.pixelformat.rgbbitcount >> 3));
            }

            return format;
        }
    }
}
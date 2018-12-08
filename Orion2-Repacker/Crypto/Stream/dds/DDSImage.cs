using Pfim;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Orion.Crypto.Stream.DDS
{
    public class DDSImage : IDisposable
    {
        private readonly IImage _image;
        private Bitmap _bitmap;

        public Bitmap BitmapImage
        {
            get { return _bitmap; }
        }

        public DDSImage(byte[] ddsImage)
        {
            if (ddsImage == null)
                return;

            if (ddsImage.Length == 0)
                return;

            _image = Dds.Create(ddsImage, new PfimConfig());
            Parse();
        }

        public DDSImage(System.IO.Stream ddsImage)
        {
            if (ddsImage == null)
                return;

            if (!ddsImage.CanRead)
                return;

            _image = Dds.Create(ddsImage, new PfimConfig());
            Parse();
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }

        private void Parse()
        {
            if (_image == null)
                throw new Exception("Image data failed to create within Pfim");

            if (_image.Compressed)
                _image.Decompress();

            _bitmap = CreateBitmap(_image);
        }

        private Bitmap CreateBitmap(IImage image)
        {
            var pxFormat = PixelFormat.Format24bppRgb;
            if (_image.Format == Pfim.ImageFormat.Rgba32)
                pxFormat = PixelFormat.Format32bppArgb;

            unsafe
            {
                fixed (byte* bytePtr = image.Data)
                {
                    return new Bitmap(image.Width, image.Height, image.Stride, pxFormat, (IntPtr) bytePtr);
                }
            }
        }
    }
}
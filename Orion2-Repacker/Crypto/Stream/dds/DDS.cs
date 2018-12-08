using System.Drawing;
using System.IO;

namespace Orion.Crypto.Stream.DDS
{
    /// <summary>
    /// This is the main class of the library.  All static methods are contained within.
    /// </summary>
    public class DDS
    {
        /// <summary>
        /// Loads a DDS image from a byte array, and returns a Bitmap object of the image.
        /// </summary>
        /// <param name="data">The image data, as a byte array.</param>
        /// <returns>The Bitmap representation of the image.</returns>
        public static Bitmap LoadImage(byte[] data)
        {
            DDSImage im = new DDSImage(data);
            return im.BitmapImage;
        }

        /// <summary>
        /// Loads a DDS image from a file, and returns a Bitmap object of the image.
        /// </summary>
        /// <param name="file">The image file.</param>
        /// <returns>The Bitmap representation of the image.</returns>
        public static Bitmap LoadImage(string file)
        {
            byte[] data = File.ReadAllBytes(file);
            DDSImage im = new DDSImage(data);
            return im.BitmapImage;
        }

        /// <summary>
        /// Loads a DDS image from a Stream, and returns a Bitmap object of the image.
        /// </summary>
        /// <param name="stream">The stream to read the image data from.</param>
        /// <returns>The Bitmap representation of the image.</returns>
        public static Bitmap LoadImage(System.IO.Stream stream)
        {
            DDSImage im = new DDSImage(stream);
            return im.BitmapImage;
        }
    }
}
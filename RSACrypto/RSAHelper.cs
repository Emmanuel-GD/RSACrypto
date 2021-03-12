
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace RSACrypto
{
    class RSAHelper
    {
        public static byte[] DecodeHex(string hextext)
        {
            String[] arr = hextext.Split('-');
            byte[] array = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                array[i] = Convert.ToByte(arr[i], 16);
            }

            return array;
        }

        public static Bitmap ConvertByteToImage(byte[] bytes)
        {
            return (new Bitmap(Image.FromStream(new MemoryStream(bytes))));
        }

        public static byte[] ConvertImageToByte(Image My_Image)
        {
            MemoryStream m1 = new MemoryStream();
            new Bitmap(My_Image).Save(m1, System.Drawing.Imaging.ImageFormat.Jpeg);
            return m1.ToArray();
        }

        public static byte[] ConvertTextToByte(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text.ToCharArray());
            return bytes;
        }

        public static string ConvertByteToText(byte[] bytes)
        {
            var x = Encoding.Unicode.GetString(bytes);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}

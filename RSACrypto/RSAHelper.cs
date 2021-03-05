
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

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

        public static bool IsPrime(int number)
        {
            if (number < 2) return false;
            if (number % 2 == 0) return (number == 2);
            int root = (int)Math.Sqrt((double)number);
            for (int i = 3; i <= root; i += 2)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }

        public static Bitmap ConvertByteToImage(byte[] bytes)
        {
            return (new Bitmap(Image.FromStream(new MemoryStream(bytes))));
        }

        public static byte[] ConvertImageToByte(Image My_Image)
        {
            MemoryStream m1 = new MemoryStream();
            new Bitmap(My_Image).Save(m1, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] header = new byte[] { 255, 216 };
            header = m1.ToArray();
            return (header);
        }

        public static List<int> GeneratePrimes(int n)
        {
            var primes = from i in Enumerable.Range(2, n - 1)
                         .AsParallel()
                         where Enumerable.Range(1, (int)Math.Sqrt(i))
                         .All(j => j == 1 || i % j != 0)
                         select i;
            return primes.ToList();
        }

        public static ulong[] GetTwoPrimes(List<int> primes)
        {
            ulong[] pq = new ulong[2];
            Random random = new Random();
            pq[0] = (ulong)random.Next(0, primes.Count - 1);
            pq[1] = (ulong)random.Next(0, primes.Count - 1);
            return pq;
        }
    }
}

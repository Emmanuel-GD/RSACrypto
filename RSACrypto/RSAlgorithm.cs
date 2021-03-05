using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Drawing;

namespace RSACrypto
{
    class RSAlgorithm
    {
        public static ulong Square(ulong a)
        {
            return a * a;
        }
        public static ulong BigMod(ulong cipher, ulong key, ulong N) //b^p%m=?
        {
            if (key == 0)
            {
                return 1;
            }
            else if (key % 2 == 0)
            {
                return Square(BigMod(cipher, key / 2, N)) % N;
            }
            else
            {
                return ((cipher % N) * BigMod(cipher, key - 1, N)) % N;
            }
        }
        public static ulong N(ulong prime1, ulong prime2)
        {
            return (prime1 * prime2);
        }
        public static ulong Phi(ulong prime1, ulong prime2)
        {
            return ((prime1 - 1) * (prime2 - 1));
        }
        public static bool Coprime(ulong u, ulong v)
        {
            if (((u | v) & 1) == 0)
            {
                return false;
            }

            while ((u & 1) == 0)
            {
                u >>= 1;
            }

            if (u == 1)
            {
                return true;
            }

            do
            {
                while ((v & 1) == 0)
                {
                    v >>= 1;
                }

                if (v == 1)
                {
                    return true;
                }

                if (u > v)
                {
                    ulong t = v; v = u; u = t;
                }
                v -= u;
            } while (v != 0);
            return false;
        }
        public static ulong Find_E(ulong phi)
        {
            List<ulong> AllCoprimes = new List<ulong>();
            ulong e = 0;
            for (ulong i = 2; i < phi; i++)
            {
                if (Coprime(i, phi))
                {
                    AllCoprimes.Add(i);
                    break;
                }
            }
            e = AllCoprimes[0];
            return e;
        }


    }
}

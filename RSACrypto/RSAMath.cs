using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RSACrypto
{
    public class RSAMath
    {
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

        public static List<int> GeneratePrimes(int n)
        {
            var primes = from i in Enumerable.Range(2, n - 1)
                         .AsParallel()
                         where Enumerable.Range(1, (int)Math.Sqrt(i))
                         .All(j => j == 1 || i % j != 0)
                         select i;
            return primes.ToList();
        }

        public static int[] GetTwoPrimes(List<int> primes)
        {
            int[] pq = new int[2];
            Random random = new Random();
            pq[0] = random.Next(0, primes.Count - 1);
            pq[1] = random.Next(0, primes.Count - 1);
            return pq;
        }

        public static object[] PopulateKeys(ulong e, ulong d, ulong p, ulong q, params TextBox[] textBoxes)
        {
            var primes = GeneratePrimes(10000);
            var result = GetTwoPrimes(primes);

            string Prime1 = primes[(int)result[0]].ToString();
            string Prime2 = primes[(int)result[1]].ToString();
            
            var N = RSAlgorithm
                        .N((ulong)primes[result[0]], (ulong)primes[result[1]]);
            p = Convert.ToUInt64(primes[result[0]]);
            q = Convert.ToUInt64(primes[result[1]]);

            textBoxes[0].Text = Prime1;
            textBoxes[1].Text = Prime2;
            textBoxes[2].Text = N.ToString();

            var phi = RSAlgorithm.Phi(p, q);
            e = RSAlgorithm.Find_E(phi);
            bool flag = false;
            for (ulong j = 1; !flag; j++)
            {
                if ((e * j) % phi == 1)
                {
                    d = j;
                    flag = true;
                }
            }
            textBoxes[3].Text = e.ToString();
            textBoxes[4].Text = d.ToString(); ;
            var returnValues = new object[] { p, q, N, e, d };
            return returnValues;
        }
    }
}

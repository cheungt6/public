using System;

namespace Palladium2020
{
    class Program
    {
        static void Main(string[] args)
        {
            var H = new[] { 1, 1, 7, 6, 6, 6 };
            var ans = solution(H);

        }

        // Second go at solution, using an array to hold the max seen hight at any position
        public static int solution(int[] H)
        {
            if (H.Length == 0)
                return 0;
            else if (H.Length == 1)
                return H[0];
            else
            {
                var bestResult = int.MaxValue;
                var maxArray1 = new int[H.Length];
                var maxArray2 = new int[H.Length];
                var maxArray1Total = 0;
                var maxArray2Total = 0;
                {
                    var maxSeen1 = int.MinValue;
                    var maxSeen2 = int.MinValue;
                    for (int i = 0; i < H.Length; i++)
                    {
                        if (H[i] > maxSeen1)
                            maxSeen1 = H[i];

                        if (H[H.Length - i - 1] > maxSeen2)
                            maxSeen2 = H[H.Length - i - 1];

                        maxArray1[i] = maxSeen1;
                        maxArray1Total += maxSeen1;

                        maxArray2[i] = maxSeen2;
                        maxArray2Total += maxSeen2;
                    }

                    // working from left and right, figure out the least required canvas areas
                    for (int i = 0; i < H.Length; i++)
                    {
                        var total1 = (maxArray1[i] * (i + 1)) + (maxArray1[H.Length - 1] * (H.Length - i - 1));
                        var total2 = (maxArray2[i] * (i + 1)) + (maxArray2[H.Length - 1] * (H.Length - i - 1));
                        if (total1 < bestResult)
                            bestResult = total1;
                        if (total2 < bestResult)
                            bestResult = total2;
                    }
                    return bestResult;
                }
            }
        }

        // Brute force answer at every position
        public static int solution1(int[] H)
        {
            if (H.Length == 0)
                return 0;
            else if (H.Length == 1)
                return H[0];
            else
            {
                var bestResult = -1;
                for (int i = 0; i < H.Length; i++)
                {
                    var max1 = Max(H, 0, i);
                    var max2 = Max(H, i + 1, H.Length - 1);
                    var total = (max1 * (i + 1)) + ((max2 * (H.Length - (i + 1))));
                    if (bestResult == -1 || bestResult > total)
                        bestResult = total;
                }
                return bestResult;
            }
        }

        private static int Max(int[] h, int start, int finish)
        {
            var max = int.MinValue;
            for (int i = start; i < finish + 1; i++)
                if (max < h[i])
                    max = h[i];
            return max;
        }
    }
}

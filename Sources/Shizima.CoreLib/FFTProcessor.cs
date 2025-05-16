using System.Numerics;

namespace Shizima.CoreLib
{
    public static class FFTProcessor
    {
        /// <summary>
        /// Performs an in-place iterative FFT using the Cooley-Tukey algorithm
        /// </summary>
        public static Complex[] ComputeFFT(float[] samples)
        {
            int n = samples.Length;

            // Ensure the size is a power of 2
            if (n <= 0 || (n & (n - 1)) != 0)
                throw new ArgumentException("Sample size must be a power of 2");

            Complex[] data = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                data[i] = new Complex(samples[i], 0);
            }

            // Bit-reversal permutation
            int logN = (int)Math.Log2(n);
            for (int i = 0; i < n; i++)
            {
                int reversedIndex = ReverseBits(i, logN);
                if (i < reversedIndex)
                {
                    (data[i], data[reversedIndex]) = (data[reversedIndex], data[i]);
                }
            }

            // Iterative FFT
            for (int len = 2; len <= n; len *= 2)
            {
                double angle = -2 * Math.PI / len;
                Complex wLen = new Complex(Math.Cos(angle), Math.Sin(angle));

                for (int i = 0; i < n; i += len)
                {
                    Complex w = Complex.One;
                    for (int j = 0; j < len / 2; j++)
                    {
                        Complex even = data[i + j];
                        Complex odd = data[i + j + len / 2] * w;

                        data[i + j] = even + odd;
                        data[i + j + len / 2] = even - odd;

                        w *= wLen;
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Reverses the bits of an index for bit-reversal ordering
        /// </summary>
        private static int ReverseBits(int index, int bitCount)
        {
            int reversed = 0;
            for (int i = 0; i < bitCount; i++)
            {
                reversed = (reversed << 1) | (index & 1);
                index >>= 1;
            }
            return reversed;
        }
    }
}

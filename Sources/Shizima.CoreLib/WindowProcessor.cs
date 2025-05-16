namespace Shizima.CoreLib
{
    internal static class WindowProcessor
    {
        /// <summary>
        /// Applies a Hamming windowing function
        /// </summary>
        internal static float[] ApplyHammingWindow(float[] chunk)
        {
            float[] windowed = new float[chunk.Length];
            for (int i = 0; i < chunk.Length; i++)
            {
                double window = 0.54 - 0.46 * Math.Cos((2 * Math.PI * i) / (chunk.Length - 1));
                windowed[i] = (float)(chunk[i] * window);
            }
            return windowed;
        }

        /// <summary>
        /// Applies a Blackman windowing function
        /// </summary>
        internal static float[] ApplyBlackmanWindow(float[] chunk)
        {
            float[] windowed = new float[chunk.Length];
            for (int i = 0; i < chunk.Length; i++)
            {
                double window = 0.42 - 0.5 * Math.Cos((2 * Math.PI * i) / (chunk.Length - 1)) + 0.08 * Math.Cos((4 * Math.PI * i) / (chunk.Length - 1));
                windowed[i] = (float)(chunk[i] * window);
            }
            return windowed;
        }

        /// <summary>
        /// Applies a Hann windowing function
        /// </summary>
        internal static float[] ApplyHannWindow(float[] chunk)
        {
            float[] windowed = new float[chunk.Length];
            for (int i = 0; i < chunk.Length; i++)
            {
                double window = 0.5 * (1 - Math.Cos((2 * Math.PI * i) / (chunk.Length - 1)));
                windowed[i] = (float)(chunk[i] * window);
            }
            return windowed;
        }

        /// <summary>
        /// Applies a Kaiser windowing function
        /// </summary>
        internal static float[] ApplyKaiserWindow(float[] chunk, double beta)
        {
            float[] windowed = new float[chunk.Length];
            double denominator = BesselI0(beta);
            for (int i = 0; i < chunk.Length; i++)
            {
                double ratio = (2.0 * i) / (chunk.Length - 1) - 1.0;
                double window = BesselI0(beta * Math.Sqrt(1.0 - ratio * ratio)) / denominator;
                windowed[i] = (float)(chunk[i] * window);
            }
            return windowed;
        }


        /// <summary>
        /// Computes the zeroth order modified Bessel function of the first kind
        /// </summary>
        private static double BesselI0(double x)
        {
            double sum = 1.0;
            double term = 1.0;
            for (int k = 1; k < 25; k++)
            {
                term *= (x * x) / (4 * k * k);
                sum += term;
            }
            return sum;
        }

    }
}

using Shizima.CoreLib.Entities;
using System.Numerics;

namespace Shizima.CoreLib
{
    public static class AudioProcessor
    {
        /// <summary>
        /// Converts raw audio data to PCM samples
        /// </summary>
        public static float[] ConvertByteArrayToPcm(byte[] audioData, int bitsPerSample = 16, int channels = 2)
        {
            if (audioData == null || audioData.Length == 0)
                return new float[0];

            int bytesPerSample = bitsPerSample / 8;
            int sampleCount = audioData.Length / (bytesPerSample * channels);
            float[] samples = new float[sampleCount];

            if (bitsPerSample == 16)
            {
                short[] shortSamples = new short[audioData.Length / 2];
                Buffer.BlockCopy(audioData, 0, shortSamples, 0, audioData.Length);

                for (int i = 0; i < sampleCount; i++)
                {
                    float sum = 0;
                    for (int c = 0; c < channels; c++)
                    {
                        sum += shortSamples[i * channels + c];
                    }
                    samples[i] = sum / (channels * 32768.0f);
                }
            }
            else if (bitsPerSample == 24)
            {
                for (int i = 0; i < sampleCount; i++)
                {
                    float sum = 0;
                    for (int c = 0; c < channels; c++)
                    {
                        int index = (i * channels + c) * 3;
                        int value = (audioData[index] << 16) | (audioData[index + 1] << 8) | (audioData[index + 2]);

                        // Sign extend 24-bit value
                        if ((value & 0x800000) != 0)
                            value |= unchecked((int)0xFF000000);

                        sum += value / 8388608.0f;
                    }
                    samples[i] = sum / channels;
                }
            }
            else if (bitsPerSample == 32)
            {
                float[] floatSamples = new float[audioData.Length / 4];
                Buffer.BlockCopy(audioData, 0, floatSamples, 0, audioData.Length);

                for (int i = 0; i < sampleCount; i++)
                {
                    float sum = 0;
                    for (int c = 0; c < channels; c++)
                    {
                        sum += floatSamples[i * channels + c];
                    }
                    samples[i] = sum / channels;
                }
            }

            return samples;
        }

        /// <summary>
        /// Resamples the audio signal to 44.1kHz if necessary
        /// </summary>
        public static float[] ResampleIfNeeded(float[] samples, int originalSampleRate)
        {
            if (originalSampleRate == Constants.SampleRate)
                return samples;

            int outputLength = (int)((long)samples.Length * Constants.SampleRate / originalSampleRate);
            float[] resampled = new float[outputLength];

            for (int i = 0; i < outputLength; i++)
            {
                float position = i * originalSampleRate / (float)Constants.SampleRate;
                int index = (int)position;
                float fraction = position - index;

                if (index + 1 < samples.Length)
                    resampled[i] = samples[index] * (1 - fraction) + samples[index + 1] * fraction;
                else
                    resampled[i] = samples[index];
            }

            return resampled;
        }

        /// <summary>
        /// Calculates the spectrogram from an audio signal
        /// </summary>
        public static float[,] ComputeSpectrogram(float[] samples)
        {
            int numChunks = Math.Max(1, (samples.Length - Constants.ChunkOverlap) / (Constants.ChunkSize - Constants.ChunkOverlap));
            float[,] spectrogram = new float[numChunks, Constants.FftBinCount];

            for (int i = 0; i < numChunks; i++)
            {
                int startIndex = i * (Constants.ChunkSize - Constants.ChunkOverlap);
                if (startIndex + Constants.ChunkSize > samples.Length)
                    break;

                float[] chunk = new float[Constants.ChunkSize];
                Array.Copy(samples, startIndex, chunk, 0, Constants.ChunkSize);
                chunk = WindowProcessor.ApplyBlackmanWindow(chunk);

                Complex[] fftResult = FFTProcessor.ComputeFFT(chunk);

                for (int j = 0; j < Constants.FftBinCount; j++)
                {
                    float magnitude = Math.Max((float)fftResult[j].Magnitude, 1e-9f);
                    float db = 20 * (float)Math.Log10(magnitude);
                    db = Math.Max(db, -100);

                    if (db > Constants.NoiseThreshold)
                    {
                        spectrogram[i, j] = db;
                    }
                    else
                    {
                        spectrogram[i, j] = Constants.NoiseThreshold;
                    }
                }
            }

            return spectrogram;
        }

        /// <summary>
        /// Converts FFT bin indices to logarithmic frequency bands
        /// </summary>
        internal static int FrequencyBinToLogBand(int freqBin)
        {
            double freq = freqBin * Constants.SampleRate / (double)Constants.ChunkSize;

            if (freq < Constants.MinFreq || freq > Constants.MaxFreq)
                return -1;

            double minLog = Math.Log(Constants.MinFreq);
            double maxLog = Math.Log(Constants.MaxFreq);
            double logFreq = Math.Log(freq);

            int band = (int)((logFreq - minLog) / (maxLog - minLog) * Constants.NumFreqBands);
            return Math.Min(Math.Max(band, 0), Constants.NumFreqBands - 1);
        }
    }
}

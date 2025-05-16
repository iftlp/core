using Shizima.CoreLib.Entities;

namespace Shizima.CoreLib
{
    public static class FingerprintGenerator
    {
        /// <summary>
        /// Finds the anchor points (energy peaks) in the spectrogram
        /// </summary>
        public static List<AnchorPoint> FindAnchorPoints(float[,] spectrogram)
        {
            List<AnchorPoint> anchorPoints = new List<AnchorPoint>();
            int timeFrames = spectrogram.GetLength(0);
            int freqBins = spectrogram.GetLength(1);

            for (int timeIndex = 0; timeIndex < timeFrames; timeIndex++)
            {
                Dictionary<int, float> bandEnergies = new Dictionary<int, float>();

                for (int freqBin = 0; freqBin < freqBins; freqBin++)
                {
                    int band = AudioProcessor.FrequencyBinToLogBand(freqBin);
                    if (band >= 0)
                    {
                        float energy = spectrogram[timeIndex, freqBin];
                        if (energy > Constants.NoiseThreshold)
                        {
                            bandEnergies.TryGetValue(band, out float existingEnergy);
                            bandEnergies[band] = Math.Max(existingEnergy, energy);
                        }
                    }
                }

                var peaks = bandEnergies
                    .OrderByDescending(pair => pair.Value)
                    .Take(Constants.PeaksPerFrame)
                    .ToList();

                foreach (var peak in peaks)
                {
                    anchorPoints.Add(new AnchorPoint
                    {
                        TimeIndex = timeIndex,
                        FreqBand = peak.Key,
                        Energy = peak.Value
                    });
                }
            }

            return anchorPoints;
        }

        /// <summary>
        /// Computes a hash from a pair of points
        /// </summary>
        internal static uint ComputeHash(int freq1, int freq2, int timeDelta)
        {
            return ((uint)(freq1 & 0xFFF) << 20) | ((uint)(freq2 & 0xFFF) << 8) | ((uint)(timeDelta & 0xFF));
        }

        /// <summary>
        /// Generates fingerprints from anchor points
        /// </summary>
        public static List<Fingerprint> GenerateFingerprints(List<AnchorPoint> anchorPoints)
        {
            List<Fingerprint> fingerprints = new List<Fingerprint>();

            for (int i = 0; i < anchorPoints.Count; i++)
            {
                AnchorPoint anchorPoint = anchorPoints[i];

                for (int j = i + 1; j < Math.Min(i + 1 + Constants.FanoutFactor, anchorPoints.Count); j++)
                {
                    AnchorPoint targetPoint = anchorPoints[j];

                    int timeDelta = targetPoint.TimeIndex - anchorPoint.TimeIndex;

                    if (timeDelta >= Constants.MinHashTimeDelta && timeDelta <= Constants.MaxHashTimeDelta)
                    {
                        uint hash = ComputeHash(anchorPoint.FreqBand, targetPoint.FreqBand, timeDelta);

                        fingerprints.Add(new Fingerprint
                        {
                            Hash = hash,
                            TimeOffset = anchorPoint.TimeIndex
                        });
                    }
                }
            }

            return fingerprints;
        }

        /// <summary>
        /// Analyzes the audio signal and generates fingerprints
        /// </summary>
        public static List<Fingerprint> AnalyzeAudio(byte[] audioData, int sampleRate = 44100, int bitsPerSample = 16, int channels = 2)
        {
            if (audioData == null || audioData.Length == 0)
                return new List<Fingerprint>();

            float[] samples = AudioProcessor.ConvertByteArrayToPcm(audioData, bitsPerSample, channels);

            if (sampleRate != Constants.SampleRate)
                samples = AudioProcessor.ResampleIfNeeded(samples, sampleRate);

            float[,] spectrogram = AudioProcessor.ComputeSpectrogram(samples);

            List<AnchorPoint> anchorPoints = FindAnchorPoints(spectrogram);

            return GenerateFingerprints(anchorPoints);
        }
    }
}

namespace Shizima.CoreLib.Entities
{
    internal class Constants
    {
        // Audio Processing
        internal const int SampleRate = 44100;  // Standard WAV sample rate
        internal const int ChunkSize = 4096;    // FFT window size
        internal const int ChunkOverlap = ChunkSize / 2;  // 50% overlap

        // FFT and Frequency Bands
        internal const int FftBinCount = ChunkSize / 2;  // FFT bins (Nyquist limit)
        internal const int MinFreq = 40;     // Minimum frequency considered
        internal const int MaxFreq = 15000;  // Maximum frequency considered
        internal const int NumFreqBands = 32;  // Number of logarithmic frequency bands

        // Feature Extraction
        internal const int PeaksPerFrame = 5;  // Number of peaks per frame
        internal const int FanoutFactor = 15;  // Number of anchor points linked

        // Hashing Parameters
        internal const int MinHashTimeDelta = 1;   // Minimum time difference for hashing
        internal const int MaxHashTimeDelta = 100; // Maximum time difference for hashing

        // Recognition Thresholds
        // internal const int MinMatchesForRecognition = 50000;  // Minimum matches required
        internal const double ScoreThreshold = 0.0;  // Confidence threshold for recognition
        internal const float NoiseThreshold = -80.0f; // Threshold in dB
    }
}

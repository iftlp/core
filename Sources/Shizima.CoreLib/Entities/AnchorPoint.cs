namespace Shizima.CoreLib.Entities
{
    /// <summary>
    /// Represent an energy point of the spectrogram
    /// </summary>
    public struct AnchorPoint
    {
        /// <summary>
        /// Time index
        /// </summary>
        public int TimeIndex { get; set; }

        /// <summary>
        /// Frequency band
        /// </summary>
        public int FreqBand { get; set; }

        /// <summary>
        /// Energy of the point
        /// </summary>
        public float Energy { get; set; }
    }
}

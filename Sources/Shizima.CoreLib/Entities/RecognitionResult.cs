namespace Shizima.CoreLib.Entities
{
    /// <summary>
    /// Result from an audio recogntion
    /// </summary>
    public class RecognitionResult
    {
        /// <summary>
        /// The id of the music
        /// </summary>
        public string TrackId { get; set; }

        /// <summary>
        /// The confidence of this recognition
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// The time offset (in milliseconds)
        /// </summary>
        public int TimeOffset { get; set; }

        /// <summary>
        /// The number of match
        /// </summary>
        public int MatchCount { get; set; }
    }
}

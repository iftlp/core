namespace Shizima.CoreLib.Entities
{
    /// <summary>
    /// Represent an audio fingerprint
    /// </summary>
    public struct Fingerprint
    {
        /// <summary>
        /// Hash value of the fingerprint
        /// </summary>
        public uint Hash { get; set; }

        /// <summary>
        /// Time offset from the audio begining
        /// </summary>
        public int TimeOffset { get; set; }
    }
}

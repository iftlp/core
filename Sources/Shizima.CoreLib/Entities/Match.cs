namespace Shizima.CoreLib.Entities
{
    /// <summary>
    /// Represent values of a audio match
    /// </summary>
    public struct Match
    {
        /// <summary>
        /// Time offset
        /// </summary>
        public int QueryTimeOffset { get; set; }

        /// <summary>
        /// Reference time offset
        /// </summary>
        public int ReferenceTimeOffset { get; set; }

        /// <summary>
        /// Hash of the fingerprint
        /// </summary>
        public uint Hash { get; set; }
    }
}

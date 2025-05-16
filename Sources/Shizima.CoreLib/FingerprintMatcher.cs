using System.Diagnostics;
using Shizima.CoreLib.Entities;

namespace Shizima.CoreLib
{
    public static class FingerprintMatcher
    {
        /// <summary>
        /// Finds matches between two sets of fingerprints
        /// </summary>
        public static List<Match> FindMatches(List<Fingerprint> queryFingerprints, List<Fingerprint> referenceFingerprints)
        {
            if (queryFingerprints == null || referenceFingerprints == null || queryFingerprints.Count == 0 || referenceFingerprints.Count == 0)
                return new List<Match>();

            List<Match> matches = new List<Match>();
            Dictionary<uint, List<int>> hashTable = new Dictionary<uint, List<int>>();

            // Build hash table with all time offsets for each hash
            foreach (var fingerprint in referenceFingerprints)
            {
                if (!hashTable.TryGetValue(fingerprint.Hash, out var timeOffsets))
                {
                    timeOffsets = new List<int>();
                    hashTable[fingerprint.Hash] = timeOffsets;
                }
                timeOffsets.Add(fingerprint.TimeOffset);
            }

            // Find matches
            foreach (var queryFingerprint in queryFingerprints)
            {
                if (hashTable.TryGetValue(queryFingerprint.Hash, out var refTimeOffsets))
                {
                    foreach (var refTimeOffset in refTimeOffsets)
                    {
                        matches.Add(new Match
                        {
                            QueryTimeOffset = queryFingerprint.TimeOffset,
                            ReferenceTimeOffset = refTimeOffset,
                            Hash = queryFingerprint.Hash
                        });
                    }
                }
            }

            return matches;
        }

        /// <summary>
        /// Analyzes matches to determine the best result
        /// </summary>
        public static RecognitionResult? AnalyzeMatches(List<Match> matches, double minMatchesForRecognition, long trackId = -1)
        {
            if (matches == null || matches.Count <= minMatchesForRecognition)
                return null;

            const int BucketTolerance = 5; // Acceptable variation in offset
            var offsetHistogram = new Dictionary<int, int>();

            // Count matches by time offset delta
            foreach (var match in matches)
            {
                int offset = match.ReferenceTimeOffset - match.QueryTimeOffset;
                
                if (!offsetHistogram.ContainsKey(offset))
                {
                    offsetHistogram[offset] = 0;
                }
                offsetHistogram[offset]++;
            }

            // Group similar offsets into buckets
            var bucketedHistogram = new Dictionary<int, int>();
            
            foreach (var entry in offsetHistogram)
            {
                int offset = entry.Key;
                int count = entry.Value;
                
                // Find or create a bucket
                bool addedToBucket = false;
                foreach (var bucket in bucketedHistogram.Keys.ToList())
                {
                    if (Math.Abs(bucket - offset) <= BucketTolerance)
                    {
                        bucketedHistogram[bucket] += count;
                        addedToBucket = true;
                        break;
                    }
                }
                
                if (!addedToBucket)
                {
                    bucketedHistogram[offset] = count;
                }
            }

            // Find the best bucket
            var bestOffset = 0;
            var bestCount = 0;
            
            foreach (var bucket in bucketedHistogram)
            {
                if (bucket.Value > bestCount)
                {
                    bestOffset = bucket.Key;
                    bestCount = bucket.Value;
                }
            }

            if (bestCount < minMatchesForRecognition)
                return null;
            
            // Estimate time offset in milliseconds
            int timeOffsetMs = bestOffset * (Constants.ChunkSize - Constants.ChunkOverlap) * 1000 / Constants.SampleRate;
            timeOffsetMs = Math.Abs(timeOffsetMs);
            
            return new RecognitionResult
            {
                TrackId = trackId,
                Confidence = 0,
                TimeOffset = timeOffsetMs,
                MatchCount = bestCount,
            };
        }
    }
}
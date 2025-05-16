using Shizima.CoreLib.Entities;

namespace Shizima.CoreLib
{
    public static class AudioRecognizer
    {
        public static RecognitionResult? RecognizeAudio(
            List<Fingerprint> queryFingerprints,
            Dictionary<string, List<Fingerprint>> database)
        {
            return RecognizeAudioList(queryFingerprints, database)
                .FirstOrDefault();
        }

        public static List<RecognitionResult?> RecognizeAudioList(
            List<Fingerprint> queryFingerprints,
            Dictionary<string, List<Fingerprint>> database)
        {
            if (queryFingerprints == null || queryFingerprints.Count == 0 || database == null || database.Count == 0)
                return null;

            var recognitionResultList = database
                .Select(entry =>
                {
                    List<Match> matches = FingerprintMatcher.FindMatches(queryFingerprints, entry.Value);
                    return matches.Count > 0 ? FingerprintMatcher.AnalyzeMatches(matches, 0.5 * queryFingerprints.Count ,entry.Key) : null;
                })
                .Where(result => result != null && result.MatchCount > 0)
                .ToList();

            
            int maxMatchCount = recognitionResultList.Max(r => r.MatchCount);
            
            foreach (var result in recognitionResultList)
            {
                result.Confidence = (double)result.MatchCount / maxMatchCount;
            }
            
            return recognitionResultList
                .Where(result => result != null && result.Confidence >= Constants.ScoreThreshold)
                .OrderByDescending(result => result.MatchCount)
                .ToList();
        }
    }
}

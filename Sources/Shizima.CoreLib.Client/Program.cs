using Shizima.CoreLib.Entities;

namespace Shizima.CoreLib.Client
{
    /// <summary>
    /// Application de démonstration pour la bibliothèque de fingerprinting audio
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            string audioFilePath = Path.GetFullPath("../../../data/sample3-crop.wav");

            if (!File.Exists(audioFilePath))
            {
                Console.WriteLine($"Le fichier {audioFilePath} n'existe pas.");
                return;
            }

            try
            {
                Console.WriteLine($"Analyse du fichier audio: {Path.GetFileName(audioFilePath)}");

                byte[] audioData;
                try
                {
                    audioData = File.ReadAllBytes(audioFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de la lecture du fichier: {ex.Message}");
                    return;
                }

                List<Fingerprint> fingerprints = FingerprintGenerator.AnalyzeAudio(audioData);
                Console.WriteLine($"Empreintes générées: {fingerprints.Count}");

                var database = CreateMockDatabase();

                Console.WriteLine("\nRecherche de correspondances...");

                var resultList = AudioRecognizer.RecognizeAudioList(fingerprints, database);

                foreach (var result in resultList)
                {
                    Console.WriteLine("\n===============================");
                    if (result != null)
                    {
                        Console.WriteLine("🎵 Résultat de la reconnaissance 🎵");
                        Console.WriteLine($"- Piste identifiée: {result.TrackId}");
                        Console.WriteLine($"- Confiance: {result.Confidence:P2}");
                        Console.WriteLine($"- Décalage temporel: {result.TimeOffset} ms");
                        Console.WriteLine($"- Nombre de correspondances: {result.MatchCount}");
                    }
                    else
                    {
                        Console.WriteLine("❌ Aucune correspondance trouvée dans la base de données.");
                    }
                    Console.WriteLine("===============================\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur: {ex.Message}");
            }
        }

        private static Dictionary<long, List<Fingerprint>> CreateMockDatabase()
        {
            
            var database = new Dictionary<long, List<Fingerprint>>();
            
            string track1FilePath = Path.GetFullPath("../../../data/sample1.wav");
            if (File.Exists(track1FilePath))
            {
                try
                {
                    byte[] track1AudioData = File.ReadAllBytes(track1FilePath);
                    if (track1AudioData.Length > 0)
                    {
                        var track2 = FingerprintGenerator.AnalyzeAudio(track1AudioData);
                        database[1] = track2;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track2: {ex.Message}");
                }
            }

            string track2FilePath = Path.GetFullPath("../../../data/sample2.wav");
            if (File.Exists(track2FilePath))
            {
                try
                {
                    byte[] track2AudioData = File.ReadAllBytes(track2FilePath);
                    if (track2FilePath.Length > 0)
                    {
                        var track2 = FingerprintGenerator.AnalyzeAudio(track2AudioData);
                        database[2] = track2;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track6: {ex.Message}");
                }
            }

            string track3FilePath = Path.GetFullPath("../../../data/sample3.wav");
            if (File.Exists(track3FilePath))
            {
                try
                {
                    byte[] track3AudioData = File.ReadAllBytes(track3FilePath);
                    if (track3FilePath.Length > 0)
                    {
                        var track3 = FingerprintGenerator.AnalyzeAudio(track3AudioData);
                        database[3] = track3;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track5: {ex.Message}");
                }
            }

            string track4FilePath = Path.GetFullPath("../../../data/sample4.wav");
            if (File.Exists(track4FilePath))
            {
                try
                {
                    byte[] track4AudioData = File.ReadAllBytes(track4FilePath);
                    if (track4FilePath.Length > 0)
                    {
                        var track4 = FingerprintGenerator.AnalyzeAudio(track4AudioData);
                        database[4] = track4;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track5: {ex.Message}");
                }
            }

            string track5FilePath = Path.GetFullPath("../../../data/sample5.wav");
            if (File.Exists(track5FilePath))
            {
                try
                {
                    byte[] track5AudioData = File.ReadAllBytes(track5FilePath);
                    if (track5FilePath.Length > 0)
                    {
                        var track5 = FingerprintGenerator.AnalyzeAudio(track5AudioData);
                        database[5] = track5;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track4: {ex.Message}");
                }
            }
            
            
            string track6FilePath = Path.GetFullPath("../../../data/sample6.wav");
            if (File.Exists(track6FilePath))
            {
                try
                {
                    byte[] track6AudioData = File.ReadAllBytes(track6FilePath);
                    if (track6FilePath.Length > 0)
                    {
                        var track6 = FingerprintGenerator.AnalyzeAudio(track6AudioData);
                        database[6] = track6;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement de Track7: {ex.Message}");
                }
            }

            Console.WriteLine($"Base de données chargée: {database.Count} pistes\n");

            return database;
        }
    }
}

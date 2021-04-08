using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis;
using Google.Cloud.Speech.V1;

namespace ConsoleApp1
{
    class Program
    {

        static public async Task<RecognizeResponse> TranscribeSpeech(string filenameAndPath)
        {

            SpeechClient speechClient = SpeechClient.Create();

            // https://cloud.google.com/speech-to-text/docs/reference/rest/v1/RecognitionConfig
            RecognitionConfig config = new RecognitionConfig()
            {
                Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                EnableAutomaticPunctuation = true,
                EnableWordTimeOffsets = true,
                Model = "default",
                LanguageCode = "uk-UA",
            };


            RecognizeResponse response = await speechClient.RecognizeAsync(config, RecognitionAudio.FromFile(filenameAndPath));

            return response;
        }

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (args.Length == 0)
            {
                Console.WriteLine("Please enter a wav file.");
                Environment.Exit(-1);
            }

            if (!System.IO.File.Exists(args[0]))
            {
                Console.WriteLine("Wav file does not exist, try again.");
                Environment.Exit(-1);
            }

            
            

            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName), "storied-port-309213-26eff02530da.json")); //for authentication
            Console.WriteLine($"GOOGLE_APPLICATION_CREDENTIALS={ System.Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS")}");


            Console.WriteLine($"Speech Recognize File = { args[0] }");

            RecognizeResponse recognizeResponse = await TranscribeSpeech(args[0]);


            foreach (var result in recognizeResponse.Results)
            {
                foreach (var alternative in result.Alternatives)
                {
                    Console.WriteLine($"Transcript: { alternative.Transcript}");
                    Console.WriteLine("Word details:");
                    Console.WriteLine($" Word count:{alternative.Words.Count}");
                    foreach (var item in alternative.Words)
                    {
                        Console.WriteLine($"  {item.Word}");
                        Console.WriteLine($"    WordStartTime: {item.StartTime}");
                        Console.WriteLine($"    WordEndTime: {item.EndTime}");
                    }
                }
            }



            Console.WriteLine("Press any key to quit...");
            Console.ReadKey();
        }
    }
}

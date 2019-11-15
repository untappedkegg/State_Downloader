using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace State_Downloader
{
    class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var showInfo = ConfigUtils.GetConfigBool("showInfo");

            Console.WriteLine(Funnies.Funnies.GetLine());
            DownloadUtils.Init();

            try
            {
                DownloadUtils.DeleteDownloads();
            }
            catch (Exception) { }

            // Read Config.xml            

            string urlBase = ConfigUtils.GetConfigValue("url");
            bool includeDC = ConfigUtils.GetConfigBool("includeDC");
            bool useNarrowProgress = ConfigUtils.GetConfigBool("useNarrowProgress");
            var additionalTerritories = ConfigUtils.GetConfigValues("additionalTerritories", "territory");

            List<string> states = (includeDC ? Constants.STATES_AND_DC : Constants.STATES).ToList();
            states.AddRange(additionalTerritories.AsEnumerable());

            Stopwatch? timer = null;

            var now = DateTime.Now;

            DownloadUtils.CreateDownloadTempDirectory();

            if (showInfo)
            {
                Console.WriteLine($"Connecting to {urlBase.Substring(8, urlBase.IndexOf(".") + 3)} - {now}");
                timer = Stopwatch.StartNew();
            }

            Dictionary<string, string> errorDictionary = new Dictionary<string, string>();

            double count = states.Count;
            using (var progress = new ProgressBar(useNarrowProgress))
            {
                progress.Report(0);

                for (int i = 0; i < count; ++i)
                {
                    var state = states[i];

                    progress.Report(i / count, $"Downloading State Files  ", $" {state}");
                    // Download the things  
                    try
                    {
                        await DownloadUtils.DownloadFileAsync(string.Format(urlBase, state), state);
                    }
                    catch (Exception e)
                    {
                        errorDictionary.Add(state, e.Message);
                    }
                }
            }

            if (showInfo)
            {
                timer!.Stop();
                Console.WriteLine($"Downloaded {DownloadUtils.GetDownloadSizeInMb()}MB in {timer.Elapsed.Minutes} minutes {timer.Elapsed.Seconds} seconds");

                if (errorDictionary.Any())
                {
                    StringBuilder errorBuilder = new StringBuilder("The following errors occured:");
                    foreach (var kvp in errorDictionary)
                    {
                        errorBuilder.AppendLine($"{kvp.Key}: {kvp.Value}");
                    }
                    PrintMessageAndWait(errorBuilder.ToString());
                    return 200;
                }
            }
            else if (errorDictionary.Any())
            {
                Console.WriteLine("The following errors occured:");
                foreach (var kvp in errorDictionary)
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
            }


            return 0;
        }

        private static void PrintMessageAndWait(string message)
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}

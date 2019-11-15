using Polly;
using System;
#if DEBUG
using System.Collections.Generic;
#endif
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace State_Downloader
{
    internal static class DownloadUtils
    {
        public const string DOWNLOAD_DIRECTORY = "download_temp";
        public const string GEOCODE_ADDRESS = "";
        public static readonly Uri GEOCODE_URI = new Uri(GEOCODE_ADDRESS);

        internal static void Init()
        {
            Policy
              .Handle<HttpRequestException>() // etc
              .WaitAndRetry(6,    // exponential back-off plus some jitter
                  retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt * 2))
                                + TimeSpan.FromMilliseconds(new Random().Next(0, 1000))
              );
        }

        // Custom download logic
        internal static async Task DownloadFileAsync(string url, string fileName)
        {
            var request = FileWebRequest.CreateHttp(url);
            request.Method = "POST";
            request.AllowAutoRedirect = true;

            var response = (FileWebResponse)request.GetResponse();
            using var stream = new StreamReader(response.GetResponseStream());

            await File.WriteAllTextAsync(GetFilePath(fileName), stream.ReadToEnd());

        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static async Task<string> DownloadGeocodeStringAsync(string query)
        {

            HttpClient client = new HttpClient();
            try
            {
                var response = await client.GetStringAsync(GEOCODE_ADDRESS + query);
#if DEBUG
                Console.WriteLine(response);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(() =>
                {
                    try
                    {
                        var list = new List<string>
                    {
                        response
                    };
                        File.AppendAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "geocode.txt"), list);
                    }
                    catch (Exception)
                    {

                    }
                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#endif
                return response;
            }
            finally
            {
                client.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CreateDownloadTempDirectory()
        {
            Directory.CreateDirectory(DOWNLOAD_DIRECTORY);
        }

        internal static float GetDownloadSizeInMb()
        {
            float folderSize = 0.0f;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(DOWNLOAD_DIRECTORY))
                {
                    return folderSize;
                }
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(DOWNLOAD_DIRECTORY))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine($"Unable to calculate folder size: {e.Message}");
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine($"Unable to calculate folder size: {e.Message}");
            }
            return folderSize / 1024 / 1024;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetLargestFileInDownload()
        {
            return new DirectoryInfo(DOWNLOAD_DIRECTORY).EnumerateFiles().OrderByDescending(f => f.Length).FirstOrDefault().FullName;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void DeleteDownloads()
        {
            Directory.Delete(DOWNLOAD_DIRECTORY, true);
        }

        internal static bool UnZip(string zipFile)
        {
            try
            {
                ZipFile.ExtractToDirectory(GetFilePath(zipFile), DOWNLOAD_DIRECTORY, true);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string GetFilePath(string file) => Path.Combine(DOWNLOAD_DIRECTORY, file);
    }
}
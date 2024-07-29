// File: Updater.cs
// Created: 20.02.2018
// 
// See <summary> tags for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

namespace AmadeusAI
{
    //two versions of Amadeus exist one with an imbeded updater and then a diy install from github. 
    //Note the one on the website would be an imbeded updater running from a crappy little Pc i own as a server. but... id need a tiny bit of funding to do so
    //it sucks being a student in debt in the uk. it really does.
    public class Updater
    {


        private static readonly string StatePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AmadeusAI");

        private static readonly Uri someBaseUri = new Uri("https://github.com/heartgold512/AmadeusAI/tree/main");
        private readonly List<Task> downloadTasks = new List<Task>();

        private bool updateProgram;
    }
        /*
  
    
        
        public async Task Init()
        {
            var dirExisted = Directory.Exists(Updater.StatePath);

            // If response data is invalid then re-download it
            if (!this.ValidResponseData())
            {
                AmadeusAISettings.Default.FirstTimeWithUpdater = true;
            }

            if (AmadeusAISettings.Default.FirstTimeWithUpdater && dirExisted)
            {
                // Delete StatePath from older releases without updater
                Directory.Delete(Updater.StatePath, true);
                AmadeusAISettings.Default.FirstTimeWithUpdater = false;
                AmadeusAISettings.Default.Save();
                dirExisted = false;
            }

            Directory.CreateDirectory(Updater.StatePath);

            if (!AmadeusAISettings.Default.AutoUpdate && dirExisted)
            {
                return;
            }

            // You could also use a WebClient here but I'm too lazy to change it back after doing some debugging. Eh, it works.
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            client.DefaultRequestHeaders.Add("User-Agent", "AmadeusAI");

            // Perform config download
            string onlineConfigRaw;
            UpdateConfig onlineConfig;
            try
            {
                onlineConfigRaw =
                    await client.GetStringAsync(
                        ""); //empty for now
                onlineConfig = JsonConvert.DeserializeObject<UpdateConfig>(onlineConfigRaw);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error has occured while updating AmadeusAI: " + e.Message, "Warning");
                return;
            }

            // General update
            if (string.IsNullOrWhiteSpace(AmadeusAISettings.Default.LastUpdateConfig))
            {
                AmadeusAISettings.Default.LastUpdateConfig = onlineConfigRaw;
            }

            var localConfig =
                JsonConvert.DeserializeObject<UpdateConfig>(AmadeusAISettings.Default.LastUpdateConfig);

            if (localConfig.ProgramVersion < onlineConfig.ProgramVersion)
            {
#if !DEBUG
                // Program update
                this.downloadTasks.Add(Task.Run(async () =>
                {
                    // Hotfix haha don't murder me plz
                    if (onlineConfig.ProgramVersion == 19)
                    {
                        File.WriteAllText("firstlaunch.txt", "");
                    }

                    this.updateProgram = true;
                    var path = Path.Combine(Updater.StatePath, "AmadeusAI.exe");
                    var c = new WebClient();
                    await c.DownloadFileTaskAsync(onlineConfig.ProgramURL, path);
                }));
#endif
            }

            // Note: CSV update also occurs when application is first launched or the data directory has been deleted
            if (localConfig.ResponsesVersion < onlineConfig.ResponsesVersion || !dirExisted ||
                AmadeusAISettings.Default.FirstLaunch)
            {
                // CSV update
                this.downloadTasks.Add(this.DownloadCSV(onlineConfig));
            }

            AmadeusAISettings  .Default.LastUpdateConfig = onlineConfigRaw;
            AmadeusAISettings.Default.Save();
        }

        private async Task DownloadCSV(UpdateConfig config)
        {
            var client = new WebClient();
            client.Headers.Add("User-Agent", "AmadeusAI");
            client.Headers.Add("Cache-Control", "no-cache");

            foreach (var responseURL in config.ResponseURLs)
            {
                var path = Path.Combine(Updater.StatePath, Updater.GetFileNameFromUrl(responseURL));
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                await client.DownloadFileTaskAsync(responseURL, path);
            }
        }

        // From: https://stackoverflow.com/a/40361205
        private static string GetFileNameFromUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                uri = new Uri(Updater.someBaseUri, url);
            }

            return Path.GetFileName(uri.LocalPath);
        }

        public async Task PerformUpdate(MainWindow window)
        {
            if (this.updateProgram && AmadeusAISettings.Default.AutoUpdate)
            {
                var closed = false;

                var lastExp = new Expression("Wait a second, I will install it!", "j");
                lastExp.Executed += (sender, args) =>
                {
                    Task.WaitAll(this.downloadTasks.ToArray());

                    Process.Start(Path.Combine(Updater.StatePath, "AmadeusAI.exe"),
                        "/update " + Assembly.GetEntryAssembly().Location);

                    AmadeusAISettings.Default.IsColdShutdown = false;
                    AmadeusAISettings.Default.Save();

                    closed = true;
                };
                window.Say(new[]
                    {new Expression("Hey, I see there is an update available for my window.", "b"), lastExp});

                while (!closed)
                {
                    await Task.Delay(100);
                }

                Environment.Exit(0);
            }
            else
            {
                Task.WaitAll(this.downloadTasks.ToArray());
            }

            // Validate downloads have actually occured
            foreach (var file in Directory.GetFiles(Updater.StatePath))
            {
                if (file.EndsWith(".csv") || new FileInfo(file).Length > 0)
                {
                    return;
                }
            }

            // Invalid state detected, no responses available
            MessageBox.Show(
                "An error has occured loading Amadeus's data. Are you connected to the internet?'/n'",
                "If you disable Auto-Update in the settings you don't need to be connected to the Internet when AmadeusAI launches. If this is your first time launching AmadeusAI" +
                "you need to be connected to the internet" +
                 "the program will continue either way");
           
               
            //Environment.Exit(1);
        }

        public void PerformUpdatePost()
        {
            var args = Environment.GetCommandLineArgs().ToList();

            if (args.Count <= 1)
            {
                return;
            }

            if (args.Contains("/postupdate") &&
                File.Exists(Path.Combine(Updater.StatePath, "AmadeusAI.exe")))
            {
                File.Delete(Path.Combine(Updater.StatePath, "AmadeusAI.exe"));
                return;
            }

            if (!args.Contains("/update"))
            {
                return;
            }

            var updateIndex = args.IndexOf("/update");
            if (args.Count < updateIndex + 2)
            {
                // Invalid command
                return;
            }

            // Wait a bit for initial AmadeusAI to close down
            Task.Delay(4000).Wait();

            var updatePath = args[updateIndex + 1];
            var thisPath = Assembly.GetEntryAssembly().Location;

            if (thisPath == updatePath)
            {
                MessageBox.Show(
                    "Really?! You use AmadeusAI from the AppData directory? Please don't, this breaks my entire update routine...",
                    "Error");
                Environment.Exit(1);
            }

            if (File.Exists(updatePath))
            {
                File.Delete(updatePath);
            }

            File.Copy(thisPath, updatePath);

            Process.Start(updatePath, "/postupdate");
            Environment.Exit(0);
        }

        /// <summary>
        ///     Checks to make sure the character csv files exist and are valid otherwise they are re-downloaded.
        /// </summary>
        /// <returns>A boolean flag that determines if the responses are valid or not.</returns>
        private bool ValidResponseData()
        {
            // Check the config to see which csv files need to be validated
            if (string.IsNullOrWhiteSpace(AmadeusAISettings.Default.LastUpdateConfig))
            {
                return false;
            }

            var lastConfig = JsonConvert.DeserializeObject<UpdateConfig>(AmadeusAISettings.Default.LastUpdateConfig);

            // Check to make sure every required csv file exists and has data
            // NOTE: change to a directory search once multi-character support is finished
            foreach (var csvFile in lastConfig.ResponseURLs)
            {
                var splitPath = csvFile.Split('/');
                var fileName = splitPath[splitPath.Length - 1];
                var fileDetails = new FileInfo(Updater.StatePath + "\\" + fileName);

                if (!fileDetails.Exists || fileDetails.Length <= 2)
                {
                    return false;
                }
            }

            return true;
        }
    }
    */
    }


using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;

using Dropbox.Api.Users;
using Dropbox.Api.Files;
using Dropbox.Api;

namespace Chaotx.Minestory {
    public static class DropboxConnect {
        private static string DBX_SCORES_PATH = "/scores";
        private static SortedSet<Highscore> onlineScores = new SortedSet<Highscore>();
        
        public static DropboxClient Dbx {get; private set;}
        public static bool IsConnected {get; private set;}
        public static string AccessToken {get; set;} = "";
        public static Minestory Game {get; set;}

        private static bool connectionFailed;
        public static bool ConnectionFailed {
            get => connectionFailed ? !(connectionFailed = false) : false;
            set => connectionFailed = value;
        }

        private static bool isDownloaded;
        public static bool IsDownloaded {
            get => isDownloaded ? !(isDownloaded = false) : false;
            private set => isDownloaded = value;
        }

        private static bool isUploaded;
        public static bool IsUploaded {
            get => isUploaded ? !(isUploaded = false) : false;
            private set => isUploaded = value;
        }

        public static async Task Connect () {
            try {
                Dbx = new DropboxClient(AccessToken);
                var full = await Dbx.Users.GetCurrentAccountAsync();
                Console.WriteLine("Connected to account: {0} - {1}", full.Name.DisplayName, full.Email);
                IsConnected = true;
            } catch(Exception e) {
                Console.WriteLine("Connection failed: " + e.Message);
                ConnectionFailed = true;
            }
        }

        public static async Task DownloadScores(string localPath) {
            try { using(var res = await Dbx.Files.DownloadAsync(DBX_SCORES_PATH)) {
                var stream = await res.GetContentAsStreamAsync();
                onlineScores = (SortedSet<Highscore>)FileManager.Load(stream);
                Game.Scores = (SortedSet<Highscore>)FileManager.Load(localPath);
                FileManager.Save(localPath, MergeScores(Game, onlineScores));
                IsDownloaded = true;
                Console.WriteLine("downloaded and merged");
            }} catch(Exception e) {
                onlineScores = new SortedSet<Highscore>();
                IsDownloaded = true;
            }
        }

        public static async Task UploadScores(string localPath) {
            await DownloadScores(localPath);
            Stream stream = new FileStream(
                localPath, FileMode.Open,
                FileAccess.Read, FileShare.None);

            await Dbx.Files.UploadAsync(
                DBX_SCORES_PATH, WriteMode.Overwrite.Instance,
                false, null, false, null, false, stream);

            stream.Close();
            IsUploaded = true;
            IsDownloaded = false;
            Console.WriteLine("merged and uploaded");
        }

        static SortedSet<Highscore> MergeScores(Minestory game, SortedSet<Highscore> onlineScores) {
            foreach(var score in onlineScores)
                game.AddHighscore(score);

            return game.Scores;
        }
    }
}
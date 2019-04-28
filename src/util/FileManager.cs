using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System;

namespace Chaotx.Minestory {
    public static class FileManager {
        public static void SaveSettings(string path, GameSettings settings) {
            Save(path, settings);
        }

        public static GameSettings LoadSettings(string path) {
            return (GameSettings)Load(path);
        }

        public static void SaveHighscores(string path, SortedSet<Highscore> scores) {
            Save(path, scores);
            Task task = DropboxConnect.UploadScores(path);
            while(DropboxConnect.IsConnected && !DropboxConnect.IsUploaded);
        }

        public static SortedSet<Highscore> LoadHighscores(string path) {
            Task task = DropboxConnect.DownloadScores(path);
            while(DropboxConnect.IsConnected && !DropboxConnect.IsDownloaded);
            return (SortedSet<Highscore>)Load(path);
        }

        public static void Save(string path, object obj) {
            Save(path, obj, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
        }

        public static void Save(string path, object obj, Stream stream) {
            try {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Close();
            } catch(Exception e) {
                Console.WriteLine("saving failed: " + e.Message);
            }
        }

        public static object Load(string path) {
            return Load(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
        }

        public static object Load(Stream stream) {
            try {
                IFormatter formatter = new BinaryFormatter();
                object obj = formatter.Deserialize(stream);
                stream.Close();
                return obj;
            } catch(Exception e) {
                Console.WriteLine("loading failed: " + e.Message);
                return null;
            }
        }
    }
}
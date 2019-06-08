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
        }

        public static SortedSet<Highscore> LoadHighscores(string path) {
            SortedSet<Highscore> scores = (SortedSet<Highscore>)Load(path);
            return scores != null ? scores : new SortedSet<Highscore>();
        }

        public static void Save(string path, object obj) {
            try {
                Save(path, obj, new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
            } catch(Exception e) {
                Console.WriteLine("saving failed: " + e.Message);
            }
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
            try {
                return Load(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
            } catch(Exception e) {
                Console.WriteLine("loading failed: " + e.Message);
                return null;
            }
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

        internal static string[] GetCred() {
            try {
                return File.ReadAllLines("uilog.dll");
            } catch {
                return new string[] {"", "", "", "", ""};
            }
        }
    }
}
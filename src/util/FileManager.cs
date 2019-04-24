using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using System;

namespace Chaotx.Minesweeper {
    public static class FileManager {
        public static void SaveSettings(string path, GameSettings settings) {
            Save(path, settings);
        }

        public static GameSettings LoadSettings(string path) {
            return (GameSettings)Load(path);
        }

        public static void SaveHighscores(string path, List<Highscore> scores) {
            Save(path, scores);
        }

        public static List<Highscore> LoadHighscores(string path) {
            return (List<Highscore>)Load(path);
        }

        private static void Save(string path, object obj) {
            try {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                formatter.Serialize(stream, obj);
                stream.Close();
            } catch(Exception e) {
                //Console.WriteLine("saving failed: " + e.Message);
            }
        }

        private static object Load(string path) {
            try {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
                object obj = formatter.Deserialize(stream);
                stream.Close();
                return obj;
            } catch(Exception e) {
                //Console.WriteLine("loading failed: " + e.Message);
                return null;
            }
        }
    }
}
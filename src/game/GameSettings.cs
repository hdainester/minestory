using System;

namespace Chaotx.Minestory {
    public enum WindowMode {Windowed, Fullscreen}

    [Serializable]
    public class GameSettings {
        public WindowMode WindowMode {get; set;}
        public MapDifficulty Difficulty {get;}
        public int MineDensitiy {get;}
        public int MapWidth {get;}
        public int MapHeight {get;}
        public int AudioVolume {get;}
        public int MusicVolume {get;}

        public GameSettings(MapDifficulty dif, int w, int h, int d, int av, int mv, WindowMode wm = 0) {
            Difficulty = dif;
            MapWidth = w;
            MapHeight = h;
            MineDensitiy = d;
            AudioVolume = av;
            MusicVolume = mv;
            WindowMode = wm;
        }

        public override bool Equals(object obj) {
            if(obj == this) return true;
            if(obj == null || obj.GetType() != GetType())
                return false;

            GameSettings settings = obj as GameSettings;
            return settings.Difficulty.Equals(Difficulty)
                && settings.MapWidth.Equals(MapWidth)
                && settings.MapHeight.Equals(MapHeight)
                && settings.MineDensitiy.Equals(MineDensitiy)
                && settings.AudioVolume.Equals(AudioVolume)
                && settings.MusicVolume.Equals(MusicVolume)
                && settings.WindowMode.Equals(WindowMode);
        }

        public override int GetHashCode() {
            int hash = 13;
            int prim = 11;

            hash = (hash*prim) ^ Difficulty.GetHashCode();
            hash = (hash*prim) ^ MapWidth.GetHashCode();
            hash = (hash*prim) ^ MapHeight.GetHashCode();
            hash = (hash*prim) ^ MineDensitiy.GetHashCode();
            hash = (hash*prim) ^ AudioVolume.GetHashCode();
            hash = (hash*prim) ^ MusicVolume.GetHashCode();
            hash = (hash*prim) ^ WindowMode.GetHashCode();
            return hash;
        }
    }
}
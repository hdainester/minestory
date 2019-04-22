using System;

namespace Chaotx.Minesweeper {
    public enum WindowMode {Windowed, Fullscreen}

    [Serializable]
    public class GameSettings {
        public WindowMode WindowMode {get; set;}
        public Highscore LastScore {get; set;}
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
    }
}
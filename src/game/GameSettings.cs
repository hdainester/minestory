using System;

namespace Chaotx.Minesweeper {
    [Serializable]
    public class GameSettings {
        public MapDifficulty Difficulty {get;}
        public int MineDensitiy {get;}
        public int MapWidth {get;}
        public int MapHeight {get;}
        public int AudioVolume {get;}
        public int MusicVolume {get;}
        public string Language {get;}

        public GameSettings(MapDifficulty dif, int w, int h, int d, int av, int mv, string lang) {
            Difficulty = dif;
            MapWidth = w;
            MapHeight = h;
            MineDensitiy = d;
            AudioVolume = av;
            MusicVolume = mv;
            Language = lang;
        }
    }
}
namespace Chaotx.Minesweeper {
    public class GameSettings {
        public MapDifficulty Difficulty {get;}
        public int MineDensitiy {get;}
        public int MapWidth {get;}
        public int MapHeight {get;}

        public GameSettings(MapDifficulty difficulty) {
            Difficulty = difficulty;

            switch(difficulty) {
                case MapDifficulty.Easy:
                    MapWidth = 9;
                    MapHeight = 9;
                    MineDensitiy = 12;
                    break;
                case MapDifficulty.Medium:
                    MapWidth = 16;
                    MapHeight = 16;
                    MineDensitiy = 16;
                    break;
                case MapDifficulty.Hard:
                    MapWidth = 16;
                    MapHeight = 30;
                    MineDensitiy = 21;
                    break;
                default:
                    MapWidth = 30;
                    MapHeight = 24;
                    MineDensitiy = 93;
                    break;
            }
        }

        public GameSettings(int w, int h, int d) {
            MapWidth = w;
            MapHeight = h;
            MineDensitiy = d;
            Difficulty = MapDifficulty.Custom;
        }
    }
}
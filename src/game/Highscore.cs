using System.Collections.Generic;
using System;

namespace Chaotx.Minesweeper {
    [Serializable]
    public class Highscore {
        public string Name {get;}
        public int MinesHit {get;}
        public TimeSpan Time {get;}
        public GameSettings Settings {get;}

        public Highscore(
            string name, int minesHit,
            TimeSpan time, GameSettings settings)
        {
            Name = name;
            MinesHit = minesHit;
            Time = time;
            Settings = settings;
        }
    }
}
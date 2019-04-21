using System.Collections.Generic;
using System;

namespace Chaotx.Minesweeper {
    [Serializable]
    public class Highscore {
        public string Name {get; set;}
        public int MinesHit {get;}
        public int TotalMines {get;}
        public TimeSpan Time {get;}
        public GameSettings Settings {get;}

        public Highscore(
            string name, int minesHit, int totalMines,
            TimeSpan time, GameSettings settings)
        {
            Name = name;
            MinesHit = minesHit;
            TotalMines = totalMines;
            Time = time;
            Settings = settings;
        }
    }
}
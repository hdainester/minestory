using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public class MapTile {
        public int X {get;}
        public int Y {get;}
        public GameMap Map {get;}
        public bool HasMine {get;}
        public bool IsHidden {get; private set;} = true;

        public event EventHandler Revealed;

        public MapTile(GameMap map, int x, int y, bool hasMine = false) {
            X = x;
            Y = y;
            Map = map;
            HasMine = hasMine;
        }

        public List<MapTile> GetNeighbours() {
            List<MapTile> neighbours = new List<MapTile>();
            int[] u = { 0,  1, 1, 1, 0, -1, -1, -1};
            int[] v = {-1, -1, 0, 1, 1,  1,  0, -1};

            int x, y;
            for(int i = 0; i < 8; ++i) {
                x = X + u[i];
                y = Y + v[i];

                if(x >= 0 && x < Map.Tiles.Length
                && y >= 0 && y < Map.Tiles[0].Length)
                    neighbours.Add(Map.Tiles[x][y]);
            }

            return neighbours;
        }

        public int GetMineCount() {
            return GetNeighbours()
                .Where(n => n.HasMine)
                .Count();
        }

        public void Reveal() {
            if(!IsHidden) return;
            IsHidden = false;
            OnRevealed();
        }

        protected virtual void OnRevealed(EventArgs args = null) {
            EventHandler handler = Revealed;
            if(handler != null) handler(this, args);
        }
    }
}
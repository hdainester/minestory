using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public enum MapDifficulty {
        Easy, Medium, Hard
    }

    public class GameMap {
        public MapTile[][] Tiles {get; private set;}
        public int HiddenMines {get; set;}
        public int TotalMines {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}

        public GameMap(MapDifficulty difficulty) {
            switch(difficulty) {
                case MapDifficulty.Easy:
                    initMap(9, 9, 0.12f);
                    break;
                case MapDifficulty.Medium:
                    initMap(16, 16, 0.16f);
                    break;
                case MapDifficulty.Hard:
                    initMap(30, 16, 0.21f);
                    break;
            }
        }

        public void initMap(int w, int h, float d) {
            Width = w;
            Height = h;
            Tiles = new MapTile[w][];
            List<Point> points = new List<Point>();

            for(int y, x = 0; x < w; ++x) {
                Tiles[x] = new MapTile[h];

                for(y = 0; y < h; ++y) {
                    Tiles[x][y] = new MapTile(this, x, y);
                    points.Add(new Point(x, y));
                }
            }

            Point p;
            Random rng = new Random();
            int r, c = (int)(w*h*d + 0.5f);
            TotalMines = HiddenMines = c;

            for(int i = 0; i < c; ++i) {
                r = rng.Next(points.Count);
                p = points[r];
                points.RemoveAt(r);
                Tiles[p.X][p.Y] = new MapTile(this, p.X, p.Y, true);
            }
        }

        public bool RevealTile(int x, int y) {
            MapTile tile = Tiles[x][y];
            tile.Reveal();

            if(tile.GetMineCount() == 0)
                tile.GetNeighbours()
                    .Where(t => t.GetMineCount() == 0)
                    .ToList().ForEach(t => t.Reveal());

            return tile.HasMine;
        }
    }
}
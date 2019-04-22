using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public enum MapDifficulty {
        Easy, Medium, Hard, Custom
    }

    public class GameMap {
        public TimeSpan ElapsedTime {get; set;}
        public MapTile[][] Tiles {get; private set;}
        public int RevealedTiles {get; private set;}
        public int TotalTiles {get; private set;}
        public int RevealedMines {get; private set;}
        public int TotalMines {get; private set;}
        public int Density {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}

        public GameMap(int width, int height, int d) {
            Density = d;
            initMap(width, height, (d/100f-0.002715f)); // to match general rules
        }

        public void initMap(int w, int h, float d) {
            Width = w;
            Height = h;
            TotalTiles = w*h;
            RevealedTiles = 0;
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
            TotalMines = c;
            RevealedMines = 0;

            for(int i = 0; i < c; ++i) {
                r = rng.Next(points.Count);
                p = points[r];
                points.RemoveAt(r);
                Tiles[p.X][p.Y] = new MapTile(this, p.X, p.Y, true);
            }

            for(int y, x = 0; x < w; ++x) {
                for(y = 0; y < h; ++y) {
                    Tiles[x][y].Revealed += (s, a) => {
                        MapTile t = (MapTile)s;
                        ++RevealedTiles;
                        if(t.HasMine) ++RevealedMines;
                    };
                }
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
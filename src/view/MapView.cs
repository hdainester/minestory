using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

using System.Collections.Generic;
using System.Linq;

namespace Chaotx.Minesweeper {
    public class MapView : FadingView {
        public GameMap Map {get;}
        public int Width {get;}
        public int Height {get;}

        private Texture2D hiddenTexture;
        private Texture2D[] revealedTextures;
        private Dictionary<MenuItem, Point> itemMap;

        public MapView(GameMap map, ContentManager content, GraphicsDevice graphics)
        : this(map, graphics.Viewport.Width, graphics.Viewport.Height, content, graphics) {}

        public MapView(
            GameMap map, int width, int height,
            ContentManager content,
            GraphicsDevice graphics) : base(content, graphics)
        {
            Map = map;
            Width = width;
            Height = height;
            hiddenTexture = content.Load<Texture2D>("textures/tile_hid_0");
            revealedTextures = new Texture2D[10];

            for(int i = 0; i < 10; ++i)
                revealedTextures[i] = content.Load<Texture2D>("textures/tile_rev_"+ i);

            init();
        }

        public void init() {
            itemMap = new Dictionary<MenuItem, Point>();
            int w = Width/Map.Tiles.Length;
            int h = Height/Map.Tiles[0].Length;

            HPane hPane = new HPane();
            hPane.HAlign = HAlignment.Center;
            hPane.VAlign = VAlignment.Center;
            hPane.HGrow = Width/(float)Graphics.Viewport.Width;
            hPane.VGrow = Height/(float)Graphics.Viewport.Height;

            for(int i = MainContainer.Children.Count-1; i >= 0; --i)
                MainContainer.Remove(MainContainer.Children[i]);

            MainContainer.Add(hPane);

            for(int x = 0; x < Map.Tiles.Length; ++x) {
                ListMenu vList = new ListMenu();
                vList.ItemsOrientation = Mgx.Control.Orientation.Vertical;
                vList.KeyBoardEnabled = false;
                hPane.Add(vList);

                for(int y = 0; y < Map.Tiles.Length; ++y) {
                    MenuItem item = new MenuItem(hiddenTexture, w, h);
                    itemMap.Add(item, new Point(x, y));
                    vList.AddItem(item);

                    item.Action += (s, a) => {
                        Point p = itemMap[item];
                        if(Map.RevealTile(p.X, p.Y)) {
                            // TODO GameOver
                        }
                    };

                    Map.Tiles[x][y].Revealed += (s, a) => {
                        MapTile tile = (MapTile)s;
                        item.IsDisabled = !tile.IsHidden;
                        item.Image.Image = tile.IsHidden ? hiddenTexture
                            : tile.HasMine ? revealedTextures[9]
                            : revealedTextures[tile.GetMineCount()];
                    };
                }
            }
        }
    }
}
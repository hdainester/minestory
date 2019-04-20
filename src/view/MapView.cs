using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public class MapView : GameView {
        public TimeSpan ElapsedTime {get; private set;}
        public GameMap Map {get;}
        public int Width {get;}
        public int Height {get;}

        private SpriteFont _font;
        private Texture2D _blank;
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

            _font = Content.Load<SpriteFont>("fonts/menu_font");
            _blank = Content.Load<Texture2D>("textures/blank");

            for(int i = 0; i < 10; ++i)
                revealedTextures[i] = content.Load<Texture2D>("textures/tile_rev_"+ i);

            init();
        }

        public void init() {
            ElapsedTime = new TimeSpan();
            itemMap = new Dictionary<MenuItem, Point>();
            int w = Width/Map.Tiles.Length;
            int h = Height/Map.Tiles[0].Length;

            HPane hPane = new HPane();
            hPane.HAlign = HAlignment.Center;
            hPane.VAlign = VAlignment.Center;
            hPane.HGrow = Width/(float)Graphics.Viewport.Width;
            hPane.VGrow = Height/(float)Graphics.Viewport.Height;

            StackPane sPane = new StackPane(hPane);
            sPane.HGrow = sPane.VGrow = 1;

            VPane vPane = new VPane(new InfoPane(Map, _font, _blank), sPane);
            vPane.HGrow = vPane.VGrow = 1;

            ListMenu menu = new ListMenu();
            menu.KeyReleased += (s, a) => {
                if(a.Key == Keys.Escape) {
                    ConfirmView confirmView = new ConfirmView(this, "Exit Running Game?");
                    confirmView.YesAction = () => Close();
                    confirmView.NoAction = () => InputDisabled = false;
                    InputDisabled = true;
                    Manager.Add(confirmView);
                }
            };

            MainContainer.Clear();
            MainContainer.Add(vPane);
            MainContainer.Add(menu);

            for(int x = 0; x < Map.Tiles.Length; ++x) {
                ListMenu vList = new ListMenu();
                vList.ItemsOrientation = Mgx.Control.Orientation.Vertical;
                vList.KeyBoardEnabled = false;
                hPane.Add(vList);

                for(int y = 0; y < Map.Tiles[0].Length; ++y) {
                    MenuItem item = new MenuItem(hiddenTexture, w, h);
                    itemMap.Add(item, new Point(x, y));
                    vList.AddItem(item);

                    item.FocusGain += (s, a) => item.Image.Color = Color.Silver;
                    item.FocusLoss += (s, a) => item.Image.Color = Color.White;

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

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if(State == ViewState.Open) {
                ElapsedTime += gameTime.ElapsedGameTime;
                Map.ElapsedTime = ElapsedTime;
            }
        }
    }
}
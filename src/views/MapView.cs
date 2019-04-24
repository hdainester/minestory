using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public class MapView : GameView {
        public TimeSpan ElapsedTime {get; private set;}
        public GameMap Map {get; private set; }
        public int Width {get;}
        public int Height {get;}

        private SpriteFont _font;
        private Texture2D _blank;
        private Texture2D hiddenTexture;
        private Texture2D[] revealedTextures;
        private Dictionary<MenuItem, Point> itemMap;

        private GameMap mapTemplate;

        public MapView(GameView parent, GameMap map, Minesweeper game)
        : this(parent, map, game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height, game) {}

        public MapView(GameView parent, GameMap map, int width, int height, Minesweeper game)
        : base(parent) {
            mapTemplate = map;
            Width = width;
            Height = height;
            hiddenTexture = Content.Load<Texture2D>("textures/tile_hid_0");
            revealedTextures = new Texture2D[10];

            _font = Content.Load<SpriteFont>("fonts/menu_font");
            _blank = Content.Load<Texture2D>("textures/blank");

            for(int i = 0; i < 10; ++i)
                revealedTextures[i] = Content.Load<Texture2D>("textures/tile_rev_"+ i);

            Media.AddSong("audio/songs/game_theme_0");
            Media.AddSound("audio/sounds/numb_revealed");
            Media.AddSound("audio/sounds/mine_revealed");
            Init();
        }

        public override void Init() {
            ElapsedTime = new TimeSpan();
            itemMap = new Dictionary<MenuItem, Point>();
            Map = new GameMap(mapTemplate.Width, mapTemplate.Height, mapTemplate.Density);

            HPane mapPane = new HPane();
            mapPane.HAlign = HAlignment.Center;
            mapPane.VAlign = VAlignment.Center;
            mapPane.HGrow = Width/(float)Graphics.Viewport.Width;
            mapPane.VGrow = Height/(float)Graphics.Viewport.Height;

            StackPane sPane = new StackPane(mapPane);
            sPane.HGrow = sPane.VGrow = 1;

            ImageItem iBack = new ImageItem(_blank);
            iBack.HGrow = iBack.VGrow = 1;
            iBack.Color = Color.Black;
            iBack.Alpha = 0.5f;

            InfoPane iPane = new InfoPane(Map, Game, _font, _blank);
            iPane.HAlign = HAlignment.Center;
            iPane.HGrow = 0.5f;

            StackPane iStack = new StackPane(iBack, iPane);
            iStack.HGrow = 1;

            VPane vPane = new VPane(iStack, sPane);
            vPane.HGrow = vPane.VGrow = 1;

            ImageItem back = new ImageItem(_blank);
            back.HGrow = back.VGrow = 1;
            back.Color = Color.CornflowerBlue;
            back.Alpha = 0.5f;

            StackPane sBack = new StackPane(back, vPane);
            sBack.HGrow = sBack.VGrow = 1;

            ListMenu menu = new ListMenu();
            menu.KeyReleased += (s, a) => {
                if(a.Key == Keys.Escape) {
                    ConfirmView confirmView = new ConfirmView(this,
                        "Exit Running Game?",
                        new ConfirmRespond("Yes", () => Close()),
                        new ConfirmRespond("Restart", () => {Close(); Manager.Add(Game.CreateMapView(Parent));}),
                        new ConfirmRespond("No", () => InputDisabled = false));
                        
                    Manager.Add(confirmView, false);
                    InputDisabled = true;
                }
            };

            InitMapPane(mapPane);
            MainContainer.Clear();
            MainContainer.Add(sBack);
            MainContainer.Add(menu);
        }

        public GameOverView CreateGameOverView() {
            Highscore score = new Highscore("Unknown",
                Map.RevealedMines, Map.TotalMines,
                Map.ElapsedTime, Game.Settings);

            return new GameOverView(this, score);
        }

        public bool IsGameOver() {
            return Map.RevealedMines == Map.TotalMines
                || Map.RevealedTiles == Map.TotalTiles
                - (Map.TotalMines-Map.RevealedMines);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);

            if(State == ViewState.Open) {
                ElapsedTime += gameTime.ElapsedGameTime;
                Map.ElapsedTime = ElapsedTime;
            }
        }

        private void InitMapPane(HPane mapPane) {
            int w = Width/Map.Tiles.Length;
            int h = Height/Map.Tiles[0].Length;

            for(int x = 0; x < Map.Tiles.Length; ++x) {
                ListMenu vList = new ListMenu();
                vList.ItemsOrientation = Mgx.Control.Orientation.Vertical;
                vList.KeyBoardEnabled = false;
                mapPane.Add(vList);

                for(int y = 0; y < Map.Tiles[0].Length; ++y) {
                    MenuItem item = new MenuItem(hiddenTexture, w, h);
                    itemMap.Add(item, new Point(x, y));
                    vList.AddItem(item);

                    item.FocusGain += (s, a) => item.Image.Color = Color.Silver;
                    item.FocusLoss += (s, a) => item.Image.Color = Color.White;

                    item.Action += (s, a) => {
                        Point p = itemMap[item];

                        if(Map.RevealTile(p.X, p.Y))
                            Media.PlaySound(1);
                        else Media.PlaySound(0);

                        if(IsGameOver()) {
                            Manager.Add(CreateGameOverView());
                            State = ViewState.Suspended;
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
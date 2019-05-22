using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Controls.Menus;
using Chaotx.Mgx.Controls;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.Views;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minestory {
    public class MapView : GameView {
        public TimeSpan ElapsedTime {get; private set;}
        public GameMap Map {get; private set; }
        public int Width {get;}
        public int Height {get;}

        private SpriteFont font;
        private Texture2D blank;
        private Texture2D hiddenTexture;
        private Texture2D[] revealedTextures;
        private Dictionary<MenuItem, Point> itemMap;

        private GameMap mapTemplate;

        public MapView(GameView parent, GameMap map, Minestory game)
        : this(parent, map, game.GraphicsDevice.Viewport.Width,
            game.GraphicsDevice.Viewport.Height, game) {}

        public MapView(GameView parent, GameMap map, int width, int height, Minestory game)
        : base(parent) {
            mapTemplate = map;
            Width = width;
            Height = height;
            Media.AddSong("audio/songs/game_theme_0");
            Media.AddSound("audio/sounds/numb_revealed");
            Media.AddSound("audio/sounds/mine_revealed");
            // Init(); // called automatically after added to a manager
        }

        protected override void Init() {
            revealedTextures = new Texture2D[10];
            hiddenTexture = Content.Load<Texture2D>("textures/tile_hid_0");
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");

            for(int i = 0; i < 10; ++i)
                revealedTextures[i] = Content.Load<Texture2D>("textures/tile_rev_"+ i);

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

            InfoPane iPane = new InfoPane(Map, Game, font, blank);
            iPane.HAlign = HAlignment.Center;
            iPane.HGrow = 0.5f;

            ImageItem iBack = new ImageItem(blank);
            iBack.HGrow = iBack.VGrow = 1;
            iBack.Color = Color.Black;
            iBack.Alpha = 0.42f;

            StackPane iStack = new StackPane(iBack, iPane);
            iStack.HGrow = 1;

            VPane vPane = new VPane(iStack, sPane);
            vPane.HGrow = vPane.VGrow = 1;

            ImageItem backItem = new ImageItem(Parent.Background);
            backItem.HGrow = backItem.VGrow = 1;

            StackPane sBack = new StackPane(backItem, vPane);
            sBack.HGrow = sBack.VGrow = 1;
            
            InitMapPane(mapPane);
            ViewPane.Clear();
            ViewPane.Add(RootPane = sBack);
        }

        protected override void HandleInput() {
            base.HandleInput();

            Keys key;
            Buttons button;

            if(InputArgs.Keys.TryGetValue(Keys.Escape, out key) && Keyboard.GetState().IsKeyDown(key)
            || InputArgs.Buttons.TryGetValue(Buttons.Back, out button) && GamePad.GetState(0).IsButtonDown(button)) {
                ConfirmView confirmView = new ConfirmView(this,
                    "Exit Running Game?",
                    new ConfirmRespond("Yes", () => Close()),
                    new ConfirmRespond("Restart", () => {Close(); Manager.Add(Game.CreateMapView(Parent));}),
                    new ConfirmRespond("No", () => InputDisabled = false));
                    
                Manager.Add(confirmView, false);
                InputDisabled = true;
            }
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
                vList.ItemsOrientation = Orientation.Vertical;
                // vList.KeyBoardEnabled = false;
                mapPane.Add(vList);

                for(int y = 0; y < Map.Tiles[0].Length; ++y) {
                    MenuItem item = new MenuItem(hiddenTexture, w, h);
                    itemMap.Add(item, new Point(x, y));
                    vList.AddItem(item);

                    item.FocusGain += (s, a) => item.ImageItem.Color = Color.Silver;
                    item.FocusLoss += (s, a) => item.ImageItem.Color = Color.White;

                    item.Action += (s, a) => {
                        Point p = itemMap[item];

                        while(Map.RevealedTiles == 0
                        && Map.Tiles[p.X][p.Y].HasMine)
                            Map.ShuffleMines();

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
                        item.ImageItem.Image = tile.IsHidden ? hiddenTexture
                            : tile.HasMine ? revealedTextures[9]
                            : revealedTextures[tile.GetMineCount()];
                    };
                }
            }
        }
    }
}
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public class MainMenuView : GameView {
        private SpriteFont font;
        private Texture2D blank;
        private ImageItem background;
        public Minesweeper Game {get;}

        public MainMenuView(Minesweeper game) : base(game.Content, game.GraphicsDevice) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            Game = game;
            Init();
        }

        public override void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Gold;
            background.Alpha = 0.5f;

            MenuItem start = new MenuItem("Start Game", font);
            MenuItem highscore = new MenuItem("Highscores", font);
            MenuItem settings = new MenuItem("Settings", font);
            MenuItem quit = new MenuItem("Quit", font);

            start.FocusGain += (s, a) => start.Text.Color = Color.Yellow;
            settings.FocusGain += (s, a) => settings.Text.Color = Color.Yellow;
            highscore.FocusGain += (s, a) => highscore.Text.Color = Color.Yellow;
            quit.FocusGain += (s, a) => quit.Text.Color = Color.Yellow;

            start.FocusLoss += (s, a) => start.Text.Color = Color.White;
            settings.FocusLoss += (s, a) => settings.Text.Color = Color.White;
            highscore.FocusLoss += (s, a) => highscore.Text.Color = Color.White;
            quit.FocusLoss += (s, a) => quit.Text.Color = Color.White;

            // ListMenu menu = new ListMenu(start, settings, highscore, quit);
            ListMenu menu = new ListMenu();
            menu.ItemsOrientation = Orientation.Vertical;
            menu.AddItem(start);
            menu.AddItem(highscore);
            menu.AddItem(settings);
            menu.AddItem(quit);

            start.Action += (s, a) => {
                Manager.Add(CreateMapView());
                Hide();
            };

            settings.Action += (s, a) => {
                Manager.Add(new SettingsView(this, Game));
                Hide();
            };

            highscore.Action += (s, a) => {
                Manager.Add(new HighscoreView(Game));
                Hide();
            };

            quit.Action += (s, a) => Close();

            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(menu);
        }

        public MapView CreateMapView() {
            int vw = (int)(Graphics.Viewport.Width*0.75f);
            int vh = (int)(Graphics.Viewport.Height*0.75f);
            GameMap gameMap = new GameMap(
                Game.Settings.MapWidth,
                Game.Settings.MapHeight,
                Game.Settings.MineDensitiy);

            return new MapView(this, gameMap, vw, vh, Game);
        }
    }
}
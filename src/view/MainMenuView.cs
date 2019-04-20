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

        public MainMenuView(ContentManager content, GraphicsDevice graphics) : base(content, graphics) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            Init();
        }

        public void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Gold;
            background.Alpha = 0.5f;

            MenuItem start = new MenuItem("Start Game", font);
            MenuItem settings = new MenuItem("Settings", font);
            MenuItem highscore = new MenuItem("Highscores", font);
            MenuItem quit = new MenuItem("Quit", font);

            start.Text.LayoutWithTrueSize = true;
            settings.Text.LayoutWithTrueSize = true;
            highscore.Text.LayoutWithTrueSize = true;
            quit.Text.LayoutWithTrueSize = true;

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
            menu.AddItem(settings);
            menu.AddItem(highscore);
            menu.AddItem(quit);

            start.Action += (s, a) => {
                Manager.Add(CreateMapView());
                Hide();
            };

            settings.Action += (s, a) => {
                // TODO
            };

            highscore.Action += (s, a) => {
                // TODO
            };

            quit.Action += (s, a) => Close();

            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(menu);
        }

        public MapView CreateMapView() {
            int vw = (int)(Graphics.Viewport.Width*0.75f);
            int vh = (int)(Graphics.Viewport.Height*0.75f);
            GameMap gameMap = new GameMap(MapDifficulty.Easy);
            return new MapView(gameMap, vw, vh, Content, Graphics);
        }
    }
}
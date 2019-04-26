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

        public MainMenuView(Minesweeper game) : base(game) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            Media.AddSong("audio/songs/menu_theme_0");
            Media.AddSong("audio/songs/menu_theme_1");
            Init();
        }

        public override void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.CornflowerBlue;
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

            // ListMenu menu = new ListMenu(start, settings, highscore, quit); // TODO fix ctor
            ListMenu menu = new ListMenu();
            menu.ItemsOrientation = Orientation.Vertical;
            menu.AddItem(start);
            menu.AddItem(highscore);
            menu.AddItem(settings);
            menu.AddItem(quit);

            start.Action += (s, a) => {
                Manager.Add(Game.CreateMapView(this));
                Hide();
            };

            settings.Action += (s, a) => {
                Manager.Add(new SettingsView(this, Game));
                Hide();
            };

            highscore.Action += (s, a) => {
                Manager.Add(new HighscoreView(this, Game));
                Hide();
            };

            quit.Action += (s, a) => Close();

            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(menu);
        }
    }
}
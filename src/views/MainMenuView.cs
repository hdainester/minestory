using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public class MainMenuView : GameView {
        private ImageItem backItem;
        private Texture2D banner;
        private SpriteFont font;

        public MainMenuView(Minesweeper game) : base(game) {
            Media.AddSong("audio/songs/menu_theme_0");
            // Media.AddSong("audio/songs/menu_theme_1");
            Init();
        }

        public override void Init() {
            Background = Content.Load<Texture2D>("textures/home_back");
            banner = Content.Load<Texture2D>("textures/banner");
            font = Content.Load<SpriteFont>("fonts/menu_font");
            backItem = new ImageItem(Background);
            backItem.HGrow = backItem.VGrow = 1;

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
            menu.HAlign = HAlignment.Center;
            menu.VAlign = VAlignment.Center;
            menu.AddItem(start);
            menu.AddItem(highscore);
            menu.AddItem(settings);
            menu.AddItem(quit);

            ImageItem bannerItem = new ImageItem(banner);
            bannerItem.VAlign = VAlignment.Center;
            bannerItem.HGrow = 1;

            VPane vPane = new VPane(bannerItem, menu);
            vPane.HAlign = HAlignment.Center;
            vPane.HGrow = 0.8f;
            vPane.VGrow = 1;

            MainContainer.Clear();
            MainContainer.Add(backItem);
            MainContainer.Add(vPane);

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
        }
    }
}
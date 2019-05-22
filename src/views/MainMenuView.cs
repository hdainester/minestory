using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Controls.Menus;
using Chaotx.Mgx.Controls;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.Views;

namespace Chaotx.Minestory {
    public class MainMenuView : GameView {
        private ImageItem backItem;
        private Texture2D banner;
        private SpriteFont font;

        public MainMenuView(Minestory game) : base(game) {
            Media.AddSong("audio/songs/menu_theme_0");
            // Media.AddSong("audio/songs/menu_theme_1");
            // Init(); // called automatically after added to a manager
        }

        protected override void Init() {
            Background = Content.Load<Texture2D>("textures/home_back");
            banner = Content.Load<Texture2D>("textures/banner");
            font = Content.Load<SpriteFont>("fonts/menu_font");
            backItem = new ImageItem(Background);
            backItem.HGrow = backItem.VGrow = 1;

            MenuItem start = new MenuItem("Start Game", font);
            MenuItem highscore = new MenuItem("Highscores", font);
            MenuItem settings = new MenuItem("Settings", font);
            MenuItem quit = new MenuItem("Quit", font);

            start.FocusGain += (s, a) => start.TextItem.Color = Color.Yellow;
            settings.FocusGain += (s, a) => settings.TextItem.Color = Color.Yellow;
            highscore.FocusGain += (s, a) => highscore.TextItem.Color = Color.Yellow;
            quit.FocusGain += (s, a) => quit.TextItem.Color = Color.Yellow;

            start.FocusLoss += (s, a) => start.TextItem.Color = Color.White;
            settings.FocusLoss += (s, a) => settings.TextItem.Color = Color.White;
            highscore.FocusLoss += (s, a) => highscore.TextItem.Color = Color.White;
            quit.FocusLoss += (s, a) => quit.TextItem.Color = Color.White;

            // ListMenu menu = new ListMenu(start, settings, highscore, quit); // TODO fix ctor
            ListMenu menu = new ListMenu();
            menu.ItemsOrientation = Orientation.Vertical;
            menu.VAlign = VAlignment.Top;
            menu.AddItem(start);
            menu.AddItem(highscore);
            menu.AddItem(settings);
            menu.AddItem(quit);

            HPane menuPane = new HPane(menu);
            menuPane.HAlign = HAlignment.Center;
            menuPane.VGrow = 1;

            ImageItem bannerItem = new ImageItem(banner, 400, 200);
            bannerItem.VAlign = VAlignment.Center;
            bannerItem.HGrow = 1;

            HPane bannerPane = new HPane(bannerItem);
            bannerPane.HGrow = bannerPane.VGrow = 1;

            VPane vPane = new VPane(bannerPane, menuPane);
            vPane.HAlign = HAlignment.Center;
            vPane.HGrow = 0.8f;
            vPane.VGrow = 1;

            ViewPane.Clear();
            RootPane = new StackPane(backItem, vPane);
            RootPane.HGrow = RootPane.VGrow = 1;
            ViewPane.Add(RootPane);

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
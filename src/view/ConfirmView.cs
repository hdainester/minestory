using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using System;

namespace Chaotx.Minesweeper {
    public class ConfirmView : GameView {
        public Action YesAction {get; set;}
        public Action NoAction {get; set;}

        private ImageItem background;
        private SpriteFont font;
        private Texture2D blank;
        private string message;

        public ConfirmView(GameView parent, string message) : base(parent) {
            FadeInTime = FadeOutTime = 500;
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            this.message = message;
            Init();
        }

        public void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Black;
            background.Alpha = 0.3f;

            TextItem msg = new TextItem(font, message);
            MenuItem yes = new MenuItem("Yes", font);
            MenuItem no = new MenuItem("No", font);
            MenuItem gap = new MenuItem("   ", font);
            msg.HAlign = HAlignment.Center;
            gap.IsDisabled = true;

            // ListMenu menu = new ListMenu(yes, no); // TODO fix ctor
            ListMenu menu = new ListMenu();
            menu.VAlign = VAlignment.Center;
            menu.AddItem(yes);
            menu.AddItem(gap);
            menu.AddItem(no);

            VPane vPane = new VPane(msg, menu);
            vPane.HAlign = HAlignment.Center;
            vPane.VGrow = 1;

            ImageItem vpBack = new ImageItem(blank);
            vpBack.HGrow = vpBack.VGrow = 1;
            vpBack.Color = Color.Black;
            vpBack.Alpha = 0.3f;

            StackPane sPane = new StackPane(vpBack, vPane);
            sPane.VAlign = VAlignment.Center;
            sPane.VGrow = 0.2f;
            sPane.HGrow = 1;

            yes.FocusGain += (s, a) => yes.Text.Color = Color.Yellow;
            yes.FocusLoss += (s, a) => yes.Text.Color = Color.White;

            no.FocusGain += (s, a) => no.Text.Color = Color.Yellow;
            no.FocusLoss += (s, a) => no.Text.Color = Color.White;

            yes.Action += (s, a) => {
                YesAction();
                Close();
            };

            no.Action += (s, a) => {
                NoAction();
                Close();
            };

            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(sPane);
        }
    }
}
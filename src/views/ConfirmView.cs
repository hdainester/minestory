using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Controls.Menus;
using Chaotx.Mgx.Controls;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.Views;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minestory {
    public class ConfirmRespond {
        public string Text {get;}
        public Action Action {get;}

        public ConfirmRespond(string text, Action action) {
            Text = text;
            Action = action;
        }
    }
    
    public class ConfirmView : GameView {
        private ImageItem background;
        private SpriteFont font;
        private Texture2D blank;

        private List<ConfirmRespond> responds;
        private string message;
        private Action action;
        private bool performed;

        public ConfirmView(GameView parent, string message,
        params ConfirmRespond[] responds) : base(parent) {
            this.responds = responds.ToList();
            this.message = message;
            // Init(); // called automatically after added to a manager
        }

        protected override void Init() {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Black;
            background.Alpha = 0.3f;

            TextItem msg = new TextItem(font, message);
            msg.HAlign = HAlignment.Center;

            ListMenu menu = new ListMenu();
            menu.VAlign = VAlignment.Center;

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

            responds.ForEach(respond => {
                if(respond != responds[0]) {
                    MenuItem gap = new MenuItem("   ", font);
                    gap.IsDisabled = true;
                    menu.AddItem(gap);
                }
                
                MenuItem item = new MenuItem(respond.Text, font);
                menu.AddItem(item);

                item.FocusGain += (s, a) => item.TextItem.Color = Color.Yellow;
                item.FocusLoss += (s, a) => item.TextItem.Color = Color.White;
                item.Action += (s, a) => {
                    action = respond.Action;
                    Close();
                };
            });

            ViewPane.Clear();
            RootPane = new StackPane(background, sPane);
            RootPane.HGrow = RootPane.VGrow = 1;
            ViewPane.Add(RootPane);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if(!performed && State == ViewState.Closed) {
                performed = true;
                action();
            }
        }
    }
}
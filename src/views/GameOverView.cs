using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Controls.Menus;
using Chaotx.Mgx.Controls;
using Chaotx.Mgx.Layout;

using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Chaotx.Minestory {
    public class GameOverView : GameView {
        private Highscore score, kicked;

        private SpriteFont font;
        private Texture2D blank;
        private ImageItem background;

        private int scoreIndex;
        private LayoutPane mainPane;
        private string message;

        public GameOverView(MapView parent, Highscore score) : base(parent) {
            this.score = score;
            // Init(); // called automatically after added to a manager
        }

        protected override void Init() {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Black;
            background.Alpha = 0.5f;
            
            TextItem gameOver = new TextItem(font, "Game Over");
            gameOver.HAlign = HAlignment.Center;
            gameOver.VAlign = VAlignment.Center;
            gameOver.IsSizeScaled = true;
            gameOver.Scale = 2;

            HPane hGameOver = new HPane(gameOver);
            hGameOver.HGrow = hGameOver.VGrow = 1;

            ImageItem bTop = new ImageItem(blank);
            bTop.HGrow = bTop.VGrow = 1;
            bTop.Color = Color.DarkGray;
            bTop.Alpha = 0.5f;

            ImageItem bBot = new ImageItem(blank);
            bBot.HGrow = bBot.VGrow = 1;
            bBot.Color = Color.Black;
            bBot.Alpha = 0.5f;

            StackPane sTop = new StackPane(bTop, hGameOver);
            sTop.VAlign = VAlignment.Center;
            sTop.HAlign = HAlignment.Center;
            sTop.HGrow = 0.5f;
            sTop.VGrow = 1;

            mainPane = new StackPane(bBot);
            mainPane.HAlign = HAlignment.Center;
            mainPane.VAlign = VAlignment.Center;
            mainPane.HGrow = 0.5f;
            mainPane.VGrow = 1;

            VPane vPane = new VPane(sTop, mainPane);
            vPane.HGrow = vPane.VGrow = 1;

            ViewPane.Clear();
            RootPane = new StackPane(background, vPane);
            RootPane.HGrow = RootPane.VGrow = 1;
            ViewPane.Add(RootPane);

            Task.Run(() => {
                Game.Scores = FileManager.LoadHighscores(Game.ScoresPath);
                int pos = Game.AddHighscore(score, out kicked);

                if(score.MinesHit < score.TotalMines
                && pos <= Minestory.MAX_SCORES_PER_DIFF) {
                    message = "New Highscore! Rank " + pos + ". Enter your Name:";
                    mainPane.Add(CreateTextInputPane(message));
                } else mainPane.Add(CreateNavPane());
            });
        }

        private LayoutPane CreateNavPane() {
            MenuItem newGame = new MenuItem("New Game", font);
            MenuItem mainMenu = new MenuItem("Main Menu", font);
            newGame.HAlign = mainMenu.HAlign = HAlignment.Center;

            HPane hNewGame = new HPane(newGame);
            HPane hMainMenu = new HPane(mainMenu);
            hNewGame.VAlign = hMainMenu.VAlign = VAlignment.Center;
            hNewGame.HGrow = hMainMenu.HGrow = 1;

            HPane hPane = new HPane(hNewGame, hMainMenu);
            hPane.HAlign = HAlignment.Center;
            hPane.VAlign = VAlignment.Center;
            hPane.HGrow = hPane.VGrow = 1;

            newGame.FocusGain += (s, a) => newGame.TextItem.Color = Color.Yellow;
            newGame.FocusLoss += (s, a) => newGame.TextItem.Color = Color.White;

            mainMenu.FocusGain += (s, a) => mainMenu.TextItem.Color = Color.Yellow;
            mainMenu.FocusLoss += (s, a) => mainMenu.TextItem.Color = Color.White;

            newGame.Action += (s, a) => {
                Manager.Add(Game.CreateMapView(Parent.Parent));
                Parent.Close();
                Close();
            };

            mainMenu.Action += (s, a) => {
                Parent.Close();
                Close();
            };

            return hPane;
        }

        private LayoutPane CreateTextInputPane(string msg) {
            MenuItem confirm = new MenuItem("Confirm", font);
            TextItem message = new TextItem(font, msg);
            TextField textField = new TextField(Game.Window, font, blank);
            confirm.HAlign = message.HAlign = textField.HAlign =  HAlignment.Center;
            confirm.VAlign = message.VAlign = textField.VAlign =  VAlignment.Center;

            textField.TextAlignment = HAlignment.Center;
            textField.HGrow = 0.8f;

            confirm.IsDisabled = textField.Text.Length < Minestory.MIN_NAME_LEN;
            confirm.Alpha = confirm.IsDisabled ? 0.5f : 1;

            HPane hConfirm = new HPane(confirm);
            HPane hMessage = new HPane(message);
            HPane hTextField = new HPane(textField);
            VPane vPane = new VPane(hMessage, hTextField, hConfirm);
            hConfirm.HGrow = hMessage.HGrow = hTextField.HGrow = 1;
            vPane.HAlign = HAlignment.Center;
            vPane.VAlign = VAlignment.Center;
            vPane.HGrow = vPane.VGrow = 1;

            textField.TextInput += (s, a) => {
                confirm.IsDisabled = textField.Text.Length < Minestory.MIN_NAME_LEN;
                confirm.Alpha = confirm.IsDisabled ? 0.5f : 1;

                int cx = (int)(confirm.X + confirm.Width/2);
                int cy = (int)(confirm.Y + confirm.Height/2);
                Mouse.SetPosition(cx, cy + (confirm.IsDisabled ? + 1 : 0)); // forces focus gain

                if(textField.Text.Length > Minestory.MAX_NAME_LEN)
                    textField.Text = textField.Text.Remove(Minestory.MAX_NAME_LEN);
            };

            confirm.FocusGain += (s, a) => confirm.TextItem.Color = Color.Yellow;
            confirm.FocusLoss += (s, a) => confirm.TextItem.Color = Color.White;
            confirm.Action += (s, a) => SaveScores(textField, vPane);

            return vPane;
        }

        private void SaveScores(TextField nameField, LayoutPane pane) {
            MapDifficulty diff = Game.Settings.Difficulty;
            score.Name = nameField.Text;

            int p = 1;
            Highscore best = null;
            List<Highscore> dups = new List<Highscore>();
            var it = Game.ScoresOf(diff).GetEnumerator();

            for(it.MoveNext(); it.Current != null; it.MoveNext()) {
                if(it.Current.Name.Equals(score.Name)) {
                    if(best == null)
                        best = it.Current;
                    else dups.Add(it.Current);
                }

                if(best == null) ++p;
            }

            if(score == best) {
                Task.Run(() => {
                    mainPane.Remove(pane);
                    dups.ForEach(Game.RemoveHighscore);
                    FileManager.SaveHighscores(Game.ScoresPath, Game.Scores);
                    MySqlHelper.Instance.Sync(Game);
                    mainPane.Add(CreateNavPane());
                });
            } else {
                Game.Scores.Remove(score);
                if(kicked != null) Game.Scores.Add(kicked);
                var ti = new TextItem(font, string.Format("A better score by {0} exists already at Rank {1}.", best.Name, p));
                var fp = new FadingPane(2000);
                fp.HGrow = fp.VGrow = 1;
                ti.HAlign = HAlignment.Center;
                ti.VAlign = VAlignment.Center;
                fp.Add(ti);

                fp.FadedOut += (s, a) => {
                    mainPane.Remove(fp);
                    mainPane.Add(CreateNavPane());
                };

                mainPane.Remove(pane);
                mainPane.Add(fp);
            }
        }
    }
}
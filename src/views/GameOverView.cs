using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;

namespace Chaotx.Minesweeper {
    public class GameOverView : GameView {
        private Highscore score;

        private SpriteFont font;
        private Texture2D blank;
        private ImageItem background;

        private int scoreIndex;
        private LayoutPane mainPane;

        public GameOverView(MapView parent, Highscore score) : base(parent) {
            this.score = score;
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Black;
            background.Alpha = 0.5f;
            Init();
        }

        public override void Init() {
            TextItem gameOver = new TextItem(font, "Game Over");
            gameOver.HAlign = HAlignment.Center;
            gameOver.VAlign = VAlignment.Center;
            gameOver.LayoutWithTrueSize = true;
            gameOver.Scale = 2;

            HPane hGameOver = new HPane(gameOver);
            hGameOver.HGrow = hGameOver.VGrow = 1;

            ImageItem bItem = new ImageItem(blank);
            bItem.HGrow = bItem.VGrow = 1;
            bItem.Color = Color.DarkGray;
            bItem.Alpha = 0.5f;

            StackPane sPane = new StackPane(bItem, hGameOver);
            sPane.VAlign = VAlignment.Center;
            sPane.HAlign = HAlignment.Center;
            sPane.HGrow = 0.5f;
            sPane.VGrow = 1;

            mainPane = new VPane(sPane);
            mainPane.HGrow = mainPane.VGrow = 1;
            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(mainPane);

            if(score.MinesHit < score.TotalMines
            && (scoreIndex = Game.GetScoreIndex(score)) < Minesweeper.MAX_SCORES)
                mainPane.Add(CreateTextInputPane("New Highscore! Enter your Name:"));
            else mainPane.Add(CreateNavPane());
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
            hPane.HGrow = hPane.VGrow = 1;

            ImageItem bItem = new ImageItem(blank);
            bItem.HGrow = bItem.VGrow = 1;
            bItem.Color = Color.Black;
            bItem.Alpha = 0.5f;

            StackPane sPane = new StackPane(bItem, hPane);
            sPane.HAlign = HAlignment.Center;
            sPane.VAlign = VAlignment.Center;
            sPane.HGrow = 0.5f;
            sPane.VGrow = 1;

            newGame.FocusGain += (s, a) => newGame.Text.Color = Color.Yellow;
            newGame.FocusLoss += (s, a) => newGame.Text.Color = Color.White;

            mainMenu.FocusGain += (s, a) => mainMenu.Text.Color = Color.Yellow;
            mainMenu.FocusLoss += (s, a) => mainMenu.Text.Color = Color.White;

            newGame.Action += (s, a) => {
                Manager.Add(((MainMenuView)Parent.Parent).CreateMapView());
                Parent.Close();
                Close();
            };

            mainMenu.Action += (s, a) => {
                Parent.Close();
                Close();
            };

            return sPane;
        }

        private LayoutPane CreateTextInputPane(string msg) {
            MenuItem confirm = new MenuItem("Confirm", font);
            TextItem message = new TextItem(font, msg);
            TextField textField = new TextField(Game.Window, font, blank);
            confirm.HAlign = message.HAlign = textField.HAlign =  HAlignment.Center;
            confirm.VAlign = message.VAlign = textField.VAlign =  VAlignment.Center;

            textField.TextAlignment = HAlignment.Center;
            textField.HGrow = 0.8f;

            if(Game.Settings.LastScore != null)
                textField.Text = Game.Settings.LastScore.Name;

            confirm.IsDisabled = textField.Text.Length < Minesweeper.MIN_NAME_LEN;
            confirm.Alpha = confirm.IsDisabled ? 0.5f : 1;

            HPane hConfirm = new HPane(confirm);
            HPane hMessage = new HPane(message);
            HPane hTextField = new HPane(textField);
            VPane vPane = new VPane(hMessage, hTextField, hConfirm);
            hConfirm.HGrow = hMessage.HGrow = hTextField.HGrow = 1;
            vPane.HAlign = HAlignment.Center;
            vPane.VAlign = VAlignment.Center;
            vPane.HGrow = vPane.VGrow = 1;

            ImageItem bItem = new ImageItem(blank);
            bItem.HGrow = bItem.VGrow = 1;
            bItem.Color = Color.Black;
            bItem.Alpha = 0.5f;

            StackPane sPane = new StackPane(bItem, vPane);
            sPane.HAlign = HAlignment.Center;
            sPane.VAlign = VAlignment.Center;
            sPane.HGrow = 0.5f;
            sPane.VGrow = 1;

            textField.TextInput += (s, a) => {
                confirm.IsDisabled = textField.Text.Length < Minesweeper.MIN_NAME_LEN;
                confirm.Alpha = confirm.IsDisabled ? 0.5f : 1;

                if(textField.Text.Length > Minesweeper.MAX_NAME_LEN)
                    textField.Text = textField.Text.Remove(Minesweeper.MAX_NAME_LEN);
            };

            textField.KeyReleased += (s, a) => {
                if(a.Key == Keys.Enter)
                    ConfirmHighscore(textField, sPane);
            };

            confirm.FocusGain += (s, a) => confirm.Text.Color = Color.Yellow;
            confirm.FocusLoss += (s, a) => confirm.Text.Color = Color.White;
            confirm.Action += (s, a) => ConfirmHighscore(textField, sPane);

            return sPane;
        }

        private void ConfirmHighscore(TextField textField, StackPane sPane) {
            score.Name = textField.Text;
            if(scoreIndex >= Game.Scores.Count)
                Game.Scores.Add(score);
            else Game.Scores.Insert(scoreIndex, score);

            Game.Settings.LastScore = score;
            FileManager.SaveHighscores(Minesweeper.SCORES_PATH, Game.Scores);
            FileManager.SaveSettings(Minesweeper.SETTINGS_PATH, Game.Settings);

            mainPane.Remove(sPane);
            mainPane.Add(CreateNavPane());
        }
    }
}
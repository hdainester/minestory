using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;

using System.Linq;

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

            MainContainer.Clear();
            MainContainer.Add(background);
            MainContainer.Add(vPane);

            if(score.MinesHit < score.TotalMines
            && (scoreIndex = Game.GetScoreIndex(score)) >= 0)
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
            hPane.HAlign = HAlignment.Center;
            hPane.VAlign = VAlignment.Center;
            hPane.HGrow = hPane.VGrow = 1;

            newGame.FocusGain += (s, a) => newGame.Text.Color = Color.Yellow;
            newGame.FocusLoss += (s, a) => newGame.Text.Color = Color.White;

            mainMenu.FocusGain += (s, a) => mainMenu.Text.Color = Color.Yellow;
            mainMenu.FocusLoss += (s, a) => mainMenu.Text.Color = Color.White;

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

            textField.TextInput += (s, a) => {
                confirm.IsDisabled = textField.Text.Length < Minesweeper.MIN_NAME_LEN;
                confirm.Alpha = confirm.IsDisabled ? 0.5f : 1;

                if(textField.Text.Length > Minesweeper.MAX_NAME_LEN)
                    textField.Text = textField.Text.Remove(Minesweeper.MAX_NAME_LEN);
            };

            textField.KeyReleased += (s, a) => {
                if(a.Key == Keys.Enter)
                    AddHighscore(textField, vPane);
            };

            confirm.FocusGain += (s, a) => confirm.Text.Color = Color.Yellow;
            confirm.FocusLoss += (s, a) => confirm.Text.Color = Color.White;
            confirm.Action += (s, a) => AddHighscore(textField, vPane);

            return vPane;
        }

        private void AddHighscore(TextField textField, LayoutPane pane) {
            MapDifficulty diff = Game.Settings.Difficulty;
            score.Name = textField.Text;

            if(scoreIndex >= Game.Scores.Count())
                Game.Scores.Add(score);
            else Game.Scores.Insert(scoreIndex, score);

            if(Game.Scores
            .Where(s => s.Settings.Difficulty == diff)
            .Count() > Minesweeper.MAX_SCORES_PER_DIFF)
                Game.Scores.Remove(Game.Scores.FindLast(
                    h => h.Settings.Difficulty == diff));

            FileManager.SaveHighscores(Minesweeper.SCORES_PATH, Game.Scores);
            FileManager.SaveSettings(Minesweeper.SETTINGS_PATH, Game.Settings);

            mainPane.Remove(pane);
            mainPane.Add(CreateNavPane());
        }
    }
}
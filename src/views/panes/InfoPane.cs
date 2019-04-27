using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Layout;
using Chaotx.Mgx.Control;

using System.Linq;

namespace Chaotx.Minestory {
    public class InfoPane : StackPane {
        private ImageItem background {get;}
        private TextItem difficultyText;
        private TextItem revealedText;
        private TextItem minesText;
        private TextItem timeText;
        private SpriteFont font;

        private Minestory game;
        private GameMap map;

        public InfoPane(GameMap map, Minestory game, SpriteFont font, Texture2D background) {
            this.map = map;
            this.game = game;
            this.font = font;
            this.background = new ImageItem(background);
            this.background.Color = Color.DarkGray;
            this.background.Alpha = 0.5f;
            Init();
        }

        public void Init() {
            background.HGrow = background.VGrow = 1;
            revealedText = new TextItem(font);
            minesText = new TextItem(font);
            timeText = new TextItem(font);
            difficultyText = new TextItem(font);

            revealedText.Color = Color.Black;
            minesText.Color = Color.Black;
            timeText.Color = Color.Black;
            difficultyText.Color = Color.Black;

            revealedText.Alpha = 0.75f;
            minesText.Alpha = 0.75f;
            timeText.Alpha = 0.75f;
            difficultyText.Alpha = 0.75f;

            VPane vPane = new VPane(difficultyText, revealedText, minesText, timeText);
            vPane.Children.ToList().ForEach(child => child.HAlign = HAlignment.Center);
            vPane.HGrow = 1;

            Clear();
            Add(background);
            Add(vPane);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            difficultyText.Text = string.Format("Difficulty: {0}", game.Settings.Difficulty);
            revealedText.Text = string.Format("Revealed Tiles: {0:000}/{1:000}", map.RevealedTiles, map.TotalTiles);
            minesText.Text = string.Format("Revealed Mines: {0:00}/{1:00}", map.RevealedMines, map.TotalMines);
            timeText.Text = string.Format("Time: {0}", map.ElapsedTime.ToString(@"hh\:mm\:ss\.ff"));
        }
    }
}
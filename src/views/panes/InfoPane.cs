using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Layout;
using Chaotx.Mgx.Control;

namespace Chaotx.Minesweeper {
    public class InfoPane : StackPane {
        public GameMap Map {get;}

        private ImageItem background {get;}
        private TextItem revealedText;
        private TextItem minesText;
        private TextItem timeText;
        private SpriteFont font;


        public InfoPane(GameMap map, SpriteFont font, Texture2D background) {
            Map = map;
            this.font = font;
            this.background = new ImageItem(background);
            this.background.Color = Color.Green;
            this.background.Alpha = 0.35f;
            init();
        }

        public void init() {
            background.HGrow = background.VGrow = 1;
            revealedText = new TextItem(font);
            minesText = new TextItem(font);
            timeText = new TextItem(font);
            VPane vPane = new VPane(revealedText, minesText, timeText);

            Clear();
            Add(background);
            Add(vPane);

            HGrow = 0.8f;
            HAlign = HAlignment.Center;
            // VGrow = 1;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            revealedText.Text = string.Format("Revealed Tiles: {0}/{1}", Map.RevealedTiles, Map.TotalTiles);
            minesText.Text = string.Format("Revealed Mines: {0}/{1}", Map.RevealedMines, Map.TotalMines);
            timeText.Text = string.Format("Time: {0}", Map.ElapsedTime.ToString(@"hh\:mm\:ss\.ff"));
        }
    }
}
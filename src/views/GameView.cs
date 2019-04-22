using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public abstract class GameView : FadingView {
        public MediaManager Media {get;}
        public GameView Parent {get;}
        public Minesweeper Game {get;}

        public GameView(GameView parent)
        : this(parent, parent.Game) {}

        public GameView(Minesweeper game)
        : this(null, game) {}

        public GameView(GameView parent, Minesweeper game) : base(game.Content, game.GraphicsDevice) {
            Media = new MediaManager(game);
            Media.Reapeat = RepeatMode.RepeatAll;
            FadeInTime = 800;
            FadeOutTime = 400;
            Parent = parent;
            Game = game;
        }

        public override void Show() {
            base.Show();

            if(Media.Songs.Count > 0)
                Media.PlaySong(0);
        }

        public override void Close() {
            base.Close();
            Media.StopSong();
        }

        public override void Hide() {
            base.Hide();
            Media.StopSong();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Media.Update(gameTime);
        }

        public abstract void Init();
    }
}
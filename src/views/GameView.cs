using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Views;

namespace Chaotx.Minestory {
    public abstract class GameView : FadingView {
        public MediaManager Media {get;}
        public GameView Parent {get;}
        public Minestory Game {get;}
        public Texture2D Background {get; protected set;}

        public GameView(GameView parent)
        : this(parent, parent.Game) {}

        public GameView(Minestory game)
        : this(null, game) {}

        public GameView(GameView parent, Minestory game) : base() {
            game.Window.ClientSizeChanged += (s, a) => AlignViewPane();
            Media = new MediaManager(game);
            Media.Repeat = RepeatMode.RepeatAll;
            FadeInTime = 800;
            FadeOutTime = 400;
            Parent = parent;
            Game = game;
        }

        public override void Show() {
            base.Show();

            Media.SongVolume = Game.Settings.MusicVolume/100f;
            Media.SoundVolume = Game.Settings.AudioVolume/100f;

            if(Media.Songs.Count > 0) {
                if(!Media.IsRunning)
                    Media.PlaySong(0);
            }
        }

        public override void Close() {
            base.Close();

            if(Media.Songs.Count > 0 && Media.IsRunning)
                Media.StopSong();
        }

        public override void Hide() {
            base.Hide();
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            Media.Update(gameTime);
        }
    }
}
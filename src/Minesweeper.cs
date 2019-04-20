using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public class Minesweeper : Game {
        private SpriteBatch spriteBatch;
        private ViewControl viewControl;
        GraphicsDeviceManager graphics;

        public Minesweeper() {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            viewControl = new ViewControl();
        }

        protected override void LoadContent() {
            int vw = (int)(GraphicsDevice.Viewport.Width*0.75f);
            int vh = (int)(GraphicsDevice.Viewport.Height*0.75f);

            GameMap gameMap = new GameMap(MapDifficulty.Easy);
            MapView mapView = new MapView(gameMap, vw, vh, Content, GraphicsDevice);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewControl.Add(mapView);
        }

        protected override void Update(GameTime gameTime) {
            viewControl.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.SlateGray);
            viewControl.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}

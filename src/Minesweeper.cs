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
            MainMenuView menuView = new MainMenuView(Content, GraphicsDevice);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewControl.Add(menuView);
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

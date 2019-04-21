using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public abstract class GameView : FadingView {
        public GameView Parent {get;}

        public GameView(ContentManager content, GraphicsDevice graphics)
        : base(content, graphics) {}

        public GameView(GameView parent) : base(parent.Content, parent.Graphics) {
            Parent = parent;
        }

        public abstract void Init();
    }
}
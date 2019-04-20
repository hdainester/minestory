using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.IO;
using System;

using Chaotx.Mgx.View;

namespace Chaotx.Minesweeper {
    public class Minesweeper : Game {
        public static readonly string APP_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            + Path.DirectorySeparatorChar + "chaotx"
            + Path.DirectorySeparatorChar + "minesweeper";

        public static readonly string SCORES_PATH = APP_DIRECTORY + Path.DirectorySeparatorChar + "scores.dat";
        public static readonly string SETTINGS_PATH = APP_DIRECTORY + Path.DirectorySeparatorChar + "settings.dat";

        public GameSettings Settings {get; set;}
        public List<Highscore> Scores {get; set;}

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
            Directory.CreateDirectory(APP_DIRECTORY);
            Settings = FileManager.LoadSettings(SETTINGS_PATH);
            Scores = FileManager.LoadHighscores(SCORES_PATH);
            if(Settings == null) Settings = CreateDefaultSettings();
            if(Scores == null) Scores = new List<Highscore>();

            MainMenuView menuView = new MainMenuView(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewControl.Add(menuView);
        }

        protected override void Update(GameTime gameTime) {
            if(viewControl.Views.Count == 0) Exit();
            viewControl.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.SlateGray);
            viewControl.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        private GameSettings CreateDefaultSettings() {
            return new GameSettings(MapDifficulty.Easy, 9, 9, 12, 70, 70, "en");
        }
    }
}

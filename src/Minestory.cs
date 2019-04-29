using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

using Chaotx.Mgx.View;

namespace Chaotx.Minestory {
    public class Minestory : Game {
        public static readonly string SCORES_FILE = "scores";
        public static readonly string SETTINGS_FILE = "settings";
        public static readonly int MAX_SCORES_PER_DIFF = 100;
        public static readonly int MIN_NAME_LEN = 3;
        public static readonly int MAX_NAME_LEN = 8;

        public string AppDirectory {get;}
        public string ScoresPath => AppDirectory + Path.DirectorySeparatorChar + SCORES_FILE;
        public string SettingsPath => AppDirectory + Path.DirectorySeparatorChar + SETTINGS_FILE;

        public GameSettings Settings {get; set;}
        public SortedSet<Highscore> Scores {get; set;}

        private SpriteBatch spriteBatch;
        private ViewControl viewControl;
        GraphicsDeviceManager graphics;

        public Minestory(string appDirectory) {
            DropboxConnect.Game = this;
            AppDirectory = appDirectory;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            viewControl = new ViewControl();
        }

        public void ApplyWindowMode(WindowMode mode) {
            if(mode == WindowMode.Fullscreen && !graphics.IsFullScreen) {
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            } else if(mode == WindowMode.Windowed && graphics.IsFullScreen) {
                graphics.PreferredBackBufferWidth = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width*0.75f);
                graphics.PreferredBackBufferHeight = (int)(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height*0.75f);
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }
        }

        public int AddHighscore(Highscore score) {
            Highscore lastSpot = null;
            bool spotFound = false;
            int curPos = 1, lastPos = 1;

            Scores.Where(s => s.Settings.Difficulty == score.Settings.Difficulty).ToList().ForEach(s => {
                if(score <= s) spotFound = true;
                if(!spotFound) ++curPos;
                lastSpot = s;
                ++lastPos;
            });

            if(curPos < lastPos
            && lastPos > MAX_SCORES_PER_DIFF)
                Scores.Remove(lastSpot);
            
            if(curPos <= MAX_SCORES_PER_DIFF
            && !Scores.Contains(score))
                Scores.Add(score);

            return curPos;
        }
        
        public MapView CreateMapView(GameView parentView) {
            int vw = (int)(GraphicsDevice.Viewport.Width*0.75f);
            int vh = (int)(GraphicsDevice.Viewport.Height*0.75f);
            GameMap gameMap = new GameMap(
                Settings.MapWidth,
                Settings.MapHeight,
                Settings.MineDensitiy);

            return new MapView(parentView, gameMap, vw, vh, this);
        }

        protected GameSettings CreateDefaultSettings() {
            return new GameSettings(
                MapDifficulty.Easy,
                9, 9, 12, 70, 70);
        }

        protected override void LoadContent() {
            Directory.CreateDirectory(AppDirectory);
            Settings = FileManager.LoadSettings(SettingsPath);
            Scores = FileManager.LoadHighscores(ScoresPath);
            if(Settings == null) Settings = CreateDefaultSettings();
            MainMenuView menuView = new MainMenuView(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewControl.Add(menuView);

            // This ensures a change in the window mode which
            // therefore (hopefully) guaranties the proper
            // alignment of the main container in Windowed mode.
            ApplyWindowMode(WindowMode.Windowed);
            ApplyWindowMode(WindowMode.Fullscreen);
            ApplyWindowMode(Settings.WindowMode);
        }

        protected override void Update(GameTime gameTime) {
            if(viewControl.Views.Count == 0) Exit();
            viewControl.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            viewControl.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}

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

        public static readonly int MAX_SCORES = 100;
        public static readonly int MIN_NAME_LEN = 3;
        public static readonly int MAX_NAME_LEN = 8;

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

        public void ApplyWindowMode(WindowMode mode) {
            if(mode == WindowMode.Fullscreen && !graphics.IsFullScreen) {
                graphics.IsFullScreen = true;
                graphics.ApplyChanges();
            } else if(mode == WindowMode.Windowed && graphics.IsFullScreen) {
                graphics.IsFullScreen = false;
                graphics.ApplyChanges();
            }
        }

        public int GetScoreIndex(Highscore score) {
            int i = 0;
            Highscore current;

            for(; i < Scores.Count; ++i) {
                current = Scores[i];
                int diff1 = (((int)score.Settings.Difficulty)+1)%4;
                int diff2 = (((int)current.Settings.Difficulty)+1)%4;

                if(diff1 >= diff2) {
                    float mines1 = 0;
                    float mines2 = 0;

                    if(diff1 == diff2) {
                        mines1 = score.MinesHit/(float)score.TotalMines;
                        mines2 = current.MinesHit/(float)current.TotalMines;
                    } else

                    if(diff1 > diff2) {
                        mines1 = 0;
                        mines2 = 1;
                    }

                    if(mines1 < mines2 || mines1 == mines2
                    && score.Time <= current.Time)
                        return i;
                }
            }
            
            return i;
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

        protected override void LoadContent() {
            Directory.CreateDirectory(APP_DIRECTORY);
            Settings = FileManager.LoadSettings(SETTINGS_PATH);
            Scores = FileManager.LoadHighscores(SCORES_PATH);
            if(Settings == null) Settings = CreateDefaultSettings();
            if(Scores == null) Scores = new List<Highscore>();
            MainMenuView menuView = new MainMenuView(this);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewControl.Add(menuView);
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

        private GameSettings CreateDefaultSettings() {
            return new GameSettings(MapDifficulty.Easy, 9, 9, 12, 70, 70);
        }
    }
}

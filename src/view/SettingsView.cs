using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public class SettingsView : GameView {
        private MapDifficulty difficulty;
        private int width, height, density;
        private int audioVolume, musicVolume;
        private string language;

        private SpriteFont font;
        private Texture2D blank;
        private Texture2D arrRight, arrLeft;
        private ImageItem background;

        private TextItem difficultyValue;
        private TextItem widthValue;
        private TextItem heightValue;
        private TextItem densityValue;
        private TextItem audioVolumeValue;
        private TextItem musicVolumeValue;
        private TextItem languageValue;

        private Minesweeper game;

        public SettingsView(GameView parent, Minesweeper game) : base(parent) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            arrLeft = Content.Load<Texture2D>("textures/arrow_left");
            arrRight = Content.Load<Texture2D>("textures/arrow_right");
            difficultyValue = new TextItem(font);
            widthValue = new TextItem(font);
            heightValue = new TextItem(font);
            densityValue = new TextItem(font);
            audioVolumeValue = new TextItem(font);
            musicVolumeValue = new TextItem(font);
            languageValue = new TextItem(font);
            width = game.Settings.MapWidth;
            height = game.Settings.MapHeight;
            density = game.Settings.MineDensitiy;
            audioVolume = game.Settings.AudioVolume;
            musicVolume = game.Settings.MusicVolume;
            language = game.Settings.Language;
            difficulty = game.Settings.Difficulty;
            this.game = game;
            Init();
        }

        public override void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.Black;
            background.Alpha = 0.5f;

            TextItem difficultyText = new TextItem(font, "Difficulty: ");
            TextItem widthText = new TextItem(font, "Map Width: ");
            TextItem heightText = new TextItem(font, "Map Height: ");
            TextItem densityText = new TextItem(font, "Mine Density: ");
            TextItem audioVolumeText = new TextItem(font, "Audio Volume: ");
            TextItem musicVolumeText = new TextItem(font, "Music Volume: ");
            TextItem languageText = new TextItem(font, "Language: ");

            MenuItem arrLeftDifficulty = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightDifficulty = new MenuItem(arrRight, 16, 16);
            HPane hpDifficulty = new HPane(arrLeftDifficulty, difficultyValue, arrRightDifficulty);

            hpDifficulty.HGrow = 1;
            arrLeftDifficulty.HAlign = HAlignment.Left;
            arrRightDifficulty.HAlign = HAlignment.Right;
            difficultyValue.HAlign = HAlignment.Center;

            MenuItem arrLeftWidth = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightWidth = new MenuItem(arrRight, 16, 16);
            HPane hpWidth = new HPane(arrLeftWidth, widthValue, arrRightWidth);

            hpWidth.HGrow = 1;
            arrLeftWidth.HAlign = HAlignment.Left;
            arrRightWidth.HAlign = HAlignment.Right;
            widthValue.HAlign = HAlignment.Center;

            MenuItem arrLeftHeight = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightHeight = new MenuItem(arrRight, 16, 16);
            HPane hpHeight = new HPane(arrLeftHeight, heightValue, arrRightHeight);

            hpHeight.HGrow = 1;
            arrLeftHeight.HAlign = HAlignment.Left;
            arrRightHeight.HAlign = HAlignment.Right;
            heightValue.HAlign = HAlignment.Center;

            MenuItem arrLeftDensity = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightDensity = new MenuItem(arrRight, 16, 16);
            HPane hpDensity = new HPane(arrLeftDensity, densityValue, arrRightDensity);

            hpDensity.HGrow = 1;
            arrLeftDensity.HAlign = HAlignment.Left;
            arrRightDensity.HAlign = HAlignment.Right;
            densityValue.HAlign = HAlignment.Center;

            MenuItem arrLeftAudioVolume = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightAudioVolume = new MenuItem(arrRight, 16, 16);
            HPane hpAudioVolume = new HPane(arrLeftAudioVolume, audioVolumeValue, arrRightAudioVolume);

            hpAudioVolume.HGrow = 1;
            arrLeftAudioVolume.HAlign = HAlignment.Left;
            arrRightAudioVolume.HAlign = HAlignment.Right;
            audioVolumeValue.HAlign = HAlignment.Center;

            MenuItem arrLeftMusicVolume = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightMusicVolume = new MenuItem(arrRight, 16, 16);
            HPane hpMusicVolume = new HPane(arrLeftMusicVolume, musicVolumeValue, arrRightMusicVolume);

            hpMusicVolume.HGrow = 1;
            arrLeftMusicVolume.HAlign = HAlignment.Left;
            arrRightMusicVolume.HAlign = HAlignment.Right;
            musicVolumeValue.HAlign = HAlignment.Center;

            MenuItem arrLeftLanguage = new MenuItem(arrLeft, 16, 16);
            MenuItem arrRightLanguage = new MenuItem(arrRight, 16, 16);
            HPane hpLanguage = new HPane(arrLeftLanguage, languageValue, arrRightLanguage);

            hpLanguage.HGrow = 1;
            arrLeftLanguage.HAlign = HAlignment.Left;
            arrRightLanguage.HAlign = HAlignment.Right;
            languageValue.HAlign = HAlignment.Center;

            VPane vpNames = new VPane(difficultyText, widthText, heightText, densityText, audioVolumeText, musicVolumeText, languageText);
            VPane vpValues = new VPane(hpDifficulty, hpWidth, hpHeight, hpDensity, hpAudioVolume, hpMusicVolume, hpLanguage);

            vpNames.Children.ToList().ForEach(child => child.VAlign = VAlignment.Center);
            vpValues.Children.ToList().ForEach(child => child.VAlign = VAlignment.Center);

            ListMenu exitMenu = new ListMenu();
            MenuItem accept = new MenuItem("Accept", font);
            MenuItem back = new MenuItem("Back", font);
            MenuItem gap = new MenuItem("   ", font);
            gap.IsDisabled = true;

            exitMenu.HAlign = HAlignment.Center;
            exitMenu.AddItem(back);
            exitMenu.AddItem(gap);
            exitMenu.AddItem(accept);

            HPane hPane = new HPane(vpNames, vpValues);
            VPane vPane = new VPane(hPane, exitMenu);

            hPane.VAlign = VAlignment.Center;
            exitMenu.VAlign = VAlignment.Center;

            vpNames.HGrow = 1;
            vpValues.HGrow = 1;
            hPane.HGrow = 1;
            vPane.HGrow = 1;

            StackPane sPane = new StackPane(background, vPane);
            sPane.HAlign = HAlignment.Center;
            sPane.VAlign = VAlignment.Center;
            sPane.HGrow = 0.5f;
            MainContainer.Add(sPane);

            accept.FocusGain += (s, a) => accept.Text.Color = Color.Yellow;
            accept.FocusLoss += (s, a) => accept.Text.Color = Color.White;
            accept.Action += (s, a) => {
                game.Settings = CreateGameSettings();
                FileManager.SaveSettings(Minesweeper.SETTINGS_PATH, game.Settings);
                Close();
            };

            back.FocusGain += (s, a) => back.Text.Color = Color.Yellow;
            back.FocusLoss += (s, a) => back.Text.Color = Color.White;
            back.Action += (s, a) => Close();

            arrLeftDifficulty.Action += (s, a) => {
                difficulty = (MapDifficulty)(Math.Max(0, (((int)difficulty)-1)));
                SetupDifficulty(difficulty);
            };

            arrRightDifficulty.Action += (s, a) => {
                difficulty = (MapDifficulty)(Math.Min(2, (((int)difficulty)+1)));
                SetupDifficulty(difficulty);
            };

            arrLeftWidth.Action += (s, a) => {
                if(width > 9) {
                    --width;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrRightWidth.Action += (s, a) => {
                if(width < 30) {
                    ++width;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrLeftHeight.Action += (s, a) => {
                if(height > 9) {
                    --height;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrRightHeight.Action += (s, a) => {
                if(height < 24) {
                    ++height;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrLeftDensity.Action += (s, a) => {
                if(density > 12) {
                    --density;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrRightDensity.Action += (s, a) => {
                if(density < 93) {
                    ++density;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            arrLeftAudioVolume.Action += (s, a) => audioVolume = Math.Max(0, audioVolume-10);
            arrRightAudioVolume.Action += (s, a) => audioVolume = Math.Min(100, audioVolume+10);

            arrLeftMusicVolume.Action += (s, a) => musicVolume = Math.Max(0, musicVolume-10);
            arrRightMusicVolume.Action += (s, a) => musicVolume = Math.Min(100, musicVolume+10);

            arrLeftLanguage.Action += (s, a) => language = "Foo";
            arrRightLanguage.Action += (s, a) => language = "Bar";
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            difficultyValue.Text = difficulty.ToString();
            widthValue.Text = width.ToString();
            heightValue.Text = height.ToString();
            densityValue.Text = density.ToString();
            audioVolumeValue.Text = audioVolume.ToString();
            musicVolumeValue.Text = musicVolume.ToString();
            languageValue.Text = language;
        }

        public GameSettings CreateGameSettings() {
            return new GameSettings(
                difficulty, width, height, density,
                audioVolume, musicVolume, language);
        }

        private void SetupDifficulty(MapDifficulty d) {
            switch(d) {
                case MapDifficulty.Easy:
                    width = 9;
                    height = 9;
                    density = 12;
                    break;
                case MapDifficulty.Medium:
                    width = 16;
                    height = 16;
                    density = 16;
                    break;
                case MapDifficulty.Hard:
                    width = 30;
                    height = 16;
                    density = 21;
                    break;
                default:
                    difficulty = width == 9 && height == 9 && density == 8 ? MapDifficulty.Easy
                        : width == 16 && height == 16 && density == 16 ? MapDifficulty.Medium
                        : width == 30 && height == 16 && density == 21 ? MapDifficulty.Hard : d;

                    break;
            }
        }
    }
}
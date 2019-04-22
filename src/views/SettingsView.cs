using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;
using Chaotx.Mgx.View;

using System.Collections.Generic;
using System.Linq;
using System;

namespace Chaotx.Minesweeper {
    public class SettingsView : GameView {
        private class MenuEntry {
            public LayoutPane ValuePane {get;}
            public LayoutPane NamePane {get;}
            public TextItem ValueItem {get;}
            public MenuItem NameItem {get;}
            public MenuItem ArrowLeft {get;}
            public MenuItem ArrowRight {get;}

            private object value;
            public object Value {
                get => value;
                set {
                    this.value = value;
                    ValueItem.Text = value.ToString();
                }
            }

            public MenuEntry(
                string name, SpriteFont font,
                Texture2D arrowLeftTexture,
                Texture2D arrowRightTexture,
                object value = null)
            {
                ArrowLeft = new MenuItem(arrowLeftTexture, 16, 16);
                ArrowRight = new MenuItem(arrowRightTexture, 16, 16);
                NameItem = new MenuItem(name, font);
                ValueItem = new TextItem(font);
                NamePane = new HPane(NameItem);
                ValuePane = new HPane(ArrowLeft, ValueItem, ArrowRight);

                Value = value;
                ValuePane.HGrow = 1;
                ValueItem.HAlign = HAlignment.Center;
                ArrowLeft.HAlign = HAlignment.Left;
                ArrowRight.HAlign = HAlignment.Right;
            }
        }

        private SpriteFont font;
        private Texture2D blank;
        private Texture2D arrRight, arrLeft;
        private ImageItem background;

        private Minesweeper game;
        private SortedDictionary<string, MenuEntry> entries;
        private Dictionary<string, string> values;

        public SettingsView(GameView parent, Minesweeper game) : base(parent) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            arrLeft = Content.Load<Texture2D>("textures/arrow_left");
            arrRight = Content.Load<Texture2D>("textures/arrow_right");
            entries = new SortedDictionary<string, MenuEntry>();
            entries.Add("difficulty", new MenuEntry("Difficulty: ", font, arrLeft, arrRight, game.Settings.Difficulty));
            entries.Add("width", new MenuEntry("Map Width: ", font, arrLeft, arrRight, game.Settings.MapWidth));
            entries.Add("height", new MenuEntry("Map Height: ", font, arrLeft, arrRight, game.Settings.MapHeight));
            entries.Add("density", new MenuEntry("Mine Density: ", font, arrLeft, arrRight, game.Settings.MineDensitiy));
            entries.Add("audioVolume", new MenuEntry("Audio Volume: ", font, arrLeft, arrRight, game.Settings.AudioVolume));
            entries.Add("musicVolume", new MenuEntry("Music Volume: ", font, arrLeft, arrRight, game.Settings.MusicVolume));
            entries.Add("windowMode", new MenuEntry("Window Mode: ", font, arrLeft, arrRight, game.Settings.WindowMode));
            this.game = game;
            Init();
        }

        public override void Init() {
            background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.CornflowerBlue;
            background.Alpha = 0.5f;

            VPane vpNames = new VPane();
            VPane vpValues = new VPane();
            vpNames.HGrow = 1;
            vpValues.HGrow = 1;

            entries.Values.ToList().ForEach(entry => {
                vpNames.Add(entry.NamePane);
                vpValues.Add(entry.ValuePane);
            });

            ListMenu exitMenu = new ListMenu();
            MenuItem accept = new MenuItem("Accept", font);
            MenuItem back = new MenuItem("Back", font);
            MenuItem gap = new MenuItem("   ", font);
            gap.IsDisabled = true;

            exitMenu.HAlign = HAlignment.Center;
            exitMenu.VAlign = VAlignment.Center;
            exitMenu.AddItem(back);
            exitMenu.AddItem(gap);
            exitMenu.AddItem(accept);

            HPane hPane = new HPane(vpNames, vpValues);
            hPane.VAlign = VAlignment.Center;
            hPane.HGrow = 1;

            StackPane sPane = new StackPane(background, hPane);
            sPane.VAlign = VAlignment.Center;
            sPane.HGrow = sPane.VGrow = 1;

            VPane vPane = new VPane(sPane, exitMenu);
            vPane.HAlign = HAlignment.Center;
            vPane.HGrow = 0.5f;
            vPane.VGrow = 1;

            MainContainer.Add(vPane);

            accept.FocusGain += (s, a) => accept.Text.Color = Color.Yellow;
            accept.FocusLoss += (s, a) => accept.Text.Color = Color.White;
            accept.Action += (s, a) => {
                game.Settings = CreateGameSettings();
                FileManager.SaveSettings(Minesweeper.SETTINGS_PATH, game.Settings);
                game.ApplyWindowMode(game.Settings.WindowMode);
                Close();
            };

            back.FocusGain += (s, a) => back.Text.Color = Color.Yellow;
            back.FocusLoss += (s, a) => back.Text.Color = Color.White;
            back.Action += (s, a) => Close();

            entries["difficulty"].ArrowLeft.Action += (s, a) => {
                entries["difficulty"].Value = (MapDifficulty)(Math.Max(0, (((int)entries["difficulty"].Value)-1)));
                SetupDifficulty((MapDifficulty)entries["difficulty"].Value);
            };

            entries["difficulty"].ArrowRight.Action += (s, a) => {
                entries["difficulty"].Value = (MapDifficulty)(Math.Min(2, (((int)entries["difficulty"].Value)+1)));
                SetupDifficulty((MapDifficulty)entries["difficulty"].Value);
            };

            entries["width"].ArrowLeft.Action += (s, a) => {
                if((int)entries["width"].Value > 9) {
                    entries["width"].Value = (int)entries["width"].Value - 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["width"].ArrowRight.Action += (s, a) => {
                if((int)entries["width"].Value < 30) {
                    entries["width"].Value = (int)entries["width"].Value + 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["height"].ArrowLeft.Action += (s, a) => {
                if((int)entries["height"].Value > 9) {
                    entries["height"].Value = (int)entries["height"].Value - 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["height"].ArrowRight.Action += (s, a) => {
                if((int)entries["height"].Value < 24) {
                    entries["height"].Value = (int)entries["height"].Value + 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["density"].ArrowLeft.Action += (s, a) => {
                if((int)entries["density"].Value > 12) {
                    entries["density"].Value = (int)entries["density"].Value - 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["density"].ArrowRight.Action += (s, a) => {
                if((int)entries["density"].Value < 93) {
                    entries["density"].Value = (int)entries["density"].Value + 1;
                    SetupDifficulty(MapDifficulty.Custom);
                }
            };

            entries["audioVolume"].ArrowLeft.Action += (s, a) => entries["audioVolume"].Value = Math.Max(0, (int)entries["audioVolume"].Value-10);
            entries["audioVolume"].ArrowRight.Action += (s, a) => entries["audioVolume"].Value = Math.Min(100, (int)entries["audioVolume"].Value+10);

            entries["musicVolume"].ArrowLeft.Action += (s, a) => entries["musicVolume"].Value = Math.Max(0, (int)entries["musicVolume"].Value-10);
            entries["musicVolume"].ArrowRight.Action += (s, a) => entries["musicVolume"].Value = Math.Min(100, (int)entries["musicVolume"].Value+10);

            entries["windowMode"].ArrowLeft.Action += (s, a) => entries["windowMode"].Value = WindowMode.Windowed;
            entries["windowMode"].ArrowRight.Action += (s, a) => entries["windowMode"].Value = WindowMode.Fullscreen;
        }

        public GameSettings CreateGameSettings() {
            GameSettings settings = new GameSettings(
                (MapDifficulty)entries["difficulty"].Value,
                (int)entries["width"].Value,
                (int)entries["height"].Value,
                (int)entries["density"].Value,
                (int)entries["audioVolume"].Value,
                (int)entries["musicVolume"].Value,
                (WindowMode)entries["windowMode"].Value);

            if(game.Settings != null)
                settings.LastScore = game.Settings.LastScore;
                
            return settings;
        }

        private void SetupDifficulty(MapDifficulty d) {
            switch(d) {
                case MapDifficulty.Easy:
                    entries["width"].Value = 9;
                    entries["height"].Value = 9;
                    entries["density"].Value = 12;
                    break;
                case MapDifficulty.Medium:
                    entries["width"].Value = 16;
                    entries["height"].Value = 16;
                    entries["density"].Value = 16;
                    break;
                case MapDifficulty.Hard:
                    entries["width"].Value = 30;
                    entries["height"].Value = 16;
                    entries["density"].Value = 21;
                    break;
                default:
                    int width = (int)entries["width"].Value;
                    int height = (int)entries["height"].Value;
                    int density = (int)entries["density"].Value;

                    entries["difficulty"].Value = width == 9 && height == 9 && density == 12 ? MapDifficulty.Easy
                        : width == 16 && height == 16 && density == 16 ? MapDifficulty.Medium
                        : width == 30 && height == 16 && density == 21 ? MapDifficulty.Hard : d;

                    break;
            }
        }
    }
}
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using System.Collections.Generic;
using System.Linq;
using System;

using Chaotx.Mgx.Control.Menu;
using Chaotx.Mgx.Control;
using Chaotx.Mgx.Layout;

namespace Chaotx.Minesweeper {
    public class HighscoreView : GameView {
        public int EntriesPerPage {get; set;} = 10;

        private Minesweeper game;
        private SpriteFont font;
        private Texture2D blank;
        private Texture2D arrLeft;
        private Texture2D arrRight;
        private Dictionary<MapDifficulty, TabPane> tabMap;
        private Dictionary<MapDifficulty, int> selMap;

        public HighscoreView(Minesweeper game) : base(game) {
            font = Content.Load<SpriteFont>("fonts/menu_font");
            blank = Content.Load<Texture2D>("textures/blank");
            arrLeft = Content.Load<Texture2D>("textures/arrow_left");
            arrRight = Content.Load<Texture2D>("textures/arrow_right");
            Media.AddSong("audio/songs/menu_theme_0");
            Media.AddSong("audio/songs/menu_theme_1");
            this.game = game;
            Init();
        }

        public override void Init() {
            tabMap = new Dictionary<MapDifficulty, TabPane>();
            selMap = new Dictionary<MapDifficulty, int>();
            MenuItem leftDiff = new MenuItem(arrLeft, 32, 32);
            MenuItem rightDiff = new MenuItem(arrRight, 32, 32);
            MenuItem leftPage = new MenuItem(arrLeft, 32, 32);
            MenuItem rightPage = new MenuItem(arrRight, 32, 32);
            MenuItem back = new MenuItem("Back", font);

            leftDiff.VAlign = rightDiff.VAlign = VAlignment.Center;
            leftDiff.HAlign = rightDiff.HAlign = HAlignment.Center;
            leftPage.VAlign = rightPage.VAlign = VAlignment.Center;
            leftPage.HAlign = rightPage.HAlign = HAlignment.Center;
            back.HAlign = HAlignment.Center;
            back.VAlign = VAlignment.Top;

            TextItem diff = new TextItem(font, "Difficulty: " + game.Settings.Difficulty);
            diff.HAlign = HAlignment.Center;
            diff.VAlign = VAlignment.Center;

            HPane hDiff = new HPane(diff);
            hDiff.HAlign = HAlignment.Center;
            hDiff.HGrow = 0.4f;

            HPane hDiffArr = new HPane(leftDiff, hDiff, rightDiff);
            hDiffArr.HGrow = 1;

            tabMap.Add(MapDifficulty.Easy, CreateScoresPane(game.Scores.Where(s => s.Settings.Difficulty == MapDifficulty.Easy).ToList()));
            tabMap.Add(MapDifficulty.Medium, CreateScoresPane(game.Scores.Where(s => s.Settings.Difficulty == MapDifficulty.Medium).ToList()));
            tabMap.Add(MapDifficulty.Hard, CreateScoresPane(game.Scores.Where(s => s.Settings.Difficulty == MapDifficulty.Hard).ToList()));
            tabMap.Add(MapDifficulty.Custom, CreateScoresPane(game.Scores.Where(s => s.Settings.Difficulty == MapDifficulty.Custom).ToList()));

            TabPane tPane = new TabPane(
                tabMap[MapDifficulty.Easy],
                tabMap[MapDifficulty.Medium],
                tabMap[MapDifficulty.Hard],
                tabMap[MapDifficulty.Custom]);

            ImageItem background = new ImageItem(blank);
            background.HGrow = background.VGrow = 1;
            background.Color = Color.CornflowerBlue;
            background.Alpha = 0.5f;

            StackPane sPane = new StackPane(background, tPane);
            sPane.HAlign = HAlignment.Center;
            sPane.HGrow = sPane.VGrow = 1;
            tPane.HGrow = tPane.VGrow = 1;

            HPane hPane = new HPane(leftPage, sPane, rightPage);
            hPane.HAlign = HAlignment.Center;
            hPane.HGrow = hPane.VGrow = 1;

            VPane vPane = new VPane(hDiffArr, hPane, back);
            vPane.HGrow = vPane.VGrow = 1;

            leftDiff.FocusGain += (s, a) => leftDiff.Image.Color = Color.Yellow;
            leftDiff.FocusLoss += (s, a) => leftDiff.Image.Color = Color.White;
            rightDiff.FocusGain += (s, a) => rightDiff.Image.Color = Color.Yellow;
            rightDiff.FocusLoss += (s, a) => rightDiff.Image.Color = Color.White;

            leftPage.FocusGain += (s, a) => leftPage.Image.Color = Color.Yellow;
            leftPage.FocusLoss += (s, a) => leftPage.Image.Color = Color.White;
            rightPage.FocusGain += (s, a) => rightPage.Image.Color = Color.Yellow;
            rightPage.FocusLoss += (s, a) => rightPage.Image.Color = Color.White;

            MapDifficulty selectedDiff = game.Settings.Difficulty;
            for(int i = 0; i < (int)selectedDiff; ++i) tPane.NextTab();

            leftDiff.IsDisabled = (int)(selectedDiff) == 0;
            rightDiff.IsDisabled = (int)(selectedDiff) == 3;
            UpdateArrow(leftDiff);
            UpdateArrow(rightDiff);

            leftDiff.Action += (s, a) => {
                if(tPane.PrevTab()) --selectedDiff;
                leftDiff.IsDisabled = (int)(selectedDiff) == 0;
                rightDiff.IsDisabled= (int)(selectedDiff) == 3;
                diff.Text = "Difficulty: " + (MapDifficulty)selectedDiff;
                UpdateArrow(leftDiff);
                UpdateArrow(rightDiff);

                int sel = selMap[selectedDiff];
                rightPage.IsDisabled = sel >= PageCount(selectedDiff)-1;
                leftPage.IsDisabled = sel <= 0;
                UpdateArrow(leftPage);
                UpdateArrow(rightPage);
            };

            rightDiff.Action += (s, a) => {
                if(tPane.NextTab()) ++selectedDiff;
                leftDiff.IsDisabled = (int)(selectedDiff) == 0;
                rightDiff.IsDisabled= (int)(selectedDiff) == 3;
                diff.Text = "Difficulty: " + (MapDifficulty)selectedDiff;
                UpdateArrow(leftDiff);
                UpdateArrow(rightDiff);

                int sel = selMap[selectedDiff];
                rightPage.IsDisabled = sel >= PageCount(selectedDiff)-1;
                leftPage.IsDisabled = sel <= 0;
                UpdateArrow(leftPage);
                UpdateArrow(rightPage);
            };

            leftPage.IsDisabled = true;
            if(PageCount(selectedDiff) < 2) rightPage.IsDisabled = true;
            UpdateArrow(leftPage);
            UpdateArrow(rightPage);

            selMap.Add(MapDifficulty.Easy, 0);
            selMap.Add(MapDifficulty.Medium, 0);
            selMap.Add(MapDifficulty.Hard, 0);
            selMap.Add(MapDifficulty.Custom, 0);

            leftPage.Action += (s, a) => {
                int sel = (selMap[selectedDiff] = Math.Max(0, selMap[selectedDiff]-1));
                rightPage.IsDisabled = sel >= PageCount(selectedDiff)-1;
                leftPage.IsDisabled = sel <= 0;
                tabMap[selectedDiff].PrevTab();
                UpdateArrow(leftPage);
                UpdateArrow(rightPage);
            };

            rightPage.Action += (s, a) => {
                int sel = (selMap[selectedDiff] = Math.Max(PageCount(selectedDiff)-1, selMap[selectedDiff]+1));
                rightPage.IsDisabled = sel >= PageCount(selectedDiff)-1;
                leftPage.IsDisabled = sel <= 0;
                tabMap[selectedDiff].NextTab();
                UpdateArrow(leftPage);
                UpdateArrow(rightPage);
            };

            back.FocusGain += (s, a) => back.Text.Color = Color.Yellow;
            back.FocusLoss += (s, a) => back.Text.Color = Color.White;
            back.Action += (s, a) => Close();

            MainContainer.Clear();
            MainContainer.Add(vPane);
        }

        private TabPane CreateScoresPane(List<Highscore> scores) {
            int p = Int32.MaxValue;
            int i = 1;

            List<VPane> pages = new List<VPane>();
            VPane page = null;

            scores.ForEach(score => {
                if(p > EntriesPerPage) {
                    page = new VPane();
                    page.HGrow = page.VGrow = 1;
                    pages.Add(page);
                    p = 1;
                }

                TextItem index = new TextItem(font, i.ToString("D3") + ": ");
                TextItem diff = new TextItem(font, score.Settings.Difficulty.ToString());
                TextItem name = new TextItem(font, score.Name);
                TextItem time = new TextItem(font, score.Time.ToString(@"hh\:mm\:ss\.ff"));
                TextItem hits = new TextItem(font, score.MinesHit.ToString("D2"));
                TextItem total = new TextItem(font, "/" + score.TotalMines.ToString("D2"));
                index.HAlign = diff.HAlign = name.HAlign = time.HAlign = hits.HAlign = total.HAlign = HAlignment.Center;

                HPane hIndex = new HPane(index);
                HPane hDiff = new HPane(diff);
                HPane hName = new HPane(name);
                HPane hTime = new HPane(time);
                HPane hHits = new HPane(hits, total);
                hDiff.HGrow = hName.HGrow = hTime.HGrow = hHits.HGrow = 2;
                hIndex.HGrow = 1;

                HPane hPane = new HPane(hIndex, hDiff, hName, hTime, hHits);
                hPane.VAlign = VAlignment.Center;
                hPane.HGrow = 1;
                page.Add(hPane);
                ++p;
                ++i;
            });

            TabPane tPane = new TabPane(pages.ToArray());
            tPane.HGrow = tPane.VGrow = 1;
            return tPane;
        }

        private void UpdateArrow(MenuItem arrow) {
            if(arrow.IsDisabled) {
                arrow.Image.Color = Color.Gray;
                arrow.Image.Alpha = 0.8f;
            } else {
                arrow.Image.Color = Color.White;
                arrow.Image.Alpha = 1;
            }
        }

        private int PageCount(MapDifficulty difficulty) {
            return tabMap[difficulty].Tabs.Count;
        }
    }
}
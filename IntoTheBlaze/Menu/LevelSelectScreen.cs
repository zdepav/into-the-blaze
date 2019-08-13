using System.Collections.Generic;
using System.IO;
using IntoTheBlaze.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace IntoTheBlaze.Menu {

    public class LevelSelectScreen : IGameScreen {

        private List<BigFlameParticle> tinyFire;
        private IntoTheBlaze game;
        private Texture2D background, levelButtons, resetButton, resetButtonActive, reallyButton;
        private MenuScreen menu;
        private Vector2[] levelButtonPositions;
        private int mouseOnButton, levelCount;
        private long progress_prg, progress_rss, progress_oe;
        private bool resetingProgress;

        internal int Progress {
            get => (int)(progress_prg % 317 % 53 / 3);
            set {
                Settings.Default.prg = progress_prg =
                    MathHelper.Clamp(value, 0, 16) * 3 +
                    Utils.Random.Next(1, 6) * 53 +
                    Utils.Random.Next(1, 3154) * 317;
                Settings.Default.rss = progress_rss = Utils.Random.Next(1, 121) * 521 - MathHelper.Clamp(value, 0, 16);
                Settings.Default.oe = progress_oe = progress_prg ^ ~progress_rss;
                Settings.Default.Save();
            }
        }

        private readonly Vector2[] tinyFireLocations = {
            // back button
            new Vector2(386, 548),
            new Vector2(638, 592),
            new Vector2(638, 548),
            new Vector2(386, 592),
            // reset button
            new Vector2(2, 594),
            new Vector2(254, 638),
            new Vector2(254, 594),
            new Vector2(2, 638)
        };

        public Song Music => menu.Music;

        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            menu = game.MenuScreen;
            tinyFire = new List<BigFlameParticle>();
            progress_prg = Settings.Default.prg;
            progress_rss = Settings.Default.rss;
            progress_oe = Settings.Default.oe;

            // check progress validity
            if (progress_oe != (progress_prg ^ ~progress_rss)) Progress = 0;
            else if (progress_prg < 370 || progress_prg > 999814) Progress = 0;
            else {
                var progc = progress_prg % 317;
                if (progc < 53 || progc > 313) Progress = 0;
                else {
                    progc = progc % 53;
                    if (progc > 48 || progc % 3 != 0) Progress = 0;
                    else if (progress_rss < 505 || progress_rss > 62520) Progress = 0;
                    else {
                        var progc2 = 521 - progress_rss % 521;
                        if (progc2 != progc / 3) Progress = 0;
                    }
                }
            }
            
            // compute button positions
            levelButtonPositions = new Vector2[16];
            for (var i = 0; i < 4; ++i)
            for (var j = 0; j < 4; ++j)
                levelButtonPositions[i * 4 + j] = new Vector2(292 + 120 * j, 60 + 120 * i);
            mouseOnButton = -1;

            levelCount = 0;
            for (var i = 1; i <= 16; ++i) {
                if (File.Exists($"Levels/{i}.level"))
                    ++levelCount;
                else break;
            }
        }

        public void LoadContent() {
            background = game.Content.Load<Texture2D>("level_select_background");
            resetButton = game.Content.Load<Texture2D>("button_reset_progress");
            resetButtonActive = game.Content.Load<Texture2D>("button_reset_progress_active");
            reallyButton = game.Content.Load<Texture2D>("button_really_active");
            levelButtons = game.Content.Load<Texture2D>("level_select_buttons");
        }

        public void Update(GameTime time) {
            mouseOnButton = -1;
            for (var i = 0; i < levelCount; ++i) {
                var pos = levelButtonPositions[i];
                if (MouseHelper.X > pos.X &&
                    MouseHelper.Y > pos.Y &&
                    MouseHelper.X < pos.X + 80 &&
                    MouseHelper.Y < pos.Y + 80
                ) {
                    mouseOnButton = i;
                    break;
                }
            }
            if (mouseOnButton < 0 &&
                MouseHelper.X > 384 &&
                MouseHelper.Y > 546 &&
                MouseHelper.X < 640 &&
                MouseHelper.Y < 594
            ) mouseOnButton = 16;
            if (mouseOnButton < 0 &&
                MouseHelper.X > 0 &&
                MouseHelper.Y > 592 &&
                MouseHelper.X < 256 &&
                MouseHelper.Y < 640
            ) mouseOnButton = 17;
            else resetingProgress = false;
            if (MouseHelper.LeftReleased) {
                if (mouseOnButton == 17) {
                    if (resetingProgress){
                        resetingProgress = false;
                        Progress = 0;
                    } else resetingProgress = true;
                } else if (mouseOnButton == 16) {
                    game.ChangeScreen(game.MenuScreen);
                } else if (mouseOnButton >= 0 && Progress >= mouseOnButton) {
                    var level = game.LevelScreen.LoadLevel(mouseOnButton + 1);
                    if (level != null) {
                        game.ChangeScreen(level);
                        return;
                    }
                }
            }
            
            var steps = (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;
            tinyFire.ForEach(p => p.Update(steps));
            tinyFire.RemoveAll(p => !p.IsAlive);

            if (mouseOnButton < 17) {
                var loc = tinyFireLocations[Utils.Random.Next(4) + 4];
                var tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
                tinyPart.Initialize(loc.X, loc.Y, Utils.Random);
                tinyFire.Add(tinyPart);
            }
            if (mouseOnButton != 16) {
                var loc = tinyFireLocations[Utils.Random.Next(4)];
                var tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
                tinyPart.Initialize(loc.X, loc.Y, Utils.Random);
                tinyFire.Add(tinyPart);
            }
            if (mouseOnButton == 16 || (mouseOnButton >= 0 && Progress >= mouseOnButton)) return;
            var _tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
            _tinyPart.Initialize(MouseHelper.X, MouseHelper.Y, Utils.Random);
            tinyFire.Add(_tinyPart);
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(background, Vector2.Zero, Color.White);
            var prog = Progress;
            for (var i = 0; i < levelCount; ++i) {
                var variant = 0;
                if (prog < i)
                    variant = 2;
                else if (mouseOnButton == i)
                    variant = 1;
                batch.Draw(
                    levelButtons,
                    levelButtonPositions[i],
                    new Rectangle(i * 80, variant * 80, 80, 80),
                    Color.White
                );
            }
            batch.Draw(
                mouseOnButton == 16 ? menu.backButtonActive : menu.backButton,
                new Vector2(384, 546),
                Color.White
            );
            batch.Draw(
                mouseOnButton == 17 ? (resetingProgress ? reallyButton : resetButtonActive) : resetButton,
                new Vector2(0, 592),
                Color.White
            );
            batch.Draw(
                menu.cursor,
                MouseHelper.Pos,
                new Rectangle(0, 0, 16, 16),
                Color.White,
                (
                    time.TotalGameTime.Milliseconds / 1000f +
                    (time.TotalGameTime.Seconds % 2 == 1 ? 1 : 0)
                ) *
                MathHelper.Pi,
                new Vector2(8),
                1, SpriteEffects.None, 0);
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            tinyFire.ForEach(p => p.Draw(batch));
            batch.End();
        }

        public void Enter(IGameScreen previous) {
            if (previous != game.MenuScreen) {
                MediaPlayer.Play(menu.Music);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void Leave(IGameScreen next) {
            if (next != game.MenuScreen)
                MediaPlayer.Stop();
            tinyFire.Clear();
        }
    }
}

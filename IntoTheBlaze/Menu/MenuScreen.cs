using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace IntoTheBlaze.Menu {

    public class MenuScreen : IGameScreen {

        private List<BigFlameParticle> fire, tinyFire;

        private IntoTheBlaze game;

        internal Texture2D
            background,
            flameParts,
            startButton,
            startButtonActive,
            creditsButton,
            creditsButtonActive,
            endButton,
            endButtonActive,
            backButton,
            backButtonActive,
            cursor;

        private float timer;
        private bool mouseOnStartButton, mouseOnCreditsButton, mouseOnEndButton;

        private readonly Vector2[] tinyFireLocations = {
            // start button
            new Vector2(386, 202),
            new Vector2(638, 246),
            new Vector2(638, 202),
            new Vector2(386, 246),
            // credits button
            new Vector2(386, 298),
            new Vector2(638, 342),
            new Vector2(638, 298),
            new Vector2(386, 342),
            // end button
            new Vector2(386, 394),
            new Vector2(638, 438),
            new Vector2(638, 394),
            new Vector2(386, 438)
        };

        public Song Music { get; private set; }

        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            fire = new List<BigFlameParticle>();
            tinyFire = new List<BigFlameParticle>();
            mouseOnStartButton = mouseOnCreditsButton = mouseOnEndButton = false;
        }

        public void LoadContent() {
            background = game.Content.Load<Texture2D>("menu_background");
            flameParts = game.Content.Load<Texture2D>("fire_parts_big");
            startButton = game.Content.Load<Texture2D>("button_start");
            startButtonActive = game.Content.Load<Texture2D>("button_start_active");
            creditsButton = game.Content.Load<Texture2D>("button_credits");
            creditsButtonActive = game.Content.Load<Texture2D>("button_credits_active");
            endButton = game.Content.Load<Texture2D>("button_end");
            endButtonActive = game.Content.Load<Texture2D>("button_end_active");
            backButton = game.Content.Load<Texture2D>("button_back");
            backButtonActive = game.Content.Load<Texture2D>("button_back_active");
            cursor = game.Content.Load<Texture2D>("menu_cursor");
            Music = game.Content.Load<Song>("welcome_to_chaos");
        }

        public void Update(GameTime time) {
            timer += (float)time.ElapsedGameTime.TotalMilliseconds;
            var steps = (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;

            fire.ForEach(p => p.Update(steps));
            fire.RemoveAll(p => !p.IsAlive);
            while (timer > 1) {
                var part = new BigFlameParticle(flameParts);
                part.Initialize(Utils.Random.Next(1024), 640, Utils.Random);
                fire.Add(part);
                --timer;
            }

            tinyFire.ForEach(p => p.Update(steps));
            tinyFire.RemoveAll(p => !p.IsAlive);
            for (var i = 0; i < 3; ++i) {
                var index = Utils.Random.Next(tinyFireLocations.Length);
                if ((mouseOnStartButton && index < 4) ||
                    (mouseOnCreditsButton && index >= 4 && index < 8) ||
                    (mouseOnEndButton && index >= 8)
                ) continue;
                var loc = tinyFireLocations[index];
                var tinyPart = new BigFlameParticle(flameParts, 0.25f, 0.075f);
                tinyPart.Initialize(loc.X, loc.Y, Utils.Random);
                tinyFire.Add(tinyPart);
            }
            mouseOnStartButton = mouseOnCreditsButton = mouseOnEndButton = false;
            if (MouseHelper.X > 384 && MouseHelper.X < 640) {
                if (MouseHelper.Y > 200 && MouseHelper.Y < 248) {
                    mouseOnStartButton = true;
                } else if (MouseHelper.Y > 296 && MouseHelper.Y < 344) {
                    mouseOnCreditsButton = true;
                } else if (MouseHelper.Y > 392 && MouseHelper.Y < 440) {
                    mouseOnEndButton = true;
                }
            }

            if (MouseHelper.LeftReleased) {
                if (mouseOnStartButton) {
                    game.ChangeScreen(game.LevelSelectScreen);
                } else if (mouseOnCreditsButton) {
                    game.ChangeScreen(game.CreditsScreen);
                } else if (mouseOnEndButton) game.Exit();
            }

            if (mouseOnStartButton || mouseOnCreditsButton || mouseOnEndButton) return;
            var _tinyPart = new BigFlameParticle(flameParts, 0.25f, 0.075f);
            _tinyPart.Initialize(MouseHelper.X, MouseHelper.Y, Utils.Random);
            tinyFire.Add(_tinyPart);
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(background, Vector2.Zero, Color.White);
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            fire.ForEach(p => p.Draw(batch));
            batch.End();
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(
                mouseOnStartButton ? startButtonActive : startButton,
                new Vector2(384, 200),
                Color.White
            );
            batch.Draw(
                mouseOnCreditsButton ? creditsButtonActive : creditsButton,
                new Vector2(384, 296),
                Color.White
            );
            batch.Draw(
                mouseOnEndButton ? endButtonActive : endButton,
                new Vector2(384, 392),
                Color.White
            );
            batch.Draw(
                cursor,
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
            if (previous != game.LevelSelectScreen && previous != game.CreditsScreen) {
                MediaPlayer.Play(Music);
                MediaPlayer.IsRepeating = true;
            }
        }

        public void Leave(IGameScreen next) {
            if (next != game.LevelSelectScreen && next != game.CreditsScreen)
                MediaPlayer.Stop();
            fire.Clear();
            tinyFire.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using IntoTheBlaze.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace IntoTheBlaze.Game {

    public class LevelEndScreen : IGameScreen {

        private List<BigFlameParticle> tinyFire;
        private IntoTheBlaze game;

        private Texture2D
            winBackground,
            failBackground,
            continueButton,
            continueButtonActive;

        private Song winSound, failSound;

        private bool mouseOnContinueButton;
        private bool won;
        private MenuScreen menu;
        private LevelScreen level;

        private readonly Vector2[] tinyFireLocations = {
            // continue button
            new Vector2(386, 458),
            new Vector2(638, 502),
            new Vector2(638, 458),
            new Vector2(386, 502)
        };

        public Song Music => won ? winSound : failSound;

        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            menu = game.MenuScreen;
            level = game.LevelScreen;
            tinyFire = new List<BigFlameParticle>();
            mouseOnContinueButton = false;
        }

        public void LoadContent() {
            winBackground = game.Content.Load<Texture2D>("win_background");
            failBackground = game.Content.Load<Texture2D>("fail_background");
            continueButton = game.Content.Load<Texture2D>("button_continue");
            continueButtonActive = game.Content.Load<Texture2D>("button_continue_active");
            winSound = game.Content.Load<Song>("snd_success");
            failSound = game.Content.Load<Song>("snd_boo");
        }

        public void Update(GameTime time) {
            mouseOnContinueButton = false;
            mouseOnContinueButton =
                MouseHelper.X > 384 &&
                MouseHelper.Y > 456 &&
                MouseHelper.X < 640 &&
                MouseHelper.Y < 504;

            if (MouseHelper.LeftReleased && mouseOnContinueButton) {
                game.ChangeScreen(game.LevelSelectScreen);
            }
            
            var steps = (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;
            tinyFire.ForEach(p => p.Update(steps));
            tinyFire.RemoveAll(p => !p.IsAlive);

            if (mouseOnContinueButton) return;

            var loc = tinyFireLocations[Utils.Random.Next(tinyFireLocations.Length)];
            var tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
            tinyPart.Initialize(loc.X, loc.Y, Utils.Random);
            tinyFire.Add(tinyPart);
            if (!won) {
                tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
                tinyPart.Initialize(MouseHelper.X, MouseHelper.Y, Utils.Random);
                tinyFire.Add(tinyPart);
            }
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(won ? winBackground : failBackground, Vector2.Zero, Color.White);

            batch.Draw(
                mouseOnContinueButton ? continueButtonActive : continueButton,
                new Vector2(384, 456),
                Color.White
            );
            batch.Draw(
                won ? level.cursor : menu.cursor,
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
            MediaPlayer.Play(Music);
            MediaPlayer.IsRepeating = true;
        }

        public void Leave(IGameScreen next) => tinyFire.Clear();
        
        /// <returns>this</returns>
        public LevelEndScreen SetWin(bool won) {
            this.won = won;
            return this;
        }
    }
}

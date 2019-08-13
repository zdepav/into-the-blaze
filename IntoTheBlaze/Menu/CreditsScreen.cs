using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace IntoTheBlaze.Menu {

    public class CreditsScreen : IGameScreen {

        private List<BigFlameParticle> tinyFire;
        private IntoTheBlaze game;
        private Texture2D background;
        private MenuScreen menu;
        private Tuple<Rectangle, string>[] links;
        private bool mouseOnBackButton;

        private readonly Vector2[] tinyFireLocations = {
            // back button
            new Vector2(386, 546),
            new Vector2(638, 590),
            new Vector2(638, 546),
            new Vector2(386, 590)
        };

        public Song Music => game.MenuScreen.Music;

        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            menu = game.MenuScreen;
            tinyFire = new List<BigFlameParticle>();
            links = new[] {
                new Tuple<Rectangle, string>(
                    new Rectangle(379, 124, 253, 31),
                    "https://zdepav.cz"),
                new Tuple<Rectangle, string>(
                    new Rectangle(354, 254, 193, 34),
                    "https://www.youtube.com/watch?v=q5w5VX4tAD4"),
                new Tuple<Rectangle, string>(
                    new Rectangle(375, 294, 150, 27),
                    "http://teknoaxe.com/Link_Code_3.php?q=1292"),
                new Tuple<Rectangle, string>(
                    new Rectangle(302, 413, 407, 28),
                    "https://www.freesoundeffects.com")
            };
            mouseOnBackButton = false;
        }

        public void LoadContent() {
            background = game.Content.Load<Texture2D>("credits_background");
        }

        public void Update(GameTime time) {
            mouseOnBackButton = false;
            mouseOnBackButton =
                MouseHelper.X > 384 &&
                MouseHelper.Y > 544 &&
                MouseHelper.X < 640 &&
                MouseHelper.Y < 592;

            if (MouseHelper.LeftReleased) {
                if (mouseOnBackButton) {
                    game.ChangeScreen(game.MenuScreen);
                } else {
                    foreach (var link in links) {
                        if (MouseHelper.X > link.Item1.Left &&
                            MouseHelper.Y > link.Item1.Top &&
                            MouseHelper.X < link.Item1.Right &&
                            MouseHelper.Y < link.Item1.Bottom
                        ) System.Diagnostics.Process.Start(link.Item2);
                    }
                }
            }
            
            var steps = (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;
            tinyFire.ForEach(p => p.Update(steps));
            tinyFire.RemoveAll(p => !p.IsAlive);

            if (mouseOnBackButton) return;

            var loc = tinyFireLocations[Utils.Random.Next(tinyFireLocations.Length)];
            var tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
            tinyPart.Initialize(loc.X, loc.Y, Utils.Random);
            tinyFire.Add(tinyPart);
            tinyPart = new BigFlameParticle(menu.flameParts, 0.25f, 0.075f);
            tinyPart.Initialize(MouseHelper.X, MouseHelper.Y, Utils.Random);
            tinyFire.Add(tinyPart);
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Begin(SpriteSortMode.Immediate);
            batch.Draw(background, Vector2.Zero, Color.White);

            batch.Draw(
                mouseOnBackButton ? menu.backButtonActive : menu.backButton,
                new Vector2(384, 544),
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
                ) * MathHelper.Pi,
                new Vector2(8),
                1, SpriteEffects.None, 0);
            batch.End();
            batch.Begin(SpriteSortMode.Immediate, BlendState.Additive);
            tinyFire.ForEach(p => p.Draw(batch));
            batch.End();
        }

        public void Enter(IGameScreen previous) {
            if (previous != game.MenuScreen){
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

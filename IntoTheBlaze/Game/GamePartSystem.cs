using System;
using System.Collections.Generic;
using System.Linq;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IntoTheBlaze.Game {

    public class GamePartSystem {

        private List<IParticle> parts;

        private IntoTheBlaze game;

        public void Initialize(IntoTheBlaze game) {
            this.game = game;
            parts = new List<IParticle>();
        }

        internal Texture2D
            firePart,
            firePart__,
            waterPart,
            waterPart__,
            fireworkSpark,
            smokePart;

        public void LoadContent() {
            firePart = game.Content.Load<Texture2D>("fire_parts");
            firePart__ = game.Content.Load<Texture2D>("fire_parts_ee");
            waterPart = game.Content.Load<Texture2D>("water_part");
            waterPart__ = game.Content.Load<Texture2D>("water_part_ee");
            fireworkSpark = game.Content.Load<Texture2D>("firework_spark");
            smokePart = game.Content.Load<Texture2D>("smoke");
        }

        public void Update(GameTime time) {
            var steps = (float)time.ElapsedGameTime.TotalMilliseconds / 100f * 6f;
            parts.ForEach(p => p.Update(steps));
            parts.RemoveAll(p => !p.IsAlive);
            if (KeyboardHelper.Key(Keys.RightShift) && KeyboardHelper.KeyPressed(Keys.T)) SwapTextures();
        }

        public void Draw(SpriteBatch batch, GameTime time) {
            batch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            foreach (var part in parts.Where(p => p.IsGlowing)) part.Draw(batch);
            batch.End();
            batch.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied);
            foreach (var part in parts.Where(p => !p.IsGlowing)) part.Draw(batch);
            batch.End();
        }

        public void AddFirePart(Vector2 pos, FireType type) {
            var part = new FirePart(firePart, type);
            part.Initialize(pos.X, pos.Y, Utils.Random);
            parts.Add(part);
        }

        public void AddExtAgentPart(Vector2 pos, float direction, ExtinguishingAgent type) {
            var part = new ExtAgentPart(waterPart, type, direction);
            part.Initialize(pos.X, pos.Y, Utils.Random);
            parts.Add(part);
        }

        public IEnumerable<ExtAgentPart> GetAgents() {
            foreach (var part in parts)
                if (part is ExtAgentPart e)
                    yield return e;
        }

        public void Explode(Point pos) {
            for (var i = 0; i < 8; ++i) {
                var part = new SmokePart(smokePart, 3f, 5f, Color.Yellow, Color.Red, 1f);
                part.Initialize(pos.X, pos.Y, Utils.Random);
                parts.Add(part);
            }
        }

        public void SwapTextures() {
            var t = firePart;
            firePart = firePart__;
            firePart__ = t;
            t = waterPart;
            waterPart = waterPart__;
            waterPart__ = t;
        }
    }

}

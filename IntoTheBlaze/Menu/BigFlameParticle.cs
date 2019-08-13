using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Menu {

    internal class BigFlameParticle : IParticle {

        private readonly Texture2D texture;

        private readonly float scale, speedScale;

        private int type;

        private float age, maxAge, x, y, rotation, dx, dy, speed, size, colorTint;

        public BigFlameParticle(Texture2D texture, float scale = 1f, float speedScale = 1f) {
            this.texture = texture;
            this.scale = scale;
            this.speedScale = speedScale;
        }

        public void Initialize(float x, float y, Random r) {
            age = 0;
            maxAge = r.Next(80, 101);
            this.x = x;
            this.y = y;
            type = r.Next(12);
            rotation = (float)(r.NextDouble() * 2 * Math.PI);
            var direction = (r.NextDouble() * 10 + 85) / 180 * Math.PI;
            dx = (float)Math.Cos(direction);
            dy = -(float)Math.Sin(direction);
            speed = (float)(r.NextDouble() * 4 + 1);
            size = (float)(r.NextDouble() * 0.5 + 0.5);
            colorTint = (float)(r.NextDouble() * 0.5);
        }

        public void Update(float steps) {
            if (!IsAlive) return;
            age += steps;
            x += dx * speed * steps * speedScale;
            y += dy * speed * steps * speedScale;
            size -= 0.005f * steps;
            speed -= 0.025f * steps;
            rotation += 0.035f * steps;
        }

        public void Draw(SpriteBatch batch) {
            if (!IsAlive) return;
            var coef = age / maxAge;
            var alpha = coef < 0.5f ? 1f : 1f - (coef - 0.5f) * 2f;
            batch.Draw(
                texture,
                new Vector2(x, y),
                new Rectangle(type * 64, 0, 64, 64),
                new Color(Color.Lerp(Color.Lerp(Color.Orange, Color.Red, coef), Color.White, colorTint), alpha),
                rotation,
                new Vector2(32, 32),
                size * scale,
                SpriteEffects.None, 0
            );
        }

        public bool IsAlive => age <= maxAge;

        public bool IsGlowing => true;
    }
}

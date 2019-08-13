using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Game {

    internal class SmokePart : IParticle {

        private readonly float size1, size2;

        private readonly Color color1, color2;

        private readonly float alpha;

        private readonly Texture2D texture;

        private float age, maxAge, x, y, rotation;

        public SmokePart(Texture2D texture, float size1, float size2, Color color1, Color color2, float alpha) {
            this.texture = texture;
            this.size1 = size1;
            this.size2 = size2;
            this.color1 = color1;
            this.color2 = color2;
            this.alpha = alpha;
        }

        public void Initialize(float x, float y, Random r) {
            age = 0;
            maxAge = r.Next(60, 81);
            this.x = x;
            this.y = y;
            rotation = (float)(r.NextDouble() * 2 * Math.PI);
        }

        public void Update(float steps) {
            if (!IsAlive) return;
            age += steps;
        }

        public void Draw(SpriteBatch batch) {
            if (!IsAlive) return;
            var coef = age / maxAge;
            batch.Draw(
                texture,
                new Vector2(x, y),
                null,
                new Color(Color.Lerp(color1, color2, coef), (1f - coef) * alpha),
                rotation,
                new Vector2(40, 40),
                MathHelper.Lerp(size1, size2, coef),
                SpriteEffects.None, 0
            );
        }

        public bool IsAlive => age <= maxAge;

        public bool IsGlowing => false;
    }
}
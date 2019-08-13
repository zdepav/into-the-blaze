using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Game {

    internal class FirePart : IParticle {

        private readonly Texture2D texture;

        private int type, img;

        private float age, maxAge, x, y, rotation, size;

        private readonly Color color1, color2;

        public FirePart(Texture2D texture, FireType type) {
            this.texture = texture;
            switch (type) {
                case FireType.A:
                    color1 = Color.SandyBrown;
                    color2 = Color.Red;
                    break;
                case FireType.B:
                    color1 = Color.LightCoral;
                    color2 = Color.Crimson;
                    break;
                case FireType.C:
                    color1 = Color.LightCyan;
                    color2 = Color.Red;
                    break;
                case FireType.D:
                    color1 = Color.White;
                    color2 = Color.LightBlue;
                    break;
                case FireType.K:
                    color1 = Color.Khaki;
                    color2 = Color.Crimson;
                    break;
                default:
                    color1 = Color.White;
                    color2 = Color.Black;
                    break;
            }
        }

        public void Initialize(float x, float y, Random r) {
            age = 0;
            maxAge = r.Next(60, 81);
            this.x = x;
            this.y = y;
            type = r.Next(12);
            img = r.Next(4);
            rotation = (float)(r.NextDouble() * 2 * Math.PI);
            size = (float)(r.NextDouble() * 0.5 + 0.5);
        }

        public void Update(float steps) {
            if (!IsAlive) return;
            age += steps;
            size -= 0.005f * steps;
            rotation += 0.035f * steps;
        }

        public void Draw(SpriteBatch batch) {
            if (!IsAlive) return;
            var coef = age / maxAge;
            var alpha = coef < 0.5f ? 1f : 1f - (coef - 0.5f) * 2f;
            batch.Draw(
                texture,
                new Vector2(x, y),
                new Rectangle(img * 12, 0, 12, 12),
                new Color(Color.Lerp(color1, color2, coef), alpha),
                rotation,
                new Vector2(6, 6),
                size,
                SpriteEffects.None, 0
            );
        }

        public bool IsAlive => age <= maxAge;

        public bool IsGlowing => true;
    }
}
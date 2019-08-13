using System;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IntoTheBlaze.Game {

    public class ExtAgentPart : IParticle {

        private readonly Texture2D texture;

        private float age, maxAge, x, y, dx, dy, speed, rotation, size, direction;
        
        public readonly ExtinguishingAgent Type;

        public ExtAgentPart(Texture2D texture, ExtinguishingAgent type, float direction) {
            this.texture = texture;
            this.direction = direction;
            Type = type;
        }

        public void Initialize(float x, float y, Random r) {
            age = 0;
            maxAge = r.Next(45, 61);
            this.x = x;
            this.y = y;
            speed = (float)(r.NextDouble() * 2 + 2);
            if (Type.PartVariant == 1) {
                speed *= 3f;
                maxAge /= 3f;
                direction = MathHelper.ToRadians(direction - 5 + r.Next(11));
            } else direction = MathHelper.ToRadians(direction - 15 + r.Next(31));
            dx = (float)Math.Cos(direction);
            dy = -(float)Math.Sin(direction);
            rotation = (float)(r.NextDouble() * 2 * Math.PI);
            size = (float)(r.NextDouble() * 0.5 + 0.5);
        }

        public void Update(float steps) {
            if (!IsAlive) return;
            age += steps;
            x += dx * speed * steps;
            y += dy * speed * steps;
            speed -= 0.025f * steps;
            size += 0.04f * steps;
            //rotation += 0.035f * steps;
        }

        public void Draw(SpriteBatch batch) {
            if (!IsAlive) return;
            var coef = age / maxAge;
            var alpha = coef < 0.5f ? 1f : 1f - (coef - 0.5f) * 2f;
            batch.Draw(
                texture,
                new Vector2(x, y),
                null,
                new Color(Type.PartColor, alpha),
                rotation,
                new Vector2(6, 6),
                size,
                SpriteEffects.None, 0
            );
        }

        public bool IsAlive => age <= maxAge;

        public bool IsGlowing => false;

        public IShape CollisionMask => new Circle((int)Math.Round(x), (int)Math.Round(y), (int)Math.Round(size * 6));

        public void Kill() => age = maxAge;
    }
}

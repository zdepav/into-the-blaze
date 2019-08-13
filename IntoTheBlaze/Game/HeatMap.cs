using System;
using Microsoft.Xna.Framework;

namespace IntoTheBlaze.Game {

    public class HeatMap {

        private readonly float[,] map, cooldown;

        public HeatMap() {
            map = new float[64, 34];
            cooldown = new float[64, 34];
        }

        public void UpdateBegin(GameTime time) {
            var t = 0.000025f * (float)time.ElapsedGameTime.TotalMilliseconds;
            for (var x = 0; x < 64; ++x)
            for (var y = 0; y < 34; ++y) {
                map[x, y] = 0;
                if (cooldown[x, y] < 1)
                    cooldown[x, y] = Math.Min(1, cooldown[x, y] + t);
            }
        }

        public void RadiateHeat(Point pos, float intensity) {
            var radius = Math.Min(intensity * 0.75f, 200f);
            var coef = intensity / radius;
            var left = Math.Max(0, (int)((pos.X - radius) / 16f));
            var right = Math.Min(63, (int)((pos.X + radius) / 16f));
            var top = Math.Max(0, (int)((pos.Y - radius) / 16f));
            var bottom = Math.Min(33, (int)((pos.Y + radius) / 16f));
            for (var x = left; x <= right; ++x)
            for (var y = top; y <= bottom; ++y) {
                var dist = Utils.Distance(x * 16 + 8, y * 16 + 8, pos.X, pos.Y);
                if (dist < radius)
                    map[x, y] += (radius - dist) * coef; // linear falloff
            }
        }

        public void Explode(Point pos) {
            var radius = 160f;
            var intensity = 1000f;
            var left = Math.Max(0, (int)((pos.X - radius) / 16f));
            var right = Math.Min(63, (int)((pos.X + radius) / 16f));
            var top = Math.Max(0, (int)((pos.Y - radius) / 16f));
            var bottom = Math.Min(33, (int)((pos.Y + radius) / 16f));
            for (var x = left; x <= right; ++x)
            for (var y = top; y <= bottom; ++y) {
                var dist = Utils.Distance(x * 16 + 8, y * 16 + 8, pos.X, pos.Y);
                if (dist < radius)
                    map[x, y] += Arc(dist / radius) * intensity; // arc falloff
            }
        }

        private float Arc(float f) => (float)Math.Sqrt(1 - f * f);
        
        public void CoolDown(Point pos) {
            var radius = 24f;
            var left = Math.Max(0, (int)((pos.X - radius) / 16f));
            var right = Math.Min(63, (int)((pos.X + radius) / 16f));
            var top = Math.Max(0, (int)((pos.Y - radius) / 16f));
            var bottom = Math.Min(33, (int)((pos.Y + radius) / 16f));
            for (var x = left; x <= right; ++x)
            for (var y = top; y <= bottom; ++y) {
                var dist = Utils.Distance(x * 16 + 8, y * 16 + 8, pos.X, pos.Y);
                if (dist < radius)
                    cooldown[x, y] = Math.Max(0f, cooldown[x, y] - (1f - dist / radius) * 0.1f); // linear falloff
            }
        }

        public float GetHeat(Point pos) {
            var x1 = MathHelper.Clamp((int)(pos.X / 16f), 0, 63);
            var x2 = MathHelper.Clamp((int)Math.Ceiling(pos.X / 16f), 0, 63);
            var y1 = MathHelper.Clamp((int)(pos.Y / 16f), 0, 33);
            var y2 = MathHelper.Clamp((int)Math.Ceiling(pos.Y / 16f), 0, 33);
            if (x2 == x1) {
                if (y2 == y1) {
                    return cooldown[x1, y1] * cooldown[x1, y1] * map[x1, y1];
                } else {
                    return MathHelper.Lerp(
                        cooldown[x1, y1] * cooldown[x1, y1] * map[x1, y1],
                        cooldown[x1, y2] * cooldown[x1, y2] * map[x1, y2],
                        pos.Y / 16f - y1
                    );
                }
            } else {
                var xcoef = pos.X / 16f - x1;
                if (y2 == y1) {
                    return MathHelper.Lerp(
                        cooldown[x1, y1] * cooldown[x1, y1] * map[x1, y1],
                        cooldown[x2, y1] * cooldown[x2, y1] * map[x2, y1],
                        xcoef
                    );
                } else {
                    return MathHelper.Lerp(
                        MathHelper.Lerp(
                            cooldown[x1, y1] * cooldown[x1, y1] * map[x1, y1],
                            cooldown[x2, y1] * cooldown[x2, y1] * map[x2, y1],
                            xcoef
                        ),
                        MathHelper.Lerp(
                            cooldown[x1, y2] * cooldown[x1, y2] * map[x1, y2],
                            cooldown[x2, y2] * cooldown[x2, y2] * map[x2, y2],
                            xcoef
                        ),
                        pos.Y / 16f - y1
                    );
                }
            }
        }
    }
}

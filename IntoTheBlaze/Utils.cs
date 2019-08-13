using System;
using IntoTheBlaze.Physics;
using Microsoft.Xna.Framework;

namespace IntoTheBlaze {

    internal static class Utils {

        public static Random Random = new Random();

        public static float Distance(float x, float y, Vector2 v) =>
            new Vector2(x - v.X, y - v.Y).Length();

        public static float Distance(float x1, float y1, float x2, float y2) =>
            new Vector2(x1 - x2, y1 - y2).Length();

        public static float Distance(Vector2 a, Vector2 b) => (b - a).Length();

        public static readonly Color DarkGray = new Color(32, 32, 32);

        public static bool CollidesWithEdge(IShape shape) {
            if (shape is Circle c) {
                return c.X < c.Radius || c.Y < c.Radius || c.X > 1024 - c.Radius || c.Y > 544 - c.Radius;
            } else if (shape is Rect r) {
                return r.X < r.Width / 2 || r.Y < r.Height / 2 || r.X > 1024 - r.Width / 2 || r.Y > 544 - r.Height / 2;
            } else return false;
        }
    }
}

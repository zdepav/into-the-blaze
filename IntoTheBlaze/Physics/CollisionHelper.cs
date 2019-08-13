using System;
using Microsoft.Xna.Framework;

namespace IntoTheBlaze.Physics {

    internal static class CollisionHelper {

        public static bool PointIn(Point p, Circle c) {
            var xd = c.X - p.X;
            var yd = c.Y - p.Y;
            return xd * xd + yd * yd < c.Radius;
        }

        public static bool PointIn(Point p, Rect r) {
            var xd = Math.Abs(p.X - r.X) * 2;
            var yd = Math.Abs(p.Y - r.Y) * 2;
            return xd < r.Width && yd < r.Height;
        }

        public static bool Collision(Circle c1, Circle c2) {
            var xd = c2.X - c1.X;
            var yd = c2.Y - c1.Y;
            var r = c1.Radius + c2.Radius;
            return xd * xd + yd * yd < r * r;
        }

        public static bool Collision(Rect r, Circle c) {
            // 3   1   4
            //   ┌───┐
            // 2 │ 0 │ 2
            //   └───┘
            // 5   1   6
            var xd = Math.Abs(r.X - c.X);
            var yd = Math.Abs(r.Y - c.Y);
            var w = r.Width / 2;
            var h = r.Height / 2;
            if (xd < w) return yd < h || yd < h + c.Radius; // 0 or 1
            if (yd < h && xd < w + c.Radius) return true;             // 2
            if (c.X < r.X) {
                return PointIn(
                    c.Y < r.Y
                        ? new Point(r.X - w, r.Y - h) // 3
                        : new Point(r.X - w, r.Y + h) // 5
                    , c
                );
            } else {
                return PointIn(
                    c.Y < r.Y
                        ? new Point(r.X + w, r.Y - h) // 4
                        : new Point(r.X + w, r.Y + h) // 6
                    , c
                );
            }
        }

        public static bool Collision(Circle c, Rect r) => Collision(r, c);

        public static bool Collision(Rect r1, Rect r2) {
            var xd = Math.Abs(r2.X - r1.X) * 2;
            var yd = Math.Abs(r2.Y - r1.Y) * 2;
            return xd < r1.Width + r2.Width && yd < r1.Height + r2.Height;
        }

        public static bool Collision(IShape s1, IShape s2) => s1.CollidesWith(s2);
    }
}

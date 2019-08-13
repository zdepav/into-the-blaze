namespace IntoTheBlaze.Physics {

    public class Rect : IShape {

        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public Rect BB => this;

        public Rect(int x, int y, int width, int height) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
        
        public bool CollidesWith(Rect r) => CollisionHelper.Collision(this, r);

        public bool CollidesWith(Circle c) => CollisionHelper.Collision(this, c);

        public bool CollidesWith(IShape s) => s.CollidesWith(this);
    }
}

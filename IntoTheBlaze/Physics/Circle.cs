namespace IntoTheBlaze.Physics {

    public class Circle : IShape {

        public int X { get; }

        public int Y { get; }

        public int Radius { get; }

        public Rect BB => new Rect(X, Y, Radius * 2, Radius * 2);

        public Circle(int x, int y, int radius) {
            X = x;
            Y = y;
            Radius = radius;
        }
        
        public bool CollidesWith(Rect r) => CollisionHelper.Collision(this, r);

        public bool CollidesWith(Circle c) => CollisionHelper.Collision(this, c);

        public bool CollidesWith(IShape s) => s.CollidesWith(this);
    }
}

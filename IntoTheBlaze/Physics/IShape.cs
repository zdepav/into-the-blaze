namespace IntoTheBlaze.Physics {

    public interface IShape {
        int X { get; }
        int Y { get; }
        Rect BB { get; }
        bool CollidesWith(Rect r);
        bool CollidesWith(Circle c);
        bool CollidesWith(IShape s);
    }
}

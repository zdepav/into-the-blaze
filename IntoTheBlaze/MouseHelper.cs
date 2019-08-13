using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IntoTheBlaze {

    internal static class MouseHelper {

        private static MouseState prevState, currentState;

        public static void Initialize() => prevState = currentState = Mouse.GetState();

        public static void Update() {
            prevState = currentState;
            currentState = Mouse.GetState();
        }

        public static bool LeftPressed =>
            currentState.LeftButton == ButtonState.Pressed &&
            prevState.LeftButton == ButtonState.Released;

        public static bool Left => currentState.LeftButton == ButtonState.Pressed;

        public static bool LeftReleased =>
            currentState.LeftButton == ButtonState.Released &&
            prevState.LeftButton == ButtonState.Pressed;

        public static bool MiddlePressed =>
            currentState.MiddleButton == ButtonState.Pressed &&
            prevState.MiddleButton == ButtonState.Released;

        public static bool Middle => currentState.MiddleButton == ButtonState.Pressed;

        public static bool MiddleReleased =>
            currentState.MiddleButton == ButtonState.Released &&
            prevState.MiddleButton == ButtonState.Pressed;

        public static bool RightPressed =>
            currentState.RightButton == ButtonState.Pressed &&
            prevState.RightButton == ButtonState.Released;

        public static bool Right => currentState.RightButton == ButtonState.Pressed;

        public static bool RightReleased =>
            currentState.RightButton == ButtonState.Released &&
            prevState.RightButton == ButtonState.Pressed;

        public static float X => currentState.X;
        public static float XDiff => currentState.X - prevState.X;

        public static float Y => currentState.Y;
        public static float YDiff => currentState.Y - prevState.Y;

        public static Vector2 Pos => currentState.Position.ToVector2();
        public static Vector2 PosDiff => (currentState.Position - prevState.Position).ToVector2();
    }
}

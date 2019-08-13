using Microsoft.Xna.Framework.Input;

namespace IntoTheBlaze {

    internal static class KeyboardHelper {

        private static KeyboardState prevState, currentState;

        public static void Initialize() => prevState = currentState = Keyboard.GetState();

        public static void Update() {
            prevState = currentState;
            currentState = Keyboard.GetState();
        }

        public static bool KeyPressed(Keys key) => currentState.IsKeyDown(key) && prevState.IsKeyUp(key);

        public static bool Key(Keys key) => currentState.IsKeyDown(key);

        public static bool KeyReleased(Keys key) => currentState.IsKeyUp(key) && prevState.IsKeyDown(key);
    }
}

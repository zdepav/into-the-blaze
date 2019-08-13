using System;

namespace IntoTheBlaze {
#if WINDOWS || LINUX
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new IntoTheBlaze())
                game.Run();
        }
    }
#endif
}

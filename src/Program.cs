using System.IO;
using System;

namespace Chaotx.Minestory {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new Minestory(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + Path.DirectorySeparatorChar + "chaotx"
                    + Path.DirectorySeparatorChar + "minesweeper"))
                        game.Run();
        }
    }
}

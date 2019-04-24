using System.IO;
using System;

namespace Chaotx.Minesweeper {
    public static class Program {
        [STAThread]
        static void Main() {
            using (var game = new Minesweeper(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + Path.DirectorySeparatorChar + "chaotx"
                    + Path.DirectorySeparatorChar + "minesweeper"))
                        game.Run();
        }
    }
}

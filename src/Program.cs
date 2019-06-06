using System.Threading.Tasks;
using System.IO;
using System;

namespace Chaotx.Minestory {
    public static class Program {
        [STAThread]
        static void Main(string[] args) {
            var cred = FileManager.GetCred();
            MySqlHelper.Init(cred[0], cred[1],
                cred[2], cred[3], cred[4]);

            using (var game = new Minestory(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + Path.DirectorySeparatorChar + "chaotx"
                    + Path.DirectorySeparatorChar + "minestory"))
                        game.Run();
        }
    }
}

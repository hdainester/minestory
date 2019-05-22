using System.Threading.Tasks;
using System.IO;
using System;

namespace Chaotx.Minestory {
    public static class Program {
        [STAThread]
        static void Main(string[] args) {
            if(args.Length == 0) try {
                DropboxConnect.AccessToken = File.ReadAllText("dbxacctok");
            } catch(Exception e) {
                Console.WriteLine("could not load dbxacctok: " + e.Message);
            } else if(args.Length == 1 && !args[0].Equals("offline"))
                DropboxConnect.AccessToken = args[0];

            Task task = DropboxConnect.Connect();
            while(!DropboxConnect.IsConnected
            && !DropboxConnect.ConnectionFailed);

            using (var game = new Minestory(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
                    + Path.DirectorySeparatorChar + "chaotx"
                    + Path.DirectorySeparatorChar + "minestory"))
                        game.Run();
            
            DropboxConnect.Dbx.Dispose();
            Console.WriteLine("Dbx disposed");
        }
    }
}

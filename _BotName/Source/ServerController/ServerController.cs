using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _BotName.source
{
    public class ServerController
    {
        public static ServerController Instance { get { return lazy.Value; } }
        private static readonly Lazy<ServerController> lazy =
            new Lazy<ServerController>(() => new ServerController());

        private Process processTTT;
        private Process processTerraria;

        public bool StartTTT()
        {
            processTTT = Start("");
            return processTTT == null;
        }

        public bool StopTTT()
        {
            if (processTTT == null)
                return false;

            try
            {
                processTTT.Kill();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine("Failed to stop TTT server (" + exception.ToString() + ")");
                return false;
            }
        }

        private Process Start(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            
            try
            {
                return Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to start path '" + path + "' " + "(" + e.ToString() + ")");
                return null;
            }
        }
    }
}

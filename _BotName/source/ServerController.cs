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
        static private ServerController instance;
        static public ServerController Instance
        {
            get
            {
                if (instance == null)
                    instance = new ServerController();

                return instance;
            }
        }

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

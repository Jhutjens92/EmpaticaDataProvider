using System.Threading;
using System.IO;
using System.Diagnostics;
using System;

namespace EmpaticaDataProvider.Classes
{
    class BLEServer
    {
        #region Variables
        private static readonly string exectuable = "EmpaticaBLEServer.exe";
        private static readonly string cPath1 = "C:/Program Files/Empatica/EmpaticaBLEServer";
        private static readonly string cPath2 = "C:/Program Files (x86)/Empatica/EmpaticaBLEServer";
        private static readonly string filename1 = Path.Combine(cPath1, exectuable);
        private static readonly string filename2 = Path.Combine(cPath2, exectuable);
        private static readonly string cParams = "a389709ed28b4138985821a5ac90c893 127.0.0.1 5555";
        #endregion

        private void StartBLEServer()
        {
            try
            {
                if (File.Exists(filename1)) { Process.Start(filename1, cParams); }
                else if (File.Exists(filename2)) { Process.Start(filename2, cParams); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Empatica BLE Server not started");
            }

        }

        public void CheckBLEServer()
        {
            Process[] empaticaBLEServer = Process.GetProcessesByName("EmpaticaBLEServer");
            if (empaticaBLEServer.Length == 0)
            {
                StartBLEServer();
                Thread.Sleep(2000);
            }
        }
    }
}

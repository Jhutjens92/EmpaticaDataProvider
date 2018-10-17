using System.Threading;
using System.IO;
using System.Diagnostics;


namespace EmpaticaDataProvider.Classes
{
    class BLEServer
    {
        #region Variables
        static readonly string cPath = "C:/Program Files (x86)/Empatica/EmpaticaBLEServer";
        static readonly string filename = Path.Combine(cPath, "EmpaticaBLEServer.exe");
        static readonly string cParams = "a389709ed28b4138985821a5ac90c893 127.0.0.1 5555";
        #endregion

        public static void StartBLEServer()
        {
            Process.Start(filename, cParams);
        }

        public static void CheckBLEServer()
        {
            Process[] pname = Process.GetProcessesByName("EmpaticaBLEServer");
            if (pname.Length == 0)
            {
                StartBLEServer();
            }
            Thread.Sleep(2000);
        }
    }
}

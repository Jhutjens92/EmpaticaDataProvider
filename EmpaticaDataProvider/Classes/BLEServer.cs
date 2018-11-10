using System.Threading;
using System.IO;
using System.Diagnostics;
using System;

namespace EmpaticaDataProvider.Classes
{
    /// <summary>   Main class to check and start and stop the Empatica BLE Server. </summary>
    ///
    /// <remarks>   Jordi Hutjens, 8-11-2018. </remarks>
     class BLEServer
    {

        #region Variables
        private readonly string filename1 = "C:/Program Files/Empatica/EmpaticaBLEServer/EmpaticaBLEServer.exe";
        private readonly string filename2 = "C:/Program Files (x86)/Empatica/EmpaticaBLEServer/EmpaticaBLEServer.exe";
        #endregion

        /// <summary>   Starts the Empatica BLE Server based on the startup parameters given. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 8-11-2018. </remarks>
        private void StartBLEServer()
        {
            string cParams = CheckParameters.Instance.ApiKey + " " + CheckParameters.Instance.ServerIP + " " + CheckParameters.Instance.ServerPort;
            try
            {
                if (File.Exists(filename1))
                {
                    //We are using this way of starting a new process instead of process.start(). 
                    //This way we can redirect the output of the Empatica BLE Server since we don't need it in our console.
                    Process p = new Process();
                    p.StartInfo.FileName = filename1;
                    p.StartInfo.Arguments = cParams;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                }
                else if (File.Exists(filename2))
                {
                    Process p = new Process();
                    p.StartInfo.FileName = filename2;
                    p.StartInfo.Arguments = cParams;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.Start();
                }
                Console.WriteLine("Started the Empatica BLE Server with the following values:  \n -API-Key: {0} \n -ServerIP: {1} \n -ServerPort: {2}\n", CheckParameters.Instance.ApiKey, CheckParameters.Instance.ServerIP, CheckParameters.Instance.ServerPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine("Empatica BLE Server not started.");
            }
        }

        /// <summary>   Checks if the Empatica BLE server is running. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 8-11-2018. </remarks>
        public  void CheckBLEServer()
        {
            Process[] empaticaBLEServer = Process.GetProcessesByName("EmpaticaBLEServer");
            if (empaticaBLEServer.Length == 0)
            {
                StartBLEServer();
                // We need to wait around 2 seconds. This ensures the Empatica BLE Server is started before trying to connect to it. 
                // We can only check if the Empatica BLE Server is running, however this does not ensure it is also ready.
                Thread.Sleep(2000);
            }
        }

        /// <summary>   Closes the Empatica BLE Server application. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 8-11-2018. </remarks>
        public  void CloseBLEServer()
        {
            try
            {
                Process[] empaticaBLEProcess = Process.GetProcessesByName("EmpaticaBLEServer");
                if (empaticaBLEProcess.Length > 0) { empaticaBLEProcess[0].Kill(); }
            }
            catch (Exception e)
            {
                Console.WriteLine("I got an exception after closing App " + e);
            }
        }
    }
}

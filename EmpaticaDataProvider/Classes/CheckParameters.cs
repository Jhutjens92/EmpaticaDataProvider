using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Classes
{

    /// <summary>   This class checks the startup parameters (Singleton). </summary>
    ///
    /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>

    public sealed class CheckParameters
    {

        #region Variables

        private static CheckParameters instance = null;

        private static readonly object padlock = new object();

        /// <summary>   Gets the datastream. </summary>
        ///
        /// <value> Datastream (acc/tmp/ibi/bvp/tag/gsr) </value>

        public string DataStream
        {
            get { return dataStream; }
        }
        private string dataStream;


        /// <summary>   Gets the API key. </summary>
        ///
        /// <value> The API key. </value>

        public string ApiKey
        {
            get { return apiKey; }
        }
        private string apiKey;

        /// <summary>   Gets the Server port. </summary>
        ///
        /// <value> The Server port. </value>

        public int ServerPort
        {
            get { return serverPort; }
        }
        private int serverPort;

        /// <summary>   Gets the Server IP. </summary>
        ///
        /// <value> The Server IP. </value>

        public string ServerIP
        {
            get { return serverIP; }
        }
        private string serverIP;

        /// <summary>  Get or set if the Learning Hub is running. </summary>
        ///
        /// <value> True if the Learning Hub is running. </value>
        public bool LHRunning
        {
            get { return lhRunning; }
            set { lhRunning = value; }
        }
        private bool lhRunning;


        /// <summary>   Checks if the Server IP is set by parameters. </summary>
        private bool sipPar = false;

        /// <summary>   Checks if the Server Port is set by parameters. </summary>
        private bool spPar = false;

        /// <summary>   Checks if the API key is set by parameters. Without this the program can't run.</summary>
        private bool akPar = false;

        ///// <summary>   Checks if the datastream is set by parameters. </summary>
        private bool dsPar = false;


        #endregion

        #region Methods

        /// <summary>Check startup parameters.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>

        public void CheckStartupParameters()
        {
            string[] StartupPar = Environment.GetCommandLineArgs();
            try
            {
                if (StartupPar.Any(s => s.Contains("-sp")))
                {
                    int ParIndex = Array.IndexOf(StartupPar, "-sp");
                    serverPort = Convert.ToInt32(StartupPar[ParIndex + 1]);
                    spPar = true;
                }
                if (StartupPar.Any(s => s.Contains("-ak")))
                {
                    int ParIndex = Array.IndexOf(StartupPar, "-ak");
                    apiKey = StartupPar[ParIndex + 1];
                    akPar = true;
                }
                if (StartupPar.Any(s => s.Contains("-sip")))
                {
                    int ParIndex = Array.IndexOf(StartupPar, "-sip");
                    serverIP = StartupPar[ParIndex + 1];
                    sipPar = true;
                }
                if (StartupPar.Any(s => s.Contains("-ds")))
                {
                    int ParIndex = Array.IndexOf(StartupPar, "-ds");
                    dataStream = StartupPar[ParIndex + 1];
                    dsPar = true;
                }
                else
                {
                    if (!dsPar)
                    {
                        dataStream = "acc";
                        Console.WriteLine("No datastream entered. Showing Accelerometer datastream (acc).");
                    }
                    if (!spPar)
                    {
                        serverPort = 5555;
                        Console.WriteLine("No server port entered. Starting with port: {0}", serverPort);
                    }
                    if (!akPar)
                    {
                        Console.WriteLine("No API key provided. The program can't function without a proper API-key provided by Empatica. Please see their website for more details");
                    }
                    if (!sipPar)
                    {
                        serverIP = "127.0.0.1";
                        Console.WriteLine("No server IP entered. Starting with localhost ({0})", serverIP);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            Console.WriteLine("\n The following parameters were detected: \n -API-Key: {0} \n -Server IP: {1} \n -Server port: {2} \n -Datastream: {3} \n", ApiKey, ServerIP, ServerPort, DataStream);
        }

        /// <summary>Singleton instance.</summary>
        ///
        /// <value>The instance.</value>

        public static CheckParameters Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CheckParameters();
                    }
                    return instance;
                }
            }
        }

        /// <summary>Constructor that prevents a default instance of this class from being created.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>

        CheckParameters()
        {

        }
        #endregion
    }
}

using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using EmpaticaDataProvider.ViewModel;


namespace EmpaticaDataProvider.EmpaticaManager
{
    class EmpaticaDataManager
    {
        #region Variables
        static readonly string cPath = "C:/Program Files (x86)/Empatica/EmpaticaBLEServer";
        static readonly string filename = Path.Combine(cPath, "EmpaticaBLEServer.exe");
        static readonly string cParams = "a389709ed28b4138985821a5ac90c893 127.0.0.1 5555";
        #endregion

        #region Events
        public class TagCreatedEventArgs : EventArgs
        {
            public int Tag { get; set; }
        }

        public class PPGSensorChangedEventArgs : EventArgs
        {
            public float BloodVolumePulse { get; set; }
        }

        public class IBISensorChangedEventArgs : EventArgs
        {
            public float InterBeatInterval { get; set; }
            public float HearthRateVariability { get; set; }
        }

        public class AccelerometerChangedEventArgs : EventArgs
        {
            public float AccelerometerX { get; set; }
            public float AccelerometerY { get; set; }
            public float AccelerometerZ { get; set; }
        }

        public class GSRSensorChangedEventArgs : EventArgs
        {
            public float GalvanicSkinResponse { get; set; }
        }

        public class TemperatureSensorChangedEventArgs : EventArgs
        {
            public float SkinTemperature { get; set; }
        }
                       
        public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;
        public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;
        public event EventHandler<PPGSensorChangedEventArgs> PPGSensorChanged;
        public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;
        public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;
        public event EventHandler<TagCreatedEventArgs> TagCreatedEvent;
   
        protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
        {
            AccelerometerChanged?.Invoke(this, e);
        }

        protected virtual void OnPPGSensorChanged(PPGSensorChangedEventArgs e)
        {
            PPGSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnIBISensorChanged(IBISensorChangedEventArgs e)
        {
            IBISensorChanged?.Invoke(this, e);
        }

        protected virtual void OnGSRSensorChanged(GSRSensorChangedEventArgs e)
        {
            GSRSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnTemperatureSensorChanged(TemperatureSensorChangedEventArgs e)
        {
            TemperatureSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnTagCreated(TagCreatedEventArgs e)
        {
            TagCreatedEvent?.Invoke(this, e);
        }
        #endregion

        #region EmpaticaEvents
        void EmpaticaGSRDataAcquired()
        {
            GSRSensorChangedEventArgs args = new GSRSensorChangedEventArgs();
            {
                args.GalvanicSkinResponse = 23;
            }
            OnGSRSensorChanged(args);
        }

        #endregion

        #region Methods
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

        public EmpaticaDataManager()
        {
            CheckBLEServer();
        }

        public void StartTCPClients()
        {       
            Thread EDPThread1 = new Thread(new ThreadStart(DataStreamAcc));
            Thread EDPThread2 = new Thread(new ThreadStart(DataStreamBVP));
            Thread EDPThread3 = new Thread(new ThreadStart(DataStreamGSR));
            Thread EDPThread4 = new Thread(new ThreadStart(DataStreamIBI));
            Thread EDPThread5 = new Thread(new ThreadStart(DataStreamTMP));
            Thread EDPThread6 = new Thread(new ThreadStart(DataStreamTag));
            EDPThread1.Start();
            EDPThread2.Start();
            EDPThread3.Start();
            EDPThread4.Start();
            EDPThread5.Start();
            EDPThread6.Start();

        }

        static void DataStreamAcc()
        {
            string DataStream = "acc";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }

        static void DataStreamBVP()
        {
            string DataStream = "bvp";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }

        static void DataStreamGSR()
        {
            string DataStream = "gsr";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }

        static void DataStreamIBI()
        {
            string DataStream = "ibi";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }

        static void DataStreamTMP()
        {
            string DataStream = "tmp";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }

        static void DataStreamTag()
        {
            string DataStream = "tag";
            ASyncTCPClient instance = new ASyncTCPClient();
            instance.StartClient(DataStream);

        }
        #endregion
    }
}

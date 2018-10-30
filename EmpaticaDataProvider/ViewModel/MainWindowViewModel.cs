using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EmpaticaDataProvider.Model;
using EmpaticaDataProvider.Classes;
using static EmpaticaDataProvider.Classes.TCPThreads;
using static SynchronousTCPClient;

namespace EmpaticaDataProvider.ViewModel
{
    class MainWindowViewModel : BindableBase
    {
        Datastreams datastream;
        TCPThreads tcpthreads = new TCPThreads();

        #region Vars & Properties
        private int _Tag;
        public int Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }

        private float _BloodVolumePulse;
        public float BloodVolumePulse
        {
            get { return _BloodVolumePulse; }
            set { _BloodVolumePulse = value;
                OnPropertyChanged("Empatica_BVP");
            }
        }

        private float _InterBeatInterval;
        public float InterBeatInterval
        {
            get { return _InterBeatInterval; }
            set { _InterBeatInterval = value;
                OnPropertyChanged("Empatica_IBI");
            }
        }
        private float _HearthRateVariability;
        public float HearthRateVariability
        {
            get { return _HearthRateVariability; }
            set { _HearthRateVariability = value;
                OnPropertyChanged("Empatica_HRV");
            }
        }

        private float _AccelerometerX;
        public float AccelerometerX
        {
            get { return _AccelerometerX; }
            set { _AccelerometerX = value;
                OnPropertyChanged("AccelerometerX");
            }
        }
        private float _AccelerometerY;
        public float AccelerometerY
        {
            get { return _AccelerometerY; }
            set { _AccelerometerY = value;
                OnPropertyChanged("AccelerometerY");
            }
        }

        private float _AccelerometerZ;
        public float AccelerometerZ
        {
            get { return _AccelerometerZ; }
            set { _AccelerometerZ = value;
                OnPropertyChanged("AccelerometerZ");
            }
        }

        private float _GalvanicSkinResponse;
        public float GalvanicSkinResponse
        {
            get { return _GalvanicSkinResponse; }
            set { _GalvanicSkinResponse = value;
                OnPropertyChanged("Empatica_GSR");
            }
        }
     
        private float _SkinTemperature;
        public float SkinTemperature
        {
            get { return _SkinTemperature; }
            set { _SkinTemperature = value;
                OnPropertyChanged("Empatica_Skin_Temp");
            }

        }
 
        private string _buttonText = "Start Recording";
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                OnPropertyChanged("ButtonText");

            }
        }

        private Brush _buttonColor = new SolidColorBrush(Colors.White);
        public Brush ButtonColor
        {
            get { return _buttonColor; }
            set
            {
                _buttonColor = value;
                OnPropertyChanged("ButtonColor");

            }
        }

        #endregion
        
        #region Constructor
        public MainWindowViewModel()
        {
            datastream = new Datastreams();
            datastream.instance.AccelerometerChanged += IUpdateAccelerometer;
            datastream.instance2.GSRSensorChanged += IUpdateGSRSensor;
            //tcpclient.BVPSensorChanged += IUpdatePPGSensor;
            //tcpclient.IBISensorChanged += IUpdateIBISensor;
            //tcpclient.TemperatureSensorChanged += IUpdateTemperatureSenser;
            //tcpclient.TagCreatedEvent += IUpdateTagCreated;
            HubConnector.StartConnection();
            HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
            HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            SetValueNames();
            BLEServer.CheckBLEServer();
            tcpthreads.CreateTCPThreads();
            tcpthreads.StartTcpThreads();
        }
        #endregion

        #region UI Handlers

        public void StartRecordingData()
        {
            if (Globals.IsRecordingData == false)
            {
                Globals.IsRecordingData = true;
                ButtonText = "Stop Recording";
                ButtonColor = new SolidColorBrush(Colors.Green);
                datastream.instance.GetEmpaticaData("acc");
                Console.WriteLine("test");
                //datastream.instance2.GetEmpaticaData();
            }
            else if (Globals.IsRecordingData == true)
            {
                Globals.IsRecordingData = false;
                ButtonText = "Start Recording";
                ButtonColor = new SolidColorBrush(Colors.White);
            }
        }

        #endregion

        #region UI Event Handlers
        private ICommand _buttonClicked;
        public ICommand OnButtonClicked
        {
            get
            {
                _buttonClicked = new RelayCommand(
                    param => this.StartRecordingData(), null
                    );
                return _buttonClicked;
            }
        }

        private void IUpdateAccelerometer(object sender, AccelerometerChangedEventArgs acc)
        {
            AccelerometerX = acc.AccelerometerX;
            AccelerometerY = acc.AccelerometerY;
            AccelerometerZ = acc.AccelerometerZ;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void IUpdateGSRSensor(object sender, GSRSensorChangedEventArgs gsr)
        {
            GalvanicSkinResponse = gsr.GSR;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void IUpdateIBISensor(object sender, IBISensorChangedEventArgs ibi)
        {
            InterBeatInterval = ibi.InterBeatInterval;
            HearthRateVariability = ibi.HearthRateVariability;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void IUpdatePPGSensor(object sender, BVPSensorChangedEventArgs bvp)
        {
            BloodVolumePulse = bvp.BloodVolumePulse;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void IUpdateTemperatureSenser(object sender, TemperatureSensorChangedEventArgs tmp)
        {
            SkinTemperature = tmp.SkinTemperature;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void IUpdateTagCreated(object sender, TagCreatedEventArgs tag)
        {
            Tag = tag.Tag;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SynchronousTCPClient.CloseTCPConnection();
            CloseApp();
            Environment.Exit(Environment.ExitCode);
        }

        public void CloseApp()
        {
            try
            {
                Process[] empaticaDataProviderProcess = Process.GetProcessesByName("EmpaticaDataProvider");
                empaticaDataProviderProcess[0].CloseMainWindow();

                Process[] empaticaBLEProcess = Process.GetProcessesByName("EmpaticaBLEServer");
                empaticaBLEProcess[0].Kill();

            }
            catch (Exception e)
            {
                Console.WriteLine("I got an exception after closing App" + e);
            }
        }


        #endregion

        #region Learning Hub Event Handlers
        private void MyConnector_stopRecordingEvent(object sender)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    this.StartRecordingData();
                }));
        }

        private void MyConnector_startRecordingEvent(object sender)
        {
            Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     this.StartRecordingData();
                 }));
        }
        #endregion

        #region Learning Hub Send Data
        public void SetValueNames()
        {
            var names = new List<string>
            {
                "empatica_AccX",
                "empatica_AccY",
                "empatica_AccZ",
                "empatica_Skin_Temp",
                "empatica_IBI",
                "empatica_BVP",
                "empatica_HRV",
                "empatica_GSR",
                "empatica_Tag"
            };
            HubConnector.SetValuesName(names);

        }

        public void SendData()
        {
            try
            {
                var values = new List<string>
                {
                    AccelerometerX.ToString(),
                    AccelerometerY.ToString(),
                    AccelerometerZ.ToString(),
                    SkinTemperature.ToString(),
                    InterBeatInterval.ToString(),
                    BloodVolumePulse.ToString(),
                    HearthRateVariability.ToString(),
                    GalvanicSkinResponse.ToString(),
                    Tag.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }
        #endregion
    }
}

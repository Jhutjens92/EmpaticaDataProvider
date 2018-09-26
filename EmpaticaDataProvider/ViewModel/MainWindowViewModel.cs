using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EmpaticaDataProvider.Model;
using EmpaticaDataProvider.EmpaticaManager;
using static EmpaticaDataProvider.EmpaticaManager.EmpaticaDataManager;

namespace EmpaticaDataProvider.ViewModel
{
    class MainWindowViewModel : BindableBase
    {
        EmpaticaDataManager empmanager = new EmpaticaDataManager();

        #region Vars & Properties
        private float _empatica_AccX = 0;
        public float Empatica_AccX
        {
            get { return _empatica_AccX; }
            set
            {
                _empatica_AccX = value;
                OnPropertyChanged("empatica_AccX");
            }
        }

        private float _empatica_AccY = 0;
        public float Empatica_AccY
        {
            get { return _empatica_AccY; }
            set
            {
                _empatica_AccY = value;
                OnPropertyChanged("empatica_AccY");
            }
        }

        private float _empatica_AccZ = 0;
        public float Empatica_AccZ
        {
            get { return _empatica_AccZ; }
            set
            {
                _empatica_AccZ = value;
                OnPropertyChanged("empatica_AccZ");
            }
        }

        private float _empatica_Skin_Temp = 0;
        public float Empatica_Skin_Temp
        {
            get { return _empatica_Skin_Temp; }
            set
            {
                _empatica_Skin_Temp = value;
                OnPropertyChanged("empatica_Skin_Temp");
            }
        }

        private float _empatica_BVP = 0;
        public float Empatica_BVP
        {
            get { return _empatica_BVP; }
            set
            {
                _empatica_BVP = value;
                OnPropertyChanged("empatica_BVP");
            }
        }

        private float _empatica_HRV = 0;
        public float Empatica_HRV
        {

            get { return _empatica_HRV; }
            set
            {
                _empatica_HRV = value;
                OnPropertyChanged("empatica_HRV");
            }
        }

        private float _empatica_IBI = 0;
        public float Empatica_IBI
        {

            get { return _empatica_IBI; }
            set
            {
                _empatica_IBI = value;
                OnPropertyChanged("empatica_IBI");
            }
        }

        private float _empatica_GSR = 0;
        public float Empatica_GSR
        {
            get { return _empatica_GSR; }
            set
            {
                _empatica_GSR = value;
                OnPropertyChanged("empatica_GSR");
            }
        }

        private int _empatica_Tag = 0;
        public int Empatica_Tag
        {
            get { return _empatica_Tag; }
            set
            {
                _empatica_Tag = value;
                OnPropertyChanged("empatica_Tag");
            }
        }

        private string _buttonText = "Start Recording";
        public String ButtonText
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
        
        #region Methods
        public MainWindowViewModel()
        {
            empmanager.GSRSensorChanged += UpdateGSRSensor;
            empmanager.AccelerometerChanged += UpdateAccelerometer;
            empmanager.PPGSensorChanged += UpdatePPGSensor;
            empmanager.IBISensorChanged += UpdateIBISensor;
            empmanager.TemperatureSensorChanged += UpdateTemperatureSenser;
            empmanager.TagCreated += UpdateTagCreated;
            HubConnector.StartConnection();
            HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
            HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            SetValueNames();
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
                empmanager.StartTCPClients();

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

        private void UpdateAccelerometer(object sender, AccelerometerChangedEventArgs a)
        {
            Empatica_AccX = a.AccelerometerX;
            Empatica_AccY = a.AccelerometerY;
            Empatica_AccZ = a.AccelerometerZ;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateGSRSensor(object sender, GSRSensorChangedEventArgs e)
        {
            Empatica_GSR = e.GalvanicSkinResponse;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateIBISensor(object sender, IBISensorChangedEventArgs e)
        {
            Empatica_IBI = e.InterBeatInterval;
            Empatica_HRV = e.HearthRateVariability;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdatePPGSensor(object sender, PPGSensorChangedEventArgs e)
        {
            Empatica_BVP = e.BloodVolumePulse;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateTemperatureSenser(object sender, TemperatureSensorChangedEventArgs e)
        {
            Empatica_Skin_Temp = e.SkinTemperature;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateTagCreated(object sender, TagCreatedEventArgs e)
        {
            Empatica_Tag = e.Tag;
            if (Globals.IsRecordingData == true)
            {
                SendData();
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
                    Empatica_AccX.ToString(),
                    Empatica_AccY.ToString(),
                    Empatica_AccZ.ToString(),
                    Empatica_Skin_Temp.ToString(),
                    Empatica_IBI.ToString(),
                    Empatica_BVP.ToString(),
                    Empatica_HRV.ToString(),
                    Empatica_GSR.ToString(),
                    Empatica_Tag.ToString()
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

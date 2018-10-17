﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using EmpaticaDataProvider.Model;
using EmpaticaDataProvider.Classes;
using static EmpaticaDataProvider.Classes.EmpaticaManager;

namespace EmpaticaDataProvider.ViewModel
{
    class MainWindowViewModel : BindableBase
    {
        EmpaticaManager empmanager = new EmpaticaManager();

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
                OnPropertyChanged("Empatica_AccX");
            }
        }
        private float _AccelerometerY;
        public float AccelerometerY
        {
            get { return _AccelerometerY; }
            set { _AccelerometerY = value;
                OnPropertyChanged("Empatica_AccY");
            }
        }

        private float _AccelerometerZ;
        public float AccelerometerZ
        {
            get { return _AccelerometerZ; }
            set { _AccelerometerZ = value;
                OnPropertyChanged("Empatica_AccY");
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
            empmanager.TagCreatedEvent += UpdateTagCreated;
            HubConnector.StartConnection();
            HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
            HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            SetValueNames();
            Application.Current.MainWindow.Closing += MainWindow_Closing;

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
            AccelerometerX = a.AccelerometerX;
            AccelerometerY = a.AccelerometerY;
            AccelerometerZ = a.AccelerometerZ;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateGSRSensor(object sender, GSRSensorChangedEventArgs e)
        {
            GalvanicSkinResponse = e.GalvanicSkinResponse;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateIBISensor(object sender, IBISensorChangedEventArgs e)
        {
            InterBeatInterval = e.InterBeatInterval;
            HearthRateVariability = e.HearthRateVariability;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdatePPGSensor(object sender, PPGSensorChangedEventArgs e)
        {
            BloodVolumePulse = e.BloodVolumePulse;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateTemperatureSenser(object sender, TemperatureSensorChangedEventArgs e)
        {
            SkinTemperature = e.SkinTemperature;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void UpdateTagCreated(object sender, TagCreatedEventArgs e)
        {
            Tag = e.Tag;
            if (Globals.IsRecordingData == true)
            {
                SendData();
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CloseConnection();
            CloseApp();
            Environment.Exit(Environment.ExitCode);
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

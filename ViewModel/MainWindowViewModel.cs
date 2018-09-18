using System;
﻿using EmpaticaDataProvider.Model;
using EmpaticaDataProvider.EmpaticaManager;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using static EmpaticaDataProvider.EmpaticaManager.EmpaticaDataManager;

namespace EmpaticaDataProvider.ViewModel
{
    class MainWindowViewModel: BindableBase
    {
        EmpaticaDataManager empmanager = new EmpaticaDataManager();

        #region Vars & Properties
        private string _PPG_Sensor= "";
        public String PPG_Sensor
        {
            get { return _PPG_Sensor; }
            set
            {
                _PPG_Sensor = value;
                OnPropertyChanged("PPG_Sensor");
            }
        }

        private string _Accelerometer_X= "";
        public String Accelerometer_X
        {
            get { return _Accelerometer_X; }
            set
            {
                _Accelerometer_X = value;
                OnPropertyChanged("Accelerometer_X");
            }
        }

        private string _Accelerometer_Y = "";
        public String Accelerometer_Y
        {
            get { return _Accelerometer_Y; }
            set
            {
                _Accelerometer_Y = value;
                OnPropertyChanged("Accelerometer_Y");
            }
        }

        private string _Accelerometer_Z = "";
        public String Accelerometer_Z
        {
            get { return _Accelerometer_Z; }
            set
            {
                _Accelerometer_X = value;
                OnPropertyChanged("Accelerometer_Z");
            }
        }

        private string _GSR_Sensor= "";
        public String GSR_Sensor
        {
            get { return _GSR_Sensor; }
            set
            {
                _GSR_Sensor = value;
                OnPropertyChanged("GSR_Sensor");
            }
        }

        private string _Temp_Wrist = ""; 
        public String Temp_Wrist
        {
            get { return _Temp_Wrist; }
            set
            {
                _Temp_Wrist = value;
                OnPropertyChanged("Temp_Wrist");
            }
        }

        private string _textReceived = "";
        public String TextReceived
        {
            get { return _textReceived; }
            set
            {
                _textReceived = value;
                OnPropertyChanged("TextReceived");

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

        public MainWindowViewModel()
        {
            empmanager.NewDataReceived += OnNewDataReceived;
            HubConnector.StartConnection();
            HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
            HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            SetValueNames();
        }

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

        private void OnNewDataReceived(object sender, TextReceivedEventArgs e)
        {
            TextReceived = e.TextReceived;
        }

        #region events
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

        public void StartRecordingData()
        {
            if (Globals.IsRecordingMqtt == false)
            {
                Globals.IsRecordingMqtt = true;
                ButtonText = "Stop Recording";
                ButtonColor = new SolidColorBrush(Colors.Green);

            }
            else if (Globals.IsRecordingMqtt == true)
            {
                Globals.IsRecordingMqtt = false;
                ButtonText = "Start Recording";
                ButtonColor = new SolidColorBrush(Colors.White);
            }
        }
        #endregion

        #region LearningHubMethods
        public void SetValueNames()
        {
            var names = new List<string>();
            names.Add("PPG_Sensor");
            names.Add("Accelerometer_X");
            names.Add("Accelerometer_Y");
            names.Add("Accelerometer_Z");
            names.Add("GSR_Sensor");
            names.Add("Temp_Wrist");
            HubConnector.SetValuesName(names);

        }

        public void SendData()
        {
            try
            {
                var values = new List<string>();
                values.Add(PPG_Sensor);
                values.Add(Accelerometer_X);
                values.Add(Accelerometer_Y);
                values.Add(Accelerometer_Z);
                values.Add(GSR_Sensor);
                values.Add(Temp_Wrist);
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

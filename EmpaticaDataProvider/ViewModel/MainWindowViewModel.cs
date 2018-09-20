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
        private string _empatica_AccX = "";
        public String empatica_AccX
        {
            get { return _empatica_AccX; }
            set
            {
                _empatica_AccX = value;
                OnPropertyChanged("empatica_AccX");
            }
        }

        private string _empatica_AccY = "";
        public String empatica_AccY
        {
            get { return _empatica_AccY; }
            set
            {
                _empatica_AccY = value;
                OnPropertyChanged("empatica_AccY");
            }
        }

        private string _empatica_AccZ = "";
        public String empatica_AccZ
        {
            get { return _empatica_AccZ; }
            set
            {
                _empatica_AccZ = value;
                OnPropertyChanged("empatica_AccZ");
            }
        }

        private string _empatica_Skin_Temp = "";
        public String empatica_Skin_Temp
        {
            get { return _empatica_Skin_Temp; }
            set
            {
                _empatica_Skin_Temp = value;
                OnPropertyChanged("empatica_Skin_Temp");
            }
        }

        private string _empatica_BVP = "";
        public String empatica_BVP
        {
            get { return _empatica_BVP; }
            set
            {
                _empatica_BVP = value;
                OnPropertyChanged("empatica_BVP");
            }
        }

        private string _empatica_HRV = "";
        public String empatica_HRV
        { 
        
            get { return _empatica_HRV; }
            set
            {
                _empatica_HRV = value;
                OnPropertyChanged("empatica_HRV");
            }
        }

        private string _empatica_GSR = ""; 
        public String empatica_GSR
        {
            get { return _empatica_GSR; }
            set
            {
                _empatica_GSR = value;
                OnPropertyChanged("empatica_GSR");
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
            empmanager.NewEmpaticaDataReceived += OnNewDataReceived;
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
            if (Globals.IsRecordingData == false)
            {
                Globals.IsRecordingData = true;
                ButtonText = "Stop Recording";
                ButtonColor = new SolidColorBrush(Colors.Green);

            }
            else if (Globals.IsRecordingData == true)
            {
                Globals.IsRecordingData = false;
                ButtonText = "Start Recording";
                ButtonColor = new SolidColorBrush(Colors.White);
            }
        }
        #endregion

        #region LearningHubMethods
        public void SetValueNames()
        {
            var names = new List<string>();
            names.Add("empatica_AccX");
            names.Add("empatica_AccY");
            names.Add("empatica_AccZ");
            names.Add("empatica_Skin_Temp");
            names.Add("empatica_BVP");
            names.Add("empatica_HRV");
            names.Add("empatica_GSR");
            HubConnector.SetValuesName(names);

        }

        public void SendData()
        {
            try
            {
                var values = new List<string>();
                values.Add(empatica_AccX);
                values.Add(empatica_AccY);
                values.Add(empatica_AccZ);
                values.Add(empatica_Skin_Temp);
                values.Add(empatica_BVP);
                values.Add(empatica_HRV);
                values.Add(empatica_GSR);
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

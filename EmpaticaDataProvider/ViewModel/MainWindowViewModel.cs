using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading.Tasks;
using EmpaticaDataProvider.Model;
using EmpaticaDataProvider.Classes;
using static EmpaticaDataProvider.Classes.TCPHandler;

namespace EmpaticaDataProvider.ViewModel
{
    /// <summary>   Class containing the GUI functions. </summary>
    ///
    /// <remarks>   Jordi Hutjens, 10-11-2018. </remarks>
    class MainWindowViewModel : BindableBase
    {
        #region Instance declaration

        TCPHandler synctcp = new TCPHandler();
        BLEServer bleserver = new BLEServer();
        SetLHJson lhjson = new SetLHJson();

        #endregion

        #region Vars & Properties

        /// <summary>   Gets or sets the blood volume pulse. </summary>
        ///
        /// <value> The blood volume pulse. </value>
        public float BloodVolumePulse
        {
            get { return bloodVolumePulse; }
            set { SetProperty(ref bloodVolumePulse, value); }
        }

        /// <summary>   The blood volume pulse. </summary>
        private float bloodVolumePulse;

        /// <summary>   Gets or sets the inter beat interval. </summary>
        ///
        /// <value> The inter beat interval. </value>
        public float InterBeatInterval
        {
            get { return interBeatInterval; }
            set { SetProperty(ref interBeatInterval, value); }
        }

        /// <summary>   The inter beat interval. </summary>
        private float interBeatInterval;

        /// <summary>   Gets or sets the hearth rate variability. </summary>
        ///
        /// <value> The hearth rate variability. </value>
        public float HearthRateVariability
        {
            get { return hearthRateVariability; }
            set { SetProperty(ref hearthRateVariability, value); }
        }

        /// <summary>   The hearth rate variability. </summary>
        private float hearthRateVariability;

        /// <summary>   Gets or sets the galvanic skin response. </summary>
        ///
        /// <value> The galvanic skin response. </value>
        public float GalvanicSkinResponse
        {
            get { return galvanicSkinResponse; }
            set { SetProperty(ref galvanicSkinResponse, value); }
        }

        /// <summary>   The galvanic skin response. </summary>
        private float galvanicSkinResponse;

        /// <summary>   Gets or sets the skin temperature. </summary>
        ///
        /// <value> The skin temperature. </value>
        public float SkinTemperature
        {
            get { return skinTemperature; }
            set { SetProperty(ref skinTemperature, value); }
        }

        /// <summary>   The skin temperature. </summary>
        private float skinTemperature;

        /// <summary>Gets or sets the accelerometer x coordinate.</summary>
        ///
        /// <value>The accelerometer x coordinate.</value>

        public float AccelerometerX
        {
            get { return accelerometerX; }
            set { SetProperty(ref accelerometerX, value); }
        }

        /// <summary>The accelerometer x coordinate.</summary>
        private float accelerometerX;

        /// <summary>Gets or sets the accelerometer y coordinate.</summary>
        ///
        /// <value>The accelerometer y coordinate.</value>
        public float AccelerometerY
        {
            get { return accelerometerY; }
            set { SetProperty(ref accelerometerY, value); }
        }

        /// <summary>The accelerometer y coordinate.</summary>
        private float accelerometerY;

        /// <summary>Gets or sets the accelerometer z coordinate.</summary>
        ///
        /// <value>The accelerometer z coordinate.</value>
        public float AccelerometerZ
        {
            get { return accelerometerZ; }
            set { SetProperty(ref accelerometerZ, value); }
        }

        /// <summary>The accelerometer z coordinate.</summary>
        private float accelerometerZ;

        /// <summary>Gets or sets the tag var.</summary>
        ///
        /// <value>The accelerometer z coordinate.</value>
        public int Tag
        {
            get { return tag; }
            set { SetProperty(ref tag, value); }
        }

        /// <summary>The accelerometer z coordinate.</summary>
        private int tag;

        /// <summary>Gets or sets the button text.</summary>
        ///
        /// <value>The button text.</value>
        public string ButtonText
        {
            get { return buttonText; }
            set { SetProperty(ref buttonText, value); }
        }

        /// <summary>The button text.</summary>
        private string buttonText = "Start Recording";

        /// <summary>Gets or sets the color of the button.</summary>
        ///
        /// <value>The color of the button.</value>
        public Brush ButtonColor
        {
            get { return buttonColor; }
            set { SetProperty(ref buttonColor, value); }
        }

        /// <summary>The button color.</summary>
        private Brush buttonColor = new SolidColorBrush(Colors.White);

        /// <summary>   Gets or sets a value indicating whether the ACC fields are visible. </summary>
        ///
        /// <value> True if ACC fields need to show, false if not. </value>
        public bool IAccelerometer
        {
            get { return iAccelerometer; }
            set
            {
                if (value != iAccelerometer)
                {
                    iAccelerometer = value;
                    OnPropertyChanged("IAccelerometer");
                }
            }
        }

        /// <summary>   Bool to show or hid the ACC fields. </summary>
        private bool iAccelerometer = false;

        /// <summary>   Gets or sets a value indicating whether the BVP fields are visible. </summary>
        ///
        /// <value> True if BVP fields need to show, false if not. </value>
        public bool IBloodVolumePulse
        {
            get { return iBloodVolumePulse; }
            set
            {
                if (value != iBloodVolumePulse)
                {
                    iBloodVolumePulse = value;
                    OnPropertyChanged("IBloodVolumePulse");
                }
            }
        }

        /// <summary>   Bool to show or hide the BVP fields. </summary>
        private bool iBloodVolumePulse = false;

        /// <summary>   Gets or sets a value indicating whether the IBI fields are visible. </summary>
        ///
        /// <value> True if IBI fields need to show, false if not. </value>
        public bool IInterBeatInterval
        {
            get { return iInterBeatInterval; }
            set
            {
                if (value != iInterBeatInterval)
                {
                    iInterBeatInterval = value;
                    OnPropertyChanged("IInterBeatInterval");
                }
            }
        }

        /// <summary>   Bool to show or hid the IBI fields. </summary>
        private bool iInterBeatInterval = false;

        /// <summary>   Gets or sets a value indicating whether the TMP fields are visible. </summary>
        ///
        /// <value> True if TMP fields need to show, false if not. </value>
        public bool ISkinTemperature
        {
            get { return iSkinTemperature; }
            set
            {
                if (value != iSkinTemperature)
                {
                    iSkinTemperature = value;
                    OnPropertyChanged("ISkinTemperature");
                }
            }
        }

        /// <summary>   Bool to show or hid the Skin Temp fields. </summary>
        private bool iSkinTemperature = false;

        /// <summary>   Gets or sets a value indicating whether the GSR fields are visible. </summary>
        ///
        /// <value> True if GSR fields need to show, false if not. </value>
        public bool IGalvanicSkinResponse
        {
            get { return iGalvanicSkinResponse; }
            set
            {
                if (value != iGalvanicSkinResponse)
                {
                    iGalvanicSkinResponse = value;
                    OnPropertyChanged("IGalvanicSkinResponse");
                }
            }
        }

        /// <summary>   Bool to show or hid the GSR fields. </summary>
        private bool iGalvanicSkinResponse = false;

        /// <summary>   Gets or sets a value indicating whether the TAG fields are visible. </summary>
        ///
        /// <value> True if GSR fields need to show, false if not. </value>
        public bool ITag
        {
            get { return iTag; }
            set
            {
                if (value != iTag)
                {
                    iTag = value;
                    OnPropertyChanged("ITag");
                }
            }
        }

        /// <summary>   Bool to show or hid the TAG fields. </summary>
        private bool iTag = false;

        #endregion

        #region Constructor

        /// <summary>MainWindowViewModel constructor.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        public MainWindowViewModel()
        {
            CheckLearningHub();
            if (CheckParameters.Instance.LHRunning)
            {
                HubConnector.StartConnection();
                HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
                HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
            }
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            CheckParameters.Instance.CheckStartupParameters();
            if (CheckParameters.Instance.LHRunning)
            {
                lhjson.SetValueNames();
            }
            SubscribeToEvents();
            IAppereance();
            bleserver.CheckBLEServer();
            synctcp.ConnectEmpatica();
            
        }
        #endregion

        #region Methods

        /// <summary>   Check if the Learning Hub is running. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void CheckLearningHub()
        {
            Process[] LearningHub = Process.GetProcessesByName("HubDesktop");
            if (LearningHub.Length > 0)
            {
                CheckParameters.Instance.LHRunning = true;
            }
        }

        /// <summary>   Subscribes to events based on the DataStream parameter. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void SubscribeToEvents()
        {
            switch (CheckParameters.Instance.DataStream)
            {
                case "acc":
                    synctcp.AccelerometerChanged += IUpdateAccelerometer;
                    break;
                case "gsr":
                    synctcp.GSRSensorChanged += IUpdateGalvanicSkinResponse;
                    break;
                case "tag":
                    synctcp.TagCreatedEvent += IUpdateTag;
                    break;
                case "bvp":
                    synctcp.BVPSensorChanged += IUpdateBloodVolumePulse;
                    break;
                case "tmp":
                    synctcp.TemperatureSensorChanged += IUpdateSkinTemperature;
                    break;
                case "ibi":
                    synctcp.IBISensorChanged += IUpdateInterBeatInterval;
                    break;
            }   
        }

        #endregion

        #region UI Methods

        /// <summary>Starts or stops recording data method.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        private void StartRecordingData()
        {
            if (Globals.IsRecordingData)
            {
                Globals.IsRecordingData = false;
                ButtonText = "Start Recording";
                ButtonColor = new SolidColorBrush(Colors.White);
            }
            else
            {
                Globals.IsRecordingData = true;
                ButtonText = "Stop Recording";
                ButtonColor = new SolidColorBrush(Colors.Green);
                new Task(() => { synctcp.GetEmpaticaData(); }).Start();
            }

        }

        /// <summary>   Changes the interface based on the Datastream parameter. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void IAppereance()
        {
            switch (CheckParameters.Instance.DataStream)
            {
                case "acc":
                    iAccelerometer = true;
                    break;
                case "gsr":
                    iGalvanicSkinResponse = true;
                    break;
                case "tag":
                    iTag = true;
                    break;
                case "bvp":
                    iBloodVolumePulse = true;
                    break;
                case "tmp":
                    iSkinTemperature = true;
                    break;
                case "ibi":
                    iInterBeatInterval = true;
                    break;
            }
        }

        #endregion

        #region UI Event Handlers

        /// <summary>The button clicked.</summary>
        private ICommand _buttonClicked;

        /// <summary>Gets the on button clicked.</summary>
        ///
        /// <value>The on button clicked.</value>
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

        /// <summary>Updates the accelerometer.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="acc">   Accelerometer changed event information.</param>
        private void IUpdateAccelerometer(object sender, AccelerometerChangedEventArgs acc)
        {
            AccelerometerX = acc.AccelerometerX;
            AccelerometerY = acc.AccelerometerY;
            AccelerometerZ = acc.AccelerometerZ;
        }

        /// <summary>
        /// Updates the tag value based on how many times the tag button has been pressed during a
        /// recording.
        /// </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void IUpdateTag(object sender, TagCreatedEventArgs tag)
        {
            Tag++;
        }

        /// <summary>   Updates the blood volume pulse. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="bvp">      Bvp sensor changed event information. </param>
        private void IUpdateBloodVolumePulse(object sender, BVPSensorChangedEventArgs bvp)
        {
            BloodVolumePulse = bvp.BloodVolumePulse;
        }

        /// <summary>   Updates the inter beat interval. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="ibi">      Ibi sensor changed event information. </param>
        private void IUpdateInterBeatInterval(object sender, IBISensorChangedEventArgs ibi)
        {
            InterBeatInterval = ibi.InterBeatInterval;
            HearthRateVariability = ibi.HearthRateVariability;
        }

        /// <summary>   Updates the skin temperatue. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="tmp">      Temperature sensor changed event information. </param>
        private void IUpdateSkinTemperature(object sender, TemperatureSensorChangedEventArgs tmp)
        {
            SkinTemperature = tmp.SkinTemperature;
        }

        /// <summary>   Updates the galvanic skin response. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="gsr">      Gsr sensor changed event information. </param>
        private void IUpdateGalvanicSkinResponse(object sender, GSRSensorChangedEventArgs gsr)
        {
            GalvanicSkinResponse = gsr.GalvanicSkinResponse;
        }

        /// <summary>Event handler. Called by MainWindow for closing events.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="sender">Source of the event.</param>
        /// <param name="e">     Cancel event information.</param>
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            synctcp.CloseEmpaticaConnection();
            synctcp.CloseTCPConnection();
            bleserver.CloseBLEServer();
            Environment.Exit(Environment.ExitCode);
        }

        #endregion

        #region Learning Hub Event Handlers

        /// <summary>My connector stop recording event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="sender">Source of the event.</param>

        private void MyConnector_stopRecordingEvent(object sender)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => {
                    this.StartRecordingData();
                }));
        }

        /// <summary>My connector start recording event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="sender">Source of the event.</param>

        private void MyConnector_startRecordingEvent(object sender)
        {
            Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => {
                     this.StartRecordingData();
                 }));
        }
        #endregion
    }
}

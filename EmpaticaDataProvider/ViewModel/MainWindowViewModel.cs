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

        #endregion

        #region Constructor

        /// <summary>MainWindowViewModel constructor.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>

        public MainWindowViewModel()
        {
            Process[] LearningHub = Process.GetProcessesByName("HubDesktop");
            if (LearningHub.Length > 0)
            {
                HubConnector.StartConnection();
                HubConnector.MyConnector.startRecordingEvent += MyConnector_startRecordingEvent;
                HubConnector.MyConnector.stopRecordingEvent += MyConnector_stopRecordingEvent;
                lhjson.SetValueNames();
            }
            Application.Current.MainWindow.Closing += MainWindow_Closing;
            synctcp.AccelerometerChanged += IUpdateAccelerometer;
            CheckParameters.Instance.CheckStartupParameters();
            bleserver.CheckBLEServer();
            synctcp.ConnectEmpatica();
        }
        #endregion

        #region UI Methods

        /// <summary>Starts or stops recording data method.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>

        public void StartRecordingData()
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

﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;
using EmpaticaDataProvider;
using EmpaticaDataProvider.Classes;
using EmpaticaDataProvider.ViewModel;

namespace EmpaticaDataProvider.Classes
{
    public class TCPHandler
    {

        #region Instance declaration
        SetLHJson lhsend = new SetLHJson();
        #endregion

        #region Variables
        /// <summary>   True if the TCPClient is connected to the Empatica BLE Server. </summary>
        private bool tcpClientConnected = false;

        /// <summary>   // String array containing the filtered message split on null. </summary>
        private string[] receivedStrFiltered = { };

        /// <summary>   // String array containing the filtered IBI message split on null. </summary>
        List<string> receivedStrFilteredIBI = new List<string>();

        /// <summary>   Int step counter to verify what to send next. </summary>
        private int tcpStep = 0;

        /// <summary>   Number of TCP tries before checking if the BLE might be connected. </summary>
        private int tcpTryCount = 0;

        /// <summary>Identifier for the empatica.</summary>
        private int empaticaID;

        /// <summary>   Counts the amount of times the CheckIfIBIStream functions has executed. </summary>
        private int ibiStreamCount = 0;

        /// <summary>   Create a TCP/IP  socket. </summary>
        private Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>ManualResetEvent instances signal completion.</summary>
        private readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        /// <summary>The received tcp string.</summary>
        string receivedStr = string.Empty;

        #endregion

        #region Events

        /// <summary>Additional information for empatica connected events.</summary>
        ///
        /// <remarks>Jordi Hutjens, 14-11-2018.</remarks>
        public class EmpaticaConnectedEventArgs : EventArgs
        {
        }

        /// <summary>Accelerometer changed events + vars.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        public class AccelerometerChangedEventArgs : EventArgs
        {
            private float accelerometerX;
            public float AccelerometerX
            {
                get { return accelerometerX; }
                set { accelerometerX = value; }
            }
            private float accelerometerY;
            public float AccelerometerY
            {
                get { return accelerometerY; }
                set { accelerometerY = value; }
            }

            private float accelerometerZ;
            public float AccelerometerZ
            {
                get { return accelerometerZ; }
                set { accelerometerZ = value; }
            }
        }

        /// <summary>InterBeatInterval changed events + vars.</summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        public class IBISensorChangedEventArgs : EventArgs
        {
            private float interBeatInterval;
            public float InterBeatInterval
            {
                get { return interBeatInterval; }
                set { interBeatInterval = value; }
            }
            private float hearthRateVariability;
            public float HearthRateVariability
            {
                get { return hearthRateVariability; }
                set { hearthRateVariability = value; }
            }
        }

        /// <summary>GalvanicSkinResponse changed events + vars.</summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        public class GSRSensorChangedEventArgs : EventArgs
        {
            private float galvanicSkinResponse;
            public float GalvanicSkinResponse
            {
                get { return galvanicSkinResponse; }
                set { galvanicSkinResponse = value; }
            }
        }

        /// <summary>Temperature Sensor changed events + vars.</summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        public class TemperatureSensorChangedEventArgs : EventArgs
        {
            private float skinTemperature;
            public float SkinTemperature
            {
                get { return skinTemperature; }
                set { skinTemperature = value; }
            }
        }

        /// <summary>BloodVolumePulse changed events + vars.</summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        public class BVPSensorChangedEventArgs : EventArgs
        {
            private float bloodVolumePulse;
            public float BloodVolumePulse
            {
                get { return bloodVolumePulse; }
                set { bloodVolumePulse = value; }
            }
        }

        /// <summary>Tag changed events + vars.</summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        public class TagCreatedEventArgs : EventArgs
        {
            private int tag;
            public int Tag
            {
                get { return tag; }
                set { tag = value; }
            }
        }

        /// <summary>Event queue for all listeners interested in AccelerometerChanged events.</summary>
        public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;

        /// <summary>Event queue for all listeners interested in AccelerometerChanged events.</summary>
        public event EventHandler<EmpaticaConnectedEventArgs> EmpaticaConnectedTrue;

        /// <summary>   Event queue for all listeners interested in IBISensorChanged events. </summary>
        public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;

        /// <summary>   Event queue for all listeners interested in BVPSensorChanged events. </summary>
        public event EventHandler<BVPSensorChangedEventArgs> BVPSensorChanged;

        /// <summary>   Event queue for all listeners interested in GSRSensorChanged events. </summary>
        public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;

        /// <summary>   Event queue for all listeners interested in TemperatureSensorChanged events. </summary>
        public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;

        /// <summary>   Event queue for all listeners interested in tagCreated events. </summary>
        public event EventHandler<TagCreatedEventArgs> TagCreatedEvent;


        /// <summary>Raises the accelerometer changed event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="e">Event information to send to registered event handlers.</param>
        protected virtual void OnEmpaticaConnected(EmpaticaConnectedEventArgs e)
        {
            EmpaticaConnectedTrue?.Invoke(this, e);
        }

        /// <summary>Raises the accelerometer changed event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="e">Event information to send to registered event handlers.</param>
        protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
        {
            AccelerometerChanged?.Invoke(this, e);
        }

        /// <summary>   Raises the bvp sensor changed event. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        protected virtual void OnBVPSensorChanged(BVPSensorChangedEventArgs e)
        {
            BVPSensorChanged?.Invoke(this, e);
        }

        /// <summary>   Raises the ibi sensor changed event. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        protected virtual void OnIBISensorChanged(IBISensorChangedEventArgs e)
        {
            IBISensorChanged?.Invoke(this, e);
        }

        /// <summary>   Raises the gsr sensor changed event. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        protected virtual void OnGSRSensorChanged(GSRSensorChangedEventArgs e)
        {
            GSRSensorChanged?.Invoke(this, e);
        }

        /// <summary>   Raises the temperature sensor changed event. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        protected virtual void OnTemperatureSensorChanged(TemperatureSensorChangedEventArgs e)
        {
            TemperatureSensorChanged?.Invoke(this, e);
        }

        /// <summary>   Raises the tag created event. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 12-11-2018. </remarks>
        ///
        /// <param name="e">    Event information to send to registered event handlers. </param>
        protected virtual void OnTagCreated(TagCreatedEventArgs e)
        {
            TagCreatedEvent?.Invoke(this, e);
        }

        #endregion

        #region Methods

        /// <summary>Connects the empatica.</summary>
        ///
        /// <remarks>Jordi Hutjens, 9-11-2018.</remarks>
        public void ConnectEmpatica()
        {
            try
            {
                while (!tcpClientConnected)
                {
                    StartSyncTCPClient();
                }
                while (tcpStep < 4)
                {
                    SyncSend(CreateTcpCmd());
                    SyncReceive();
                    ChkReceivedMsg();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>   Gets the empatica data (Start recording button). </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        public void GetEmpaticaData()
        {
            SyncSend(CreateTcpCmd());
            SyncReceive();
            ChkReceivedMsg();
            if (tcpStep == 5)
            {
                ASyncReceive(client);
            }
        }

        /// <summary>Check received message.</summary>
        ///
        /// <remarks>Jordi Hutjens, 9-11-2018.</remarks>
        private void ChkReceivedMsg()
        {
            switch (tcpStep)
            {
                case 0:
                    if (int.Parse(receivedStrFiltered[2]) > 0 && receivedStrFiltered[6] == "allowed")
                    {
                        tcpStep = 1;
                    }
                    if (int.Parse(receivedStrFiltered[2]) == 0)
                    {
                        tcpTryCount++;
                        if (tcpTryCount == 5) { tcpStep = 2; }
                    }
                    break;
                case 1:
                    if (receivedStrFiltered[2] == "OK")
                    {
                        tcpStep = 2;
                        Console.WriteLine("Empatica Bluetooth connected.\n");
                    }
                    else if (receivedStrFiltered[2] == "ERR" && receivedStrFiltered[7] == "connected")
                    {
                        tcpStep = 2;
                        Console.WriteLine("Empatica Bluetooth already connected.\n");
                    }
                    break;
                case 2:
                    if (Int32.Parse(receivedStrFiltered[2]) > 0)
                    {
                        tcpStep = 3;
                        empaticaID = Int32.Parse(receivedStrFiltered[4]);
                    }
                    if (tcpTryCount != 0 && Int32.Parse(receivedStrFiltered[2]) == 0)
                    {
                        tcpTryCount--;
                        if (tcpTryCount == 0) { tcpStep = 0; }
                    }
                    break;
                case 3:
                    if (receivedStrFiltered[2] == "OK")
                    {
                        tcpStep = 4;
                        Console.WriteLine("Empatica E4 Wristband with ID: {0} connected. \n", empaticaID);
                        EmpaticaConnected();
                    }
                    break;

                case 4:
                    if (receivedStrFiltered[3] == "OK")
                    {
                        tcpStep = 5;
                        Console.WriteLine("Connected to Empatica Datastream: {0}", CheckParameters.Instance.DataStream, " \n");
                    }
                    break;
                case 5:
                    if (receivedStrFiltered[3] == "OFF")
                    {
                        tcpStep = 6;
                        Console.WriteLine("Disconnected from Empatica Datastream: {0}", CheckParameters.Instance.DataStream, " \n");
                    }
                    break;
            }
        }

        /// <summary>Creates TCP command.</summary>
        ///
        /// <remarks>Jordi Hutjens, 9-11-2018.</remarks>
        ///
        /// <returns>The new TCP command.</returns>
        private string CreateTcpCmd()
        {
            var tcpCommandStr = "";
            switch (tcpStep)
            {
                case 0:
                    tcpCommandStr = "device_discover_list";
                    break;
                case 1:
                    tcpCommandStr = "device_connect_btle " + receivedStrFiltered[4];
                    break;
                case 2:
                    tcpCommandStr = "device_list";
                    break;
                case 3:
                    tcpCommandStr = "device_connect " + receivedStrFiltered[4];
                    break;
                case 4:
                    tcpCommandStr = "device_subscribe " + CheckParameters.Instance.DataStream + " ON";
                    break;
                case 5:
                    tcpCommandStr = "device_subscribe " + CheckParameters.Instance.DataStream + " OFF";
                    break;
                case 6:
                    tcpCommandStr = "device_disconnect";
                    break;
            }
            return tcpCommandStr;
        }

        /// <summary>   Checks which updates values event we should create. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void CheckUpdateValues()
        {
            if (Globals.IsRecordingData)
            {
                switch (CheckParameters.Instance.DataStream)
                {
                    case "acc":
                        UpdateAccValues();
                        break;
                    case "gsr":
                        UpdateGsrValues();
                        break;
                    case "tag":
                        UpdateTagValues();
                        break;
                    case "bvp":
                        UpdateBvpValues();
                        break;
                    case "tmp":
                        UpdateTmpValues();
                        break;
                    case "ibi":
                        UpdateIbiValues();
                        break;
                }
            }
        }

        /// <summary>   Closes empatica connection. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        public void CloseEmpaticaConnection()
        {
            if (tcpStep == 5)
            {
                SyncSend(CreateTcpCmd());
                ChkReceivedMsg();
            }
        }

        /// <summary>   Check if the subscribed stream is IBI. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void CheckIfIBIStream()
        {
            if (CheckParameters.Instance.DataStream == "ibi" && ibiStreamCount == 0)
            {
                receivedStrFilteredIBI.Add(receivedStrFiltered[2]);
                ibiStreamCount++;
            }
            else if (CheckParameters.Instance.DataStream == "ibi" && ibiStreamCount == 1)
            {
                receivedStrFilteredIBI.Add(receivedStrFiltered[2]);
                ibiStreamCount = 0;
                CheckUpdateValues();
            }
            else
            {
                CheckUpdateValues();
            }
        }

        #endregion

        #region TCP methods

        /// <summary>Starts synchronise TCP client.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        private void StartSyncTCPClient()
        {
            // Establish the remote endpoint for the socket.  
            var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(CheckParameters.Instance.ServerIP) } };
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, CheckParameters.Instance.ServerPort);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                client.Connect(remoteEP);
                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint.ToString());
                tcpClientConnected = true;               
            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }


        /// <summary>Closes TCP connection.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        public void CloseTCPConnection()
        {
            try
            {
                client.Shutdown(SocketShutdown.Both);
                // Release the socket. 
                client.Close();
            }
            catch (Exception e) // add the catch exception explanation https://stackoverflow.com/questions/4662553/how-to-abort-sockets-beginreceive
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>Synchronous send tcp.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="TCPCommandStr">The TCP command string.</param>
        private void SyncSend(string TCPCommandStr)
        {
            try
            {
                // Encode the data string into a byte array.  
                byte[] TCPCommandByte = Encoding.ASCII.GetBytes(TCPCommandStr + Environment.NewLine);
                // Send the data through the socket.  
                client.Send(TCPCommandByte);
                Console.WriteLine("Sending: {0} ", TCPCommandStr);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>Synchronous receive tcp.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <returns>the received tcp string (filtered).</returns>
        private string SyncReceive()
        {
            // Data buffer for incoming data.  
            byte[] receivedBytes = new byte[128];

            try
            {
                // Receive the response from the remote device.  
                int byteCount = client.Receive(receivedBytes);
                receivedStr = Encoding.UTF8.GetString(receivedBytes, 0, byteCount);
                receivedStrFiltered = receivedStr.Split(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.WriteLine("Received: {0}", receivedStr);
            return receivedStr;
        }

        /// <summary>A sync receive tcp function. </summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="client">Create a TCP/IP  socket.</param>
        private void ASyncReceive(Socket client)
        {
            try
            {
                // Create the state object.
                var state = new StateObject { WorkSocket = client };

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ASyncReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>Async receive callback, called bij asynchronise receive.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="ar">The result of the asynchronous operation.</param>
        private void ASyncReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket 
                // from the asynchronous state object.
                var state = (StateObject)ar.AsyncState;
                var client = state.WorkSocket;
                // Read data from the remote device.
                var bytesRead = client.EndReceive(ar);

                if (bytesRead > 0)
                {
                    // There might be more data, so store the data received so far.
                    state.Sb.Append(Encoding.ASCII.GetString(state.Buffer, 0, bytesRead));
                    receivedStr = state.Sb.ToString();
                    state.Sb.Clear();
                    ReceiveDone.Set();
                    // Get the rest of the data.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ASyncReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.Sb.Length > 1)
                    {
                        receivedStr = state.Sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            receivedStrFiltered = receivedStr.Split(null);
            //We need to execute this function because the IBI datastream sends it's data over 2 async receive functions.
            CheckIfIBIStream();
        }
        #endregion

        #region Event methods

        /// <summary>Updates the ellipse color to see if the empatica is connected.</summary>
        ///
        /// <remarks>Jordi Hutjens, 14-11-2018.</remarks>
        private void EmpaticaConnected()
        {
            EmpaticaConnectedEventArgs args = new EmpaticaConnectedEventArgs { };
            OnEmpaticaConnected(args);
        }

        /// <summary>Updates the accelerometer values and trigger the event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        private void UpdateAccValues()
        {
            try
            {
                AccelerometerChangedEventArgs args = new AccelerometerChangedEventArgs
                {
                    AccelerometerX = float.Parse(receivedStrFiltered[2]),
                    AccelerometerY = float.Parse(receivedStrFiltered[3]),
                    AccelerometerZ = float.Parse(receivedStrFiltered[4])

                };
                OnAccelerometerChanged(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendAccDataToLH(args); }
            }
            catch (Exception)
            {
                AccelerometerChangedEventArgs args = new AccelerometerChangedEventArgs
                {
                    AccelerometerX = 0.0F,
                    AccelerometerY = 0.0F,
                    AccelerometerZ = 0.0F
                };
                OnAccelerometerChanged(args);
            }
        }

        /// <summary>   Updates the ibi values. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void UpdateIbiValues()
        {
            try
            {
                IBISensorChangedEventArgs args = new IBISensorChangedEventArgs
                {
                    HearthRateVariability = float.Parse(receivedStrFilteredIBI[0]),
                    InterBeatInterval = float.Parse(receivedStrFilteredIBI[1])

                };
                receivedStrFilteredIBI.Clear();
                OnIBISensorChanged(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendIbiDataToLH(args); }
            }
            catch (Exception)
            {
                IBISensorChangedEventArgs args = new IBISensorChangedEventArgs
                {
                    InterBeatInterval = 0.0F,
                    HearthRateVariability = 0.0F
                };
                OnIBISensorChanged(args);
            }
        }

        /// <summary>   Updates the bvp values. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void UpdateBvpValues()
        {
            try
            {
                BVPSensorChangedEventArgs args = new BVPSensorChangedEventArgs
                {
                    BloodVolumePulse = float.Parse(receivedStrFiltered[2])
                };
                OnBVPSensorChanged(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendBvpDataToLH(args); }
            }
            catch (Exception)
            {
                BVPSensorChangedEventArgs args = new BVPSensorChangedEventArgs
                {
                    BloodVolumePulse = 0.0F
                };
                OnBVPSensorChanged(args);
            }
        }

        /// <summary>   Updates the gsr values. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void UpdateGsrValues()
        {
            try
            {
                GSRSensorChangedEventArgs args = new GSRSensorChangedEventArgs
                {
                    GalvanicSkinResponse = float.Parse(receivedStrFiltered[2])
                };
                OnGSRSensorChanged(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendGsrDataToLH(args); }
            }
            catch (Exception)
            {
                GSRSensorChangedEventArgs args = new GSRSensorChangedEventArgs
                {
                    GalvanicSkinResponse = 0.0F
                };
                OnGSRSensorChanged(args);
            }
        }

        /// <summary>   Updates the temperature values. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void UpdateTmpValues()
        {
            try
            {
                TemperatureSensorChangedEventArgs args = new TemperatureSensorChangedEventArgs
                {
                    SkinTemperature = float.Parse(receivedStrFiltered[2])
                };
                OnTemperatureSensorChanged(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendTmpDataToLH(args); }
            }
            catch (Exception)
            {
                TemperatureSensorChangedEventArgs args = new TemperatureSensorChangedEventArgs
                {
                    SkinTemperature = 0.0F
                };
                OnTemperatureSensorChanged(args);
            }
        }

        /// <summary>   Updates the tag values. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 13-11-2018. </remarks>
        private void UpdateTagValues()
        {
            try
            {
                TagCreatedEventArgs args = new TagCreatedEventArgs
                {
                    Tag = 1
                };
                OnTagCreated(args);
                if (CheckParameters.Instance.LHRunning) { lhsend.SendTagDataToLH(args); }
            }
            catch (Exception)
            {
                TagCreatedEventArgs args = new TagCreatedEventArgs
                {
                    Tag = 0
                };
                OnTagCreated(args);
            }
        }
        #endregion
    }
}
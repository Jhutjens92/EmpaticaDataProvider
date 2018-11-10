using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        private static bool tcpClientConnected = false;

        /// <summary>   // String containing the filtered message split on null. </summary>
        private static string[] receivedStrFiltered = { };

        /// <summary>   Int step counter to verify what to send next. </summary>
        private static int tcpStep = 0;

        /// <summary>Identifier for the empatica.</summary>
        private static int empaticaID;

        /// <summary>   Create a TCP/IP  socket. </summary>
        private static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>ManualResetEvent instances signal completion.</summary>
        private static readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        /// <summary>The received tcp string.</summary>
        string receivedStr = string.Empty;

        #endregion

        #region Events

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


        /// <summary>Event queue for all listeners interested in AccelerometerChanged events.</summary>
        public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;

        /// <summary>Raises the accelerometer changed event.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>
        ///
        /// <param name="e">Event information to send to registered event handlers.</param>

        protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
        {
            AccelerometerChanged?.Invoke(this, e);
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
                if (!tcpClientConnected)
                {
                    StartSyncTCPClient();
                    tcpClientConnected = true;
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
                    break;
                case 1:
                    if (receivedStrFiltered[2] == "OK")
                    {
                        tcpStep = 2;
                        Console.WriteLine("Empatica Bluetooth connected.\n");
                    }
                    break;
                case 2:
                    if (Int32.Parse(receivedStrFiltered[2]) > 0)
                    {
                        tcpStep = 3;
                        empaticaID = Int32.Parse(receivedStrFiltered[4]);
                    }
                    break;
                case 3:
                    if (receivedStrFiltered[2] == "OK")
                    {
                        tcpStep = 4;
                        Console.WriteLine("Empatica E4 Wristband with ID: {0} connected. \n", empaticaID);
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


        public void CloseEmpaticaConnection()
        {   
            SyncSend(CreateTcpCmd());
            ChkReceivedMsg();           
        }

        #endregion

        #region TCP methods

        /// <summary>Starts synchronise TCP client.</summary>
        ///
        /// <remarks>Jordi Hutjens, 10-11-2018.</remarks>

        private void StartSyncTCPClient()
        {
            // Connect to a remote device.  
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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
        /// <returns>A string.</returns>

        private string SyncReceive()
        {
            // Data buffer for incoming data.  
            byte[] receivedBytes = new byte[64];

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
            //Console.WriteLine(receivedStr);
            if (Globals.IsRecordingData)
            {
                UpdateAccValues(); 
            }
        }
        #endregion

        #region Event methods

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
                lhsend.SendDataToLH(args);

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
        #endregion
    }
}
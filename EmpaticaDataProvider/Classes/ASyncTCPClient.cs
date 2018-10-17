using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using EmpaticaDataProvider.ViewModel;

namespace EmpaticaDataProvider.Classes
{
    public class ASyncTCPClient
    {
        #region Vars
        // The server ip for the BLE Empatica Server.
        private const string ServerAddress = "127.0.0.1";
        // The port number for the BLE Empatica Server.
        private const int ServerPort = 5555;
        // String containing the filtered message split on null. 
        private string[] FilteredResponse = { };
        // Int step counter to verify what to send next.
        private int tcpStep = 0;
        // sendDoneBool to block the async send
        private bool sendDoneBool = true;
        // ManualResetEvent instances signal completion.
        private readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private String _response = String.Empty;
        #endregion

        #region Instance Declaration
        EmpaticaManager EDM = new EmpaticaManager();
        #endregion

        #region TCP Functions

        // Create a TCP/IP socket.
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public void StartClient(string DataStream)
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.

                var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEp = new IPEndPoint(ipAddress, ServerPort);


                // Connect to the remote endpoint.
                client.BeginConnect(remoteEp, (ConnectCallback), client);
                ConnectDone.WaitOne();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            MakeTCPCommands(DataStream);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}", client.RemoteEndPoint);

                // Signal that the connection has been made.
                ConnectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                var state = new StateObject { WorkSocket = client };

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
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
                    _response = state.Sb.ToString();
                    state.Sb.Clear();
                    ReceiveDone.Set();

                    // Get the rest of the data.
                    client.BeginReceive(state.Buffer, 0, StateObject.BufferSize, 0, ReceiveCallback, state);
                }
                else
                {
                    // All the data has arrived; put it in response.
                    if (state.Sb.Length > 1)
                    {
                        _response = state.Sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            Console.Write(_response);
            FilteredResponse = _response.Split(null);
            CheckSendStep();
            sendDoneBool = true;
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
            Console.WriteLine(data);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                var client = (Socket)ar.AsyncState;
                // Complete sending the data to the remote device.
                client.EndSend(ar);
                // Signal that all bytes have been sent.
                SendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        #endregion

        #region Program functions

        // Closes the EmpaticaBLE connection when the program stops
        public void CloseConnection()
        {
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }

        private void CheckSendStep()
        {
            if (FilteredResponse[4] != "")
            {
                tcpStep = 1;
                sendDoneBool = false;
            }
            else if (FilteredResponse[2] == "OK")
            {
                tcpStep = 2;
            }
            else if (FilteredResponse[4] != "")
            {
                tcpStep = 3;
            }
            else if (FilteredResponse[2] == "OK")
            {
                tcpStep = 4;
            }

        }

        private void MakeTCPCommands(string DataStream)
        {
            bool tcpSequenceComplete = false;
            var tcpCommand = "";
            while (tcpSequenceComplete == false)
            {
                switch (tcpStep)
                {
                    case 0:
                        tcpCommand = "device_discover_list";
                        TCPSendReceive(tcpCommand);
                        break;
                    case 1:
                        tcpCommand = "device_connect_btle " + FilteredResponse[4];
                        TCPSendReceive(tcpCommand);
                        break;
                    case 2:
                        tcpCommand = "device_list";
                        TCPSendReceive(tcpCommand);
                        break;
                    case 3:
                        tcpCommand = "device_connect " + FilteredResponse[4];
                        TCPSendReceive(tcpCommand);
                        break;
                    case 4:
                        tcpCommand = "device_subscribe " + DataStream + " ON";
                        tcpSequenceComplete = true;
                        TCPSendReceive(tcpCommand);
                        if (DataStream == "acc")
                        {
                            EDM.AccelerometerChanged += EDM_AccelerometerChanged;
                        }
                        else if (DataStream == "bvp")
                        {
                            EDM.PPGSensorChanged += EDM_PPGChanged;
                        }
                        else if (DataStream == "gsr")
                        {
                            EDM.GSRSensorChanged += EDM_GSRSensorChanged;
                        }
                        else if (DataStream == "ibi")
                        {
                            EDM.IBISensorChanged += EDM_IBISensorChanged;
                        }
                        else if (DataStream == "tmp")
                        {
                            EDM.TemperatureSensorChanged += EDM_TemperatureSensorChanged;
                        }
                        else if (DataStream == "tag")
                        {
                            EDM.TagCreatedEvent += EDM_TagCreatedEvent;
                        }
                        break;
                }
            }
        }

        private void TCPSendReceive(string tcpCommand)
        {
            if (sendDoneBool == true){
                Send(client, tcpCommand + Environment.NewLine);
                SendDone.WaitOne();
                SendDone.Reset();
            }
      
            Receive(client);
            ReceiveDone.WaitOne();
            ReceiveDone.Reset();
        }
        #endregion


        #region Event functions
        private void EDM_TagCreatedEvent(object sender, EmpaticaManager.TagCreatedEventArgs e)
        {
            FilteredResponse[2] = e.Tag.ToString();
        }

        private void EDM_TemperatureSensorChanged(object sender, EmpaticaManager.TemperatureSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.SkinTemperature.ToString();
        }

        private void EDM_IBISensorChanged(object sender, EmpaticaManager.IBISensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.InterBeatInterval.ToString();
            FilteredResponse[3] = e.HearthRateVariability.ToString();
        }

        private void EDM_PPGChanged(object sender, EmpaticaManager.PPGSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.BloodVolumePulse.ToString();
        }

        private void EDM_AccelerometerChanged(object sender, EmpaticaManager.AccelerometerChangedEventArgs e)
        {
            FilteredResponse[2] = e.AccelerometerX.ToString();
            FilteredResponse[3] = e.AccelerometerY.ToString();
            FilteredResponse[4] = e.AccelerometerZ.ToString();
        }

        private void EDM_GSRSensorChanged(object sender, EmpaticaManager.GSRSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.GalvanicSkinResponse.ToString();
        }
        #endregion
    }
}
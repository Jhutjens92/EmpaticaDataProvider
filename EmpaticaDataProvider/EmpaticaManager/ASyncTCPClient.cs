﻿using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace EmpaticaDataProvider.EmpaticaManager
{
    public class ASyncTCPClient
    {
        // The port number for the remote device.
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 5555;
        private string[] FilteredResponse = { };
        public int TCPStep = 0;
        // ManualResetEvent instances signal completion.
        private readonly ManualResetEvent ConnectDone = new ManualResetEvent(false);
        private readonly ManualResetEvent SendDone = new ManualResetEvent(false);
        private readonly ManualResetEvent ReceiveDone = new ManualResetEvent(false);

        // The response from the remote device.
        private String _response = String.Empty;
        EmpaticaDataManager EDM = new EmpaticaDataManager();
        // string containing the parameter Datastream
        string DataStreamCheck;

        public void StartClient(string DataStream)
        {
            DataStreamCheck = DataStream; 
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                
                var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
                var ipAddress = ipHostInfo.AddressList[0];
                var remoteEp = new IPEndPoint(ipAddress, ServerPort);

                // Create a TCP/IP socket.
                var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEp, (ConnectCallback), client);
                ConnectDone.WaitOne();

                while (true)
                {
                    var TCPCommand = "";
                    if (TCPStep == 3)
                    {
                        TCPCommand = "device_subscribe  " + DataStream;
                        TCPStep++;
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
                    }

                    else if (TCPStep == 2)
                    {
                        TCPCommand = "device_connect  " + FilteredResponse[4];
                        TCPStep++;
                    }

                    else if (TCPStep == 1)
                    {
                        TCPCommand = "device_connect_btle " + FilteredResponse[4];
                        TCPStep++;
                    }

                    else if (TCPStep == 0)
                    {
                        TCPCommand = "device_discover_list";
                        TCPStep++;
                    }

                    Send(client, TCPCommand + Environment.NewLine);
                    SendDone.WaitOne();
                    Receive(client);
                    ReceiveDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void EDM_TagCreatedEvent(object sender, EmpaticaDataManager.TagCreatedEventArgs e)
        {
            int i = 1;
            i = e.Tag;
        }

        private void EDM_TemperatureSensorChanged(object sender, EmpaticaDataManager.TemperatureSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.SkinTemperature.ToString();
        }

        private void EDM_IBISensorChanged(object sender, EmpaticaDataManager.IBISensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.InterBeatInterval.ToString();
            FilteredResponse[3] = e.HearthRateVariability.ToString();
        }

        private void EDM_PPGChanged(object sender, EmpaticaDataManager.PPGSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.BloodVolumePulse.ToString();
        }

        private void EDM_AccelerometerChanged (object sender, EmpaticaDataManager.AccelerometerChangedEventArgs e)
        {
            FilteredResponse[2] = e.AccelerometerX.ToString();
            FilteredResponse[3] = e.AccelerometerY.ToString();
            FilteredResponse[4] = e.AccelerometerZ.ToString();
        }

        private void EDM_GSRSensorChanged(object sender, EmpaticaDataManager.GSRSensorChangedEventArgs e)
        {
            FilteredResponse[2] = e.GalvanicSkinResponse.ToString();
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

                    HandleResponseFromEmpaticaBLEServer(_response);

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
                         FilteredResponse = _response.Split(null);
                    }
                    // Signal that all bytes have been received.
                    ReceiveDone.Set();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0, SendCallback, client);
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

        private void HandleResponseFromEmpaticaBLEServer(string response)
        {
            Console.Write(response);
        }
    }
}
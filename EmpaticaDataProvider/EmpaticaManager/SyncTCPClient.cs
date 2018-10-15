using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using EmpaticaDataProvider.EmpaticaManager;
using EmpaticaDataProvider.ViewModel;

public class SyncTCPClient
{
    #region Vars
    private const string serverAddress = "127.0.0.1";
    private const int serverPort = 5555;
    private string[] filteredResponse = { };
    private int tcpStep = 0;

    EmpaticaDataManager EDM = new EmpaticaDataManager();
    // string containing the parameter Datastream
    string dataStreamCheck;

    #endregion

    #region Events
    private void EDM_TagCreatedEvent(object sender, EmpaticaDataManager.TagCreatedEventArgs e)
    {
        filteredResponse[2] = e.Tag.ToString();
    }

    private void EDM_TemperatureSensorChanged(object sender, EmpaticaDataManager.TemperatureSensorChangedEventArgs e)
    {
        filteredResponse[2] = e.SkinTemperature.ToString();
    }

    private void EDM_IBISensorChanged(object sender, EmpaticaDataManager.IBISensorChangedEventArgs e)
    {
        filteredResponse[2] = e.InterBeatInterval.ToString();
        filteredResponse[3] = e.HearthRateVariability.ToString();
    }

    private void EDM_PPGChanged(object sender, EmpaticaDataManager.PPGSensorChangedEventArgs e)
    {
        filteredResponse[2] = e.BloodVolumePulse.ToString();
    }

    private void EDM_AccelerometerChanged(object sender, EmpaticaDataManager.AccelerometerChangedEventArgs e)
    {
        filteredResponse[2] = e.AccelerometerX.ToString();
        filteredResponse[3] = e.AccelerometerY.ToString();
        filteredResponse[4] = e.AccelerometerZ.ToString();
    }

    private void EDM_GSRSensorChanged(object sender, EmpaticaDataManager.GSRSensorChangedEventArgs e)
    {
        filteredResponse[2] = e.GalvanicSkinResponse.ToString();
    }
    #endregion

    #region Functions
    public void StartClient(string dataStream)
    {
        dataStreamCheck = dataStream;
        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  .  
            var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(serverAddress) } };
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEp = new IPEndPoint(ipAddress, serverPort);

            // Create a TCP/IP  socket.  
            var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                client.Connect(remoteEp);
                Console.WriteLine("Socket connected to {0}",
                    client.RemoteEndPoint.ToString());
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
            while (Globals.IsRecordingData == true)
            {
                SendTCPCommand(client, CheckTCPCommand());
                ReceiveEmpaticaData(client);
                CheckSendStep();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private string CheckTCPCommand()
    {
        var tcpCommand = "";
        switch (tcpStep)
        {
            case 0:
                tcpCommand = "device_discover_list";
                break;
            case 1:
                tcpCommand = "device_connect_btle " + filteredResponse[4];
                break;
            case 2:
                tcpCommand = "device_list";
                break;

            case 3:
                tcpCommand = "device_connect " + filteredResponse[4];
                break;
            case 4:
                tcpCommand = "device_subscribe " + dataStreamCheck + " ON";
                if (dataStreamCheck == "acc")
                {
                    EDM.AccelerometerChanged += EDM_AccelerometerChanged;
                }
                else if (dataStreamCheck == "bvp")
                {
                    EDM.PPGSensorChanged += EDM_PPGChanged;
                }
                else if (dataStreamCheck == "gsr")
                {
                    EDM.GSRSensorChanged += EDM_GSRSensorChanged;
                }
                else if (dataStreamCheck == "ibi")
                {
                    EDM.IBISensorChanged += EDM_IBISensorChanged;
                }
                else if (dataStreamCheck == "tmp")
                {
                    EDM.TemperatureSensorChanged += EDM_TemperatureSensorChanged;
                }
                else if (dataStreamCheck == "tag")
                {
                    EDM.TagCreatedEvent += EDM_TagCreatedEvent;
                }
                break;
        }
        return tcpCommand;

    }

    private void SendTCPCommand(Socket client, String data)
    {
        // Encode the data string into a byte array.  
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Send the data through the socket.  
        int bytesSent = client.Send(byteData);

    }

    private void ReceiveEmpaticaData(Socket client)
    {
        // Data buffer for incoming data.  
        byte[] bytes = new byte[1024];
        // Receive the response from the remote device.  
        int bytesRec = client.Receive(bytes);
        string Response = Encoding.ASCII.GetString(bytes, 0, bytesRec);
        filteredResponse = Response.Split(null);
    }

    private void CloseSocket(Socket client)
    {
        // Release the socket.  
        client.Shutdown(SocketShutdown.Both);
        client.Close();
    }

    private void CheckSendStep()
    {
        if (filteredResponse[4] != "")
        {
            tcpStep = 1;
        }
        else if (filteredResponse[2] == "OK")
        {
            tcpStep = 2;
        }
        else if (filteredResponse[4] != "")
        {
            tcpStep = 3;
        }
        else if (filteredResponse[2] == "OK")
        {
            tcpStep = 4;
        }

    }
    #endregion
}
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using EmpaticaDataProvider.Classes;

public class SynchronousTCPClient
{
    #region Variables
    // The server ip for the BLE Empatica Server.
    private const string ServerAddress = "127.0.0.1";
    // The port number for the BLE Empatica Server.
    private const int ServerPort = 5555;
    bool TCPClientConnected = false;

    // String containing the filtered message split on null. 
    private static string[] ReceivedStrFiltered = { };
    // Int step counter to verify what to send next.
    private static int tcpStep = 0;

    #endregion

    #region Events
    public class TagCreatedEventArgs : EventArgs
    {

        private int _Tag;
        public int Tag
        {
            get { return _Tag; }
            set { _Tag = value; }
        }
    }

    public class PPGSensorChangedEventArgs : EventArgs
    {
        private float _BloodVolumePulse;
        public float BloodVolumePulse
        {
            get { return _BloodVolumePulse; }
            set { _BloodVolumePulse = value; }
        }
    }

    public class IBISensorChangedEventArgs : EventArgs
    {
        private float _InterBeatInterval;
        public float InterBeatInterval
        {
            get { return _InterBeatInterval; }
            set { _InterBeatInterval = value; }
        }
        private float _HearthRateVariability;
        public float HearthRateVariability
        {
            get { return _HearthRateVariability; }
            set { _HearthRateVariability = value; }
        }
    }

    public class AccelerometerChangedEventArgs : EventArgs
    {
        private float _AccelerometerX;
        public float AccelerometerX
        {
            get { return _AccelerometerX; }
            set { _AccelerometerX = value; }
        }
        private float _AccelerometerY;
        public float AccelerometerY
        {
            get { return _AccelerometerY; }
            set { _AccelerometerY = value; }
        }

        private float _AccelerometerZ;
        public float AccelerometerZ
        {
            get { return _AccelerometerZ; }
            set { _AccelerometerZ = value; }
        }
    }

    public class GSRSensorChangedEventArgs : EventArgs
    {
        private float _GalvanicSkinResponse;
        public float GalvanicSkinResponse
        {
            get { return _GalvanicSkinResponse; }
            set { _GalvanicSkinResponse = value; }
        }
    }

    public class TemperatureSensorChangedEventArgs : EventArgs
    {
        private float _SkinTemperature;
        public float SkinTemperature
        {
            get { return _SkinTemperature; }
            set { _SkinTemperature = value; }
        }
    }

    public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;
    public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;
    public event EventHandler<PPGSensorChangedEventArgs> PPGSensorChanged;
    public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;
    public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;
    public event EventHandler<TagCreatedEventArgs> TagCreatedEvent;

    protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
    {
        AccelerometerChanged?.Invoke(this, e);
    }

    protected virtual void OnPPGSensorChanged(PPGSensorChangedEventArgs e)
    {
        PPGSensorChanged?.Invoke(this, e);
    }

    protected virtual void OnIBISensorChanged(IBISensorChangedEventArgs e)
    {
        IBISensorChanged?.Invoke(this, e);
    }

    protected virtual void OnGSRSensorChanged(GSRSensorChangedEventArgs e)
    {
        GSRSensorChanged?.Invoke(this, e);
    }

    protected virtual void OnTemperatureSensorChanged(TemperatureSensorChangedEventArgs e)
    {
        TemperatureSensorChanged?.Invoke(this, e);
    }

    protected virtual void OnTagCreated(TagCreatedEventArgs e)
    {
        TagCreatedEvent?.Invoke(this, e);
    }
    #endregion

    #region Methods
    // Create a TCP/IP  socket.  
    static Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private static void StartSyncTCPClient()
    {
        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, ServerPort);

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

    public static void CloseTCPConnection()
    {
        try
        {
            // Release the socket.  
            client.Shutdown(SocketShutdown.Both);
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static void SendTCPMessage(string TCPCommandStr)
    {
        try
        {
            // Encode the data string into a byte array.  
            byte[] TCPCommandByte = Encoding.ASCII.GetBytes(TCPCommandStr + Environment.NewLine);
            Console.WriteLine("Sending the following command: {0}", TCPCommandStr);
            // Send the data through the socket.  
            client.Send(TCPCommandByte);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static string ReceiveTCPMessage()
    {
        // Data buffer converted to string.
        string receivedStr = string.Empty;
        // Data buffer for incoming data.  
        byte[] receivedBytes = new byte[64];

        try
        {
            // Receive the response from the remote device.  
            int bytesSend = client.Receive(receivedBytes);
            receivedStr = Encoding.UTF8.GetString(receivedBytes, 0, bytesSend);
            Console.WriteLine("Received the following message: {0}", receivedStr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        return receivedStr;
    }

    public void TCPMain(string DataStream)
    {
        if (TCPClientConnected == false)
        {
            StartSyncTCPClient();
            TCPClientConnected = true;
        }
        SendTCPMessage(CreateTcpCmd(DataStream));
        CheckReceivedMessage(ReceiveTCPMessage());
    }

    public static void CheckReceivedMessage(string ReceivedStr)
    {
        ReceivedStrFiltered = ReceivedStr.Split(null);
        if (ReceivedStrFiltered[4] != "")
        {
            tcpStep = 1;
        }
        else if (ReceivedStrFiltered[2] == "OK")
        {
            tcpStep = 2;
        }
        else if (ReceivedStrFiltered[4] != "")
        {
            tcpStep = 3;
        }
        else if (ReceivedStrFiltered[2] == "OK")
        {
            tcpStep = 4;
        }
    }

    public string CreateTcpCmd(string DataStream)
    {
        var TCPCommandStr = "";
        switch (tcpStep)
        {
            case 0:
                TCPCommandStr = "device_discover_list";
                break;
            case 1:
                TCPCommandStr = "device_connect_btle " + ReceivedStrFiltered[4];
                break;
            case 2:
                TCPCommandStr = "device_list";
                break;
            case 3:
                TCPCommandStr = "device_connect " + ReceivedStrFiltered[4];
                break;
            case 4:
                TCPCommandStr = "device_subscribe " + DataStream + " ON";
                if (DataStream == "acc")
                {
                    AccelerometerChanged += EDM_AccelerometerChanged;
                }
                else if (DataStream == "bvp")
                {
                    PPGSensorChanged += EDM_PPGChanged;
                }
                else if (DataStream == "gsr")
                {
                    GSRSensorChanged += EDM_GSRSensorChanged;
                }
                else if (DataStream == "ibi")
                {
                    IBISensorChanged += EDM_IBISensorChanged;
                }
                else if (DataStream == "tmp")
                {
                    TemperatureSensorChanged += EDM_TemperatureSensorChanged;
                }
                else if (DataStream == "tag")
                {
                    TagCreatedEvent += EDM_TagCreatedEvent;
                }
                break;
        }
        return TCPCommandStr;
    }
    #endregion  

    #region Event methods
    private void EDM_TagCreatedEvent(object sender, TagCreatedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.Tag.ToString();
    }

    private void EDM_TemperatureSensorChanged(object sender, TemperatureSensorChangedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.SkinTemperature.ToString();
    }

    private void EDM_IBISensorChanged(object sender, IBISensorChangedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.InterBeatInterval.ToString();
        ReceivedStrFiltered[3] = e.HearthRateVariability.ToString();
    }

    private void EDM_PPGChanged(object sender, PPGSensorChangedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.BloodVolumePulse.ToString();
    }

    private void EDM_AccelerometerChanged(object sender, AccelerometerChangedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.AccelerometerX.ToString();
        ReceivedStrFiltered[3] = e.AccelerometerY.ToString();
        ReceivedStrFiltered[4] = e.AccelerometerZ.ToString();
    }

    private void EDM_GSRSensorChanged(object sender, GSRSensorChangedEventArgs e)
    {
        ReceivedStrFiltered[2] = e.GalvanicSkinResponse.ToString();
    }
    #endregion
}
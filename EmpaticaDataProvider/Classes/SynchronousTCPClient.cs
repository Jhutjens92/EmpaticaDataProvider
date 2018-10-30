using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using EmpaticaDataProvider.Classes;
using EmpaticaDataProvider.ViewModel;

public class SynchronousTCPClient
{
    #region Variables
    // The server ip for the BLE Empatica Server.
    private const string ServerAddress = "127.0.0.1";
    // The port number for the BLE Empatica Server.
    private const int ServerPort = 5555;
    bool TCPClientConnected = false;

    // String containing the filtered message split on null. 
    private string[] ReceivedStrFiltered = { };
    // Int step counter to verify what to send next.
    private int tcpStep = 0;

    string DataStreamStored;

    #endregion

    #region Events
    public class TagCreatedEventArgs : EventArgs
    {

        private int tag;
        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }

    public class BVPSensorChangedEventArgs : EventArgs
    {
        private float bloodVolumePulse;
        public float BloodVolumePulse
        {
            get { return bloodVolumePulse; }
            set { bloodVolumePulse = value; }
        }
    }

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

    public class GSRSensorChangedEventArgs : EventArgs
    {
        private float gsr;
        public float GSR
        {
            get { return gsr; }
            set { gsr = value; }
        }
    }

    public class TemperatureSensorChangedEventArgs : EventArgs
    {
        private float skinTemperature;
        public float SkinTemperature
        {
            get { return skinTemperature; }
            set { skinTemperature = value; }
        }
    }

    public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;
    public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;
    public event EventHandler<BVPSensorChangedEventArgs> BVPSensorChanged;
    public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;
    public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;
    public event EventHandler<TagCreatedEventArgs> TagCreatedEvent;

    protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
    {
        AccelerometerChanged?.Invoke(this, e);
    }

    protected virtual void OnBVPSensorChanged(BVPSensorChangedEventArgs e)
    {
        BVPSensorChanged?.Invoke(this, e);
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

    private void StartSyncTCPClient(string DataStream)
    {
        // Connect to a remote device.  
        try
        {
            // Establish the remote endpoint for the socket.  
            var ipHostInfo = new IPHostEntry { AddressList = new[] { IPAddress.Parse(ServerAddress) } };
            var ipAddress = ipHostInfo.AddressList[0];
            var remoteEP = new IPEndPoint(ipAddress, ServerPort);
            DataStreamStored = DataStream;

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

    private void SendTCPMessage(string TCPCommandStr)
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

    private string ReceiveTCPMessage()
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
            ReceivedStrFiltered = receivedStr.Split(null);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        return receivedStr;
    }

    public void ConnectEmpatica(string DataStream)
    {
        try
        {
            if (TCPClientConnected == false)
            {
                StartSyncTCPClient(DataStream);
                TCPClientConnected = true;
            }
            while (tcpStep < 5)
            {
                SendTCPMessage(CreateTcpCmd());
                ReceiveTCPMessage();
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
        while (Globals.IsRecordingData == true)
        {
            ReceiveTCPMessage();
            switch (DataStreamStored)
            {
                case "acc":
                    UpdateAccValues();
                    break;
                case "bvp":
                    UpdateBvpValues();
                    break;
                case "ibi":
                    UpdateIbiValues();
                    break;
                case "gsr":
                    UpdateGsrValues();
                    break;
                case "tag":
                    UpdateTagValues();
                    break;
                case "tmp":
                    UpdateTempValues();
                    break;
            }


        }
    }

    private void ChkReceivedMsg()
    {
        switch (tcpStep)
        {
            case 0:
                if (int.Parse(ReceivedStrFiltered[2]) > 0) tcpStep = 1;
                break;
            case 1:
                if (ReceivedStrFiltered[2] == "OK") tcpStep = 2;
                break;
            case 2:
                if (Int32.Parse(ReceivedStrFiltered[2]) > 0) tcpStep = 3;
                break;
            case 3:
                if (ReceivedStrFiltered[2] == "OK") tcpStep = 4;
                break;
            case 4:
                if (ReceivedStrFiltered[3] == "OK") tcpStep = 5;
                break;
        }
    }

    private string CreateTcpCmd()
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
                TCPCommandStr = "device_subscribe " + DataStreamStored + " ON";
                break;
        }
        return TCPCommandStr;
    }

    private void UpdateAccValues()
    {
        try
        {

            AccelerometerChangedEventArgs args = new AccelerometerChangedEventArgs
            {
                AccelerometerX = float.Parse(ReceivedStrFiltered[2]),
                AccelerometerY = float.Parse(ReceivedStrFiltered[3]),
                AccelerometerZ = float.Parse(ReceivedStrFiltered[4])

            };
            OnAccelerometerChanged(args);
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

    private void UpdateBvpValues()
    {
        try
        {

            BVPSensorChangedEventArgs args = new BVPSensorChangedEventArgs
            {
                BloodVolumePulse = float.Parse(ReceivedStrFiltered[2]),

            };
            OnBVPSensorChanged(args);
        }
        catch (Exception)
        {
            BVPSensorChangedEventArgs args = new BVPSensorChangedEventArgs
            {
                BloodVolumePulse = 0.0F,
            };
            OnBVPSensorChanged(args);
        }
    }

    private void UpdateIbiValues()
    {
        try
        {

            IBISensorChangedEventArgs args = new IBISensorChangedEventArgs
            {
                InterBeatInterval = float.Parse(ReceivedStrFiltered[2]),
                HearthRateVariability = float.Parse(ReceivedStrFiltered[3]),
            };
            OnIBISensorChanged(args);
        }
        catch (Exception)
        {
            IBISensorChangedEventArgs args = new IBISensorChangedEventArgs
            {
                InterBeatInterval = 0.0F,
                HearthRateVariability = 0.0F,
            };
            OnIBISensorChanged(args);
        }
    }

    private void UpdateGsrValues()
    {
        try
        {

            GSRSensorChangedEventArgs args = new GSRSensorChangedEventArgs
            {
                GSR = float.Parse(ReceivedStrFiltered[2]),
            };
            OnGSRSensorChanged(args);
        }
        catch (Exception)
        {
            GSRSensorChangedEventArgs args = new GSRSensorChangedEventArgs
            {
                GSR = 0.0F,
            };
            OnGSRSensorChanged(args);
        }
    }

    private void UpdateTagValues()
    {
        try
        {

            TagCreatedEventArgs args = new TagCreatedEventArgs
            {
                Tag = int.Parse(ReceivedStrFiltered[2]),


            };
            OnTagCreated(args);
        }
        catch (Exception)
        {
            TagCreatedEventArgs args = new TagCreatedEventArgs
            {
                Tag = 0,

            };
            OnTagCreated(args);
        }
    }

    private void UpdateTempValues()
    {
        try
        {

            TemperatureSensorChangedEventArgs args = new TemperatureSensorChangedEventArgs
            {
                SkinTemperature = float.Parse(ReceivedStrFiltered[2]),
            };
            OnTemperatureSensorChanged(args);
        }
        catch (Exception)
        {
            TemperatureSensorChangedEventArgs args = new TemperatureSensorChangedEventArgs
            {
                SkinTemperature = 0.0F,
            };
            OnTemperatureSensorChanged(args);
        }
    }
    #endregion
}
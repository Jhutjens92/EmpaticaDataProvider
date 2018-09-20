using MQTTDataProvider.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTTDataProvider.MQTTManager
{
    class MqttDataManager
    {

        MqttClient client;
        string clientId;
        string ReceivedMessage;             //string containing the UDP published message
        string BrokerAddress;               //default MQTT server value for WEKIT
        dynamic Parsed_ReceivedMessage;     //JSON Parser MQTT message

        UdpClient udpClient = new UdpClient(11000);
        public event EventHandler<TextReceivedEventArgs> NewMqttTextReceived;

        private string _txtReceived = " ";
        public string TxtReceived
        {
            get { return _txtReceived; }
            set
            {
                _txtReceived = value;

            }
        }


        protected virtual void OnNewTextReceived(TextReceivedEventArgs e)
        {
            EventHandler<TextReceivedEventArgs> handler = NewMqttTextReceived;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        public class TextReceivedEventArgs : EventArgs
        {
            public string GSR { get; set; }
            public string TextReceived { get; set; }
        }


        public MqttDataManager()
        {
            //INIT Var Values//
            BrokerAddress = "localhost";
            //MQTT Functions//
            client = new MqttClient(BrokerAddress);
            // use a unique id as client id, each time we start the application
            clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
        }


        #region Methods
        // this code runs when a message was received
        void Client_UDPMsgReceived()
        {     
            if (Globals.IsRecordingUDP == true)
            {
                JSONParse_ReceivedMessage();
                Publish_Data();
                TextReceivedEventArgs args = new TextReceivedEventArgs();
                args.TextReceived = ReceivedMessage;
                OnNewTextReceived(args);
            }

        }

        public String UpdateText()
        {
            return TxtReceived;
        }

        //parse MQTT JSON String
        void JSONParse_ReceivedMessage()
        {
            Parsed_ReceivedMessage = JObject.Parse(ReceivedMessage);
        }

        void UDPMsgReceive()
        {
            try
            {
                udpClient.Connect("127.0.0.1", 11000);
                //IPEndPoint object will allow us to read datagrams sent from any source.
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // Blocks until a message returns on this socket from a remote host.
                Byte[] receiveBytes = udpClient.Receive(ref RemoteIpEndPoint);
                ReceivedMessage = Encoding.ASCII.GetString(receiveBytes);

                // Uses the IPEndPoint object to determine which of these two hosts responded.
                Console.WriteLine("This is the message you received " + ReceivedMessage.ToString());
                Console.WriteLine("This message was sent from " + RemoteIpEndPoint.Address.ToString() + " on their port number " + RemoteIpEndPoint.Port.ToString());              
                udpClient.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    #endregion

    #region MQTT

        // this code runs when data is published to the subscribed topic
        private void Publish_Data()
        {
            // whole topic
            string GSR_Value = Parsed_ReceivedMessage.gsr;
            client.Publish("wekit/vest/GSR_Raw", Encoding.UTF8.GetBytes(GSR_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            string Pulse_Value = Parsed_ReceivedMessage.pulse;
            client.Publish("wekit/vest/Pulse_Raw", Encoding.UTF8.GetBytes(Pulse_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            string SHT1X1_Temp_Value = Parsed_ReceivedMessage.shts[0].temp;
            client.Publish("wekit/vest/Sht0_Temp", Encoding.UTF8.GetBytes(SHT1X1_Temp_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            string SHT1X1_Hum_Value = Parsed_ReceivedMessage.shts[0].hum;
            client.Publish("wekit/vest/Sht0_Hum", Encoding.UTF8.GetBytes(SHT1X1_Hum_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            string SHT2X2_Temp_Value = Parsed_ReceivedMessage.shts[1].temp;
            client.Publish("wekit/vest/Sht2_Temp", Encoding.UTF8.GetBytes(SHT2X2_Temp_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);

            string SHT2X2_Hum_Value = Parsed_ReceivedMessage.shts[1].hum;
            client.Publish("wekit/vest/Sht2_Hum", Encoding.UTF8.GetBytes(SHT2X2_Hum_Value), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
        }

        #endregion
    }
}

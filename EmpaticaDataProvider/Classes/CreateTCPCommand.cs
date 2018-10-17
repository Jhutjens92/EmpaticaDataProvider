using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Classes
{ 

    class CreateTCPCommand
    {
        // String containing the filtered message split on null. 
        private static string[] FilteredResponse = { };
        // Int step counter to verify what to send next.
        private static int tcpStep = 0;

        EmpaticaManager EDM = new EmpaticaManager();

        public static void CheckReceivedMessage()
        {
            if (FilteredResponse[4] != "")
            {
                tcpStep = 1;
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

        public void MakeTCPCommands(string DataStream)
        {
            var tcpCommand = "";
            switch (tcpStep)
            {
                case 0:
                    tcpCommand = "device_discover_list";
                    break;
                case 1:
                    tcpCommand = "device_connect_btle " + FilteredResponse[4];
                    break;
                case 2:
                    tcpCommand = "device_list";
                    break;
                case 3:
                    tcpCommand = "device_connect " + FilteredResponse[4];
                    break;
                case 4:
                    tcpCommand = "device_subscribe " + DataStream + " ON";
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

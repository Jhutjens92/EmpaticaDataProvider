using EmpaticaDataProvider.Model;
using System;
using System.Collections.Generic;
using static EmpaticaDataProvider.Classes.TCPHandler;

namespace EmpaticaDataProvider.Classes
{
    /// <summary>   Class containing functions to set the values and names used in the Learning Hub JSON File. </summary>
    ///
    /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>  
    class SetLHJson
    {
        #region Methods

        /// <summary>   Sets value names. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 8-11-2018. </remarks>
        public void SetValueNames()
        {
            try
            {
                List<string> names = new List<string>();
                if (CheckParameters.Instance.DataStream == "acc")
                {
                    names.Add("Empatica_AccX");
                    names.Add("Empatica_AccY");
                    names.Add("Empatica_AccZ");
                }
                if (CheckParameters.Instance.DataStream == "tmp")
                {
                    names.Add("Empatica_Temp");
                }
                if (CheckParameters.Instance.DataStream == "bvp")
                {
                    names.Add("Empatica_Bvp");
                }
                if (CheckParameters.Instance.DataStream == "ibi")
                {
                    names.Add("Empatica_Ibi");
                }
                if (CheckParameters.Instance.DataStream == "tag")
                {
                    names.Add("Empatica_Tag");
                }
                HubConnector.SetValuesName(names);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>Sends a data to lh.</summary>
        ///
        /// <remarks>Jordi Hutjens, 9-11-2018.</remarks>
        ///
        /// <param name="acc">Accelerometer changed event information.</param>

        public void SendAccDataToLH(AccelerometerChangedEventArgs acc)
        {
            try
            {
                var values = new List<string>
                {
                    acc.AccelerometerX.ToString(),
                    acc.AccelerometerY.ToString(),
                    acc.AccelerometerZ.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendTmpDataToLH(TemperatureSensorChangedEventArgs tmp)
        {
            try
            {
                var values = new List<string>
                {
                    tmp.SkinTemperature.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendIbiDataToLH(IBISensorChangedEventArgs ibi)
        {
            try
            {
                var values = new List<string>
                {
                    ibi.InterBeatInterval.ToString(),
                    ibi.HearthRateVariability.ToString()

                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendGsrDataToLH(GSRSensorChangedEventArgs gsr)
        {
            try
            {
                var values = new List<string>
                {
                    gsr.GalvanicSkinResponse.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendTagDataToLH(TagCreatedEventArgs tag)
        {
            try
            {
                var values = new List<string>
                {
                    tag.Tag.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void SendBvpDataToLH(BVPSensorChangedEventArgs bvp)
        {
            try
            {
                var values = new List<string>
                {
                    bvp.BloodVolumePulse.ToString()
                };
                HubConnector.SendData(values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        #endregion
    }
}



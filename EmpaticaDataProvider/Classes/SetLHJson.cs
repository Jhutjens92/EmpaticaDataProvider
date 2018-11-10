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
                var names = new List<string>
                {
                    "Empatica_AccX",
                    "Empatica_AccY",
                    "Empatica_AccZ",
                };

                //List<string> names = new List<string>();
                //if (chkpara.DataStream == "acc")
                //{
                //    names.Add("Empatica_AccX");
                //    names.Add("Empatica_AccY");
                //    names.Add("Empatica_AccZ");
                //}
                //if (chkpara.DataStream == "tmp")
                //{
                //    names.Add("Empatica_Temp");
                //}
                //if (chkpara.DataStream == "bvp")
                //{
                //    names.Add("Empatica_Bvp");
                //}
                //if (chkpara.DataStream == "ibi")
                //{
                //    names.Add("Empatica_Ibi");
                //}
                //if (chkpara.DataStream == "tag")
                //{
                //    names.Add("Empatica_Tag");
                //}
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

        public void SendDataToLH(AccelerometerChangedEventArgs acc)
        {
            try
            {
                var values = new List<string>
                {
                    acc.AccelerometerX.ToString(),
                    acc.AccelerometerY.ToString(),
                    acc.AccelerometerZ.ToString(),
                };

                //List<string> values = new List<string>();
                //if (chkpara.DataStream == "acc")
                //{
                //    values.Add(acc.AccelerometerX.ToString());
                //    values.Add(acc.AccelerometerY.ToString());
                //    values.Add(acc.AccelerometerZ.ToString());
                //}
                //if (chkpara.DataStream == "tmp")
                //{
                //    values.Add(tmp.SkinTemperature.ToString());
                //}
                //if (chkpara.DataStream == "bvp")
                //{
                //    values.Add(bvp.BloodVolumePulse.ToString());
                //}
                //if (chkpara.DataStream == "ibi")
                //{
                //    values.Add(ibi.InterBeatInterval.ToString());
                //    values.Add(ibi.HearthRateVariability.ToString());
                //}
                //if (chkpara.DataStream == "gsr")
                //{
                //    values.Add(gsr.GalvanicSkinResponse.ToString());
                //}
                //if (chkpara.DataStream == "tag")
                //{
                //    values.Add(tag.Tag.ToString());
                //}
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



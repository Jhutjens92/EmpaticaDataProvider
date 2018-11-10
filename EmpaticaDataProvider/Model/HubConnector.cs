using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Model
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Hub Connector developed by Jan Schneider </summary>
    ///
    /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    static class HubConnector
    {
        private static ConnectorHub.ConnectorHub myConnector;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets my connector. </summary>
        ///
        /// <value> my connector. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static ConnectorHub.ConnectorHub MyConnector
        {
            get
            {
                if (myConnector == null)
                {
                    StartConnection();
                }
                return myConnector;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method to start the connection to the Learning Hub. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void StartConnection()
        {
            myConnector = new ConnectorHub.ConnectorHub();

            MyConnector.init();
            MyConnector.sendReady();

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method to send the data to the Learning Hub. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>
        ///
        /// <param name="values">   The values. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SendData(List<string> values)
        {
            MyConnector.storeFrame(values);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method to start the set the value descriptions used in the JSON file. </summary>
        ///
        /// <remarks>   Jordi Hutjens, 26-10-2018. </remarks>
        ///
        /// <param name="names">    The names. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static void SetValuesName(List<string> names)
        {
            MyConnector.setValuesName(names);
        }
    }
}

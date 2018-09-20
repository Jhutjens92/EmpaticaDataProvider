using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Model
{
    static class HubConnector
    {
        private static ConnectorHub.ConnectorHub _myConnector;

        public static ConnectorHub.ConnectorHub MyConnector {
            get {
                if (_myConnector == null)
                {
                    StartConnection();
                }
                return _myConnector;
            } }

        public static void StartConnection()
        {
            _myConnector = new ConnectorHub.ConnectorHub();

            MyConnector.init();
            MyConnector.sendReady();

        }

        public static void SendData(List<string> values)
        {
            MyConnector.storeFrame(values);
        }

        public static void SetValuesName(List<string> names)
        {
            MyConnector.setValuesName(names);
        }
    }
}

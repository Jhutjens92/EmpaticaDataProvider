using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Classes
{
    class Datastreams
    {
        public static void DataStreamAcc()
        {
            string DataStream = "acc";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public static void DataStreamBVP()
        {
            string DataStream = "bvp";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public static void DataStreamGSR()
        {
            string DataStream = "gsr";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public static void DataStreamIBI()
        {
            string DataStream = "ibi";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public static void DataStreamTMP()
        {
            string DataStream = "tmp";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public static void DataStreamTag()
        {
            string DataStream = "tag";
            SynchronousTCPClient instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }
    }
}

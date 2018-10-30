using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Classes
{
    class Datastreams
    {
        public SynchronousTCPClient instance = new SynchronousTCPClient();
        public SynchronousTCPClient instance2 = new SynchronousTCPClient();

        public void DataStreamAcc()
        {
            string DataStream = "acc";
            //instance = new SynchronousTCPClient();
            instance.ConnectEmpatica(DataStream);
        }

        public void DataStreamBVP()
        {
            string DataStream = "bvp";
            //instance2 = new SynchronousTCPClient();
            instance2.ConnectEmpatica(DataStream);
        }

            //}

            //static void DataStreamGSR()
            //{
            //    string DataStream = "gsr";
            //    ASyncTCPClient instance = new ASyncTCPClient();
            //    instance.StartClient(DataStream);
            //    Console.WriteLine("GSR Datastream started");

            //}

            //static void DataStreamIBI()
            //{
            //    string DataStream = "ibi";
            //    ASyncTCPClient instance = new ASyncTCPClient();
            //    instance.StartClient(DataStream);
            //    Console.WriteLine("IBI Datastream started");

            //}

            //static void DataStreamTMP()
            //{
            //    string DataStream = "tmp";
            //    ASyncTCPClient instance = new ASyncTCPClient();
            //    instance.StartClient(DataStream);
            //    Console.WriteLine("TEMP Datastream started");

            //}

            //static void DataStreamTag()
            //{
            //    string DataStream = "tag";
            //    ASyncTCPClient instance = new ASyncTCPClient();
            //    instance.StartClient(DataStream);
            //    Console.WriteLine("TAG Datastream started");
        }
}

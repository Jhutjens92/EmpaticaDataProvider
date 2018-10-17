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
            instance.TCPRoutine(DataStream);
        }

        //static void DataStreamBVP()
        //{
        //    string DataStream = "bvp";
        //    ASyncTCPClient instance = new ASyncTCPClient();
        //    instance.StartClient(DataStream);
        //    Console.WriteLine("BVP Datastream started");

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

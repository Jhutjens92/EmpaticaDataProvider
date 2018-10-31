using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.Classes
{
    class Datastreams
    {
        public SynchronousTCPClient instance1 = new SynchronousTCPClient();
        public SynchronousTCPClient instance2 = new SynchronousTCPClient();
        public SynchronousTCPClient instance3 = new SynchronousTCPClient();
        public SynchronousTCPClient instance4 = new SynchronousTCPClient();
        public SynchronousTCPClient instance5 = new SynchronousTCPClient();
        public SynchronousTCPClient instance6 = new SynchronousTCPClient();


        public void DataStreamAcc()
        {
            string DataStream = "acc";
            instance1.ConnectEmpatica(DataStream);
        }

        public void DataStreamBVP()
        {
            string DataStream = "bvp";
            instance2.ConnectEmpatica(DataStream);
        }


        public void DataStreamGSR()
        {
            string DataStream = "gsr";
            instance3.ConnectEmpatica(DataStream);
        }

        public void DataStreamIBI()
        {
            string DataStream = "ibi";
            instance4.ConnectEmpatica(DataStream);
        }

        public void DataStreamTMP()
        {
            string DataStream = "tmp";
            instance5.ConnectEmpatica(DataStream);
        }

        public void DataStreamTag()
        {
            string DataStream = "tag";
            instance6.ConnectEmpatica(DataStream);
        }
    }
}

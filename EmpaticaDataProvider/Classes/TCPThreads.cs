using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using EmpaticaDataProvider.ViewModel;


namespace EmpaticaDataProvider.Classes
{
    /// <summary>
    /// What does this class do
    /// </summary>
    class TCPThreads
    {
        Datastreams datastream = new Datastreams(); 
        public void CreateTCPThreads()
        {
            datastream.DataStreamAcc();
            //Thread EDPThread1 = new Thread(new ThreadStart(Datastreams.DataStreamAcc));
            //Thread EDPThread2 = new Thread(new ThreadStart(Datastreams.DataStreamBVP));
            //Thread EDPThread3 = new Thread(new ThreadStart(Datastreams.DataStreamGSR));
            //Thread EDPThread4 = new Thread(new ThreadStart(Datastreams.DataStreamIBI));
            //Thread EDPThread5 = new Thread(new ThreadStart(Datastreams.DataStreamTMP));
            //Thread EDPThread6 = new Thread(new ThreadStart(Datastreams.DataStreamTag))
        }

        public void StartTcpThreads()
        {
            //EDPThread1.Start();
            //EDPThread2.Start();
            //EDPThread3.Start();
            //EDPThread4.Start();
            //EDPThread5.Start();
            //EDPThread6.Start();
        }
    }
}

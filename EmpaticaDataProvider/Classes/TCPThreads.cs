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
        #region Threads
        Thread EDPThread1;
        Thread EDPThread2;
        Thread EDPThread3;
        Thread EDPThread4;
        Thread EDPThread5;
        Thread EDPThread6;
        #endregion

        public void CreateTCPThreads()
        {
            EDPThread1 = new Thread(new ThreadStart(Datastreams.DataStreamAcc));
            //EDPThread2 = new Thread(new ThreadStart(Datastreams.DataStreamBVP));
            //EDPThread3 = new Thread(new ThreadStart(Datastreams.DataStreamGSR));
            //EDPThread4 = new Thread(new ThreadStart(Datastreams.DataStreamIBI));
            //EDPThread5 = new Thread(new ThreadStart(Datastreams.DataStreamTMP));
            //EDPThread6 = new Thread(new ThreadStart(Datastreams.DataStreamTag));
        }

        public void StartTcpThreads()
        {
            EDPThread1.Start();
            //EDPThread2.Start();
            //EDPThread3.Start();
            //EDPThread4.Start();
            //EDPThread5.Start();
            //EDPThread6.Start();
        }
    }
}

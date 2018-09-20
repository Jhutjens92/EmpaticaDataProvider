using EmpaticaDataProvider.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EmpaticaDataProvider.EmpaticaManager
{
    class EmpaticaDataManager
    {
        public event EventHandler<TextReceivedEventArgs> NewEmpaticaDataReceived;

        private string _txtReceived = " ";
        public string TxtReceived
        {
            get { return _txtReceived; }
            set
            {
                _txtReceived = value;

            }
        }


        protected virtual void OnNewTextReceived(TextReceivedEventArgs e)
        {
            EventHandler<TextReceivedEventArgs> handler = NewEmpaticaDataReceived;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        public class TextReceivedEventArgs : EventArgs
        {
            public string TextReceived { get; set; }
        }
    }
}

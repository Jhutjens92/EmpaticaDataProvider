using EmpaticaDataProvider.ViewModel;
using System;
using System.Text;

namespace EmpaticaDataProvider.EmpaticaManager
{
    class EmpaticaDataManager
    {


        public event EventHandler<TextReceivedEventArgs> NewDataReceived;

        private string _txtReceived = " ";
        public string TxtReceived
        {
            get { return _txtReceived; }
            set
            {
                _txtReceived = value;

            }
        }

        protected virtual void OnNewDataReceived(TextReceivedEventArgs e)
        {
            EventHandler<TextReceivedEventArgs> handler = NewDataReceived;
            if(handler != null)
            {
                handler(this, e);
            }
        }

        public class TextReceivedEventArgs : EventArgs
        {
            public string TextReceived { get; set; }
        }
        #region Methods
       

        public String UpdateText()
        {
            return TxtReceived;
        }

        #endregion
    }
}

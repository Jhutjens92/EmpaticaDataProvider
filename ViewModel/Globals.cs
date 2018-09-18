using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.ViewModel
{
    public static class Globals
    {
        private static bool _isRecordingMqtt = false;
        public static bool IsRecordingMqtt
        {
            get { return _isRecordingMqtt; }
            set
            {
                _isRecordingMqtt = value;
            }
        }
    }
}

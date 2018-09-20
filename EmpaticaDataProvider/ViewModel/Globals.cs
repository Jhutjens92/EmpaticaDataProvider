using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmpaticaDataProvider.ViewModel
{
    public static class Globals
    {
        private static bool _isRecordingData = false;
        public static bool IsRecordingData
        {
            get { return _isRecordingData; }
            set
            {
                _isRecordingData = value;
            }
        }
    }
}

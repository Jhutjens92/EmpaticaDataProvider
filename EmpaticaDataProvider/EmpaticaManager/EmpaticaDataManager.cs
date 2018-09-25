using System;
using EmpaticaDataProvider.ViewModel;

namespace EmpaticaDataProvider.EmpaticaManager
{
    class EmpaticaDataManager
    {
        public EmpaticaDataManager()
        {
            //do stuff
        }

        #region Variables
        public class TagCreatedEventArgs : EventArgs
        {
            public int Tag { get; set; }
        }

        public class PPGSensorChangedEventArgs : EventArgs
        {
            public float BloodVolumePulse { get; set; }
        }

        public class IBISensorChangedEventArgs : EventArgs
        {
            public float InterBeatInterval { get; set; }
            public float HearthRateVariability { get; set; }
        }

        public class AccelerometerChangedEventArgs : EventArgs
        {
            public float AccelerometerX { get; set; }
            public float AccelerometerY { get; set; }
            public float AccelerometerZ { get; set; }
        }

        public class GSRSensorChangedEventArgs : EventArgs
        {
            public float GalvanicSkinResponse { get; set; }
        }

        public class TemperatureSensorChangedEventArgs : EventArgs
        {
            public float SkinTemperature { get; set; }
        }

        #endregion

        #region Event Handlers
        public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;
        public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;
        public event EventHandler<PPGSensorChangedEventArgs> PPGSensorChanged;
        public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;
        public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;
        public event EventHandler<TagCreatedEventArgs> TagCreated;
        #endregion

        #region Event Handler Methods
        protected virtual void OnAccelerometerChanged(AccelerometerChangedEventArgs e)
        {
            AccelerometerChanged?.Invoke(this, e);
        }

        protected virtual void OnPPGSensorChanged(PPGSensorChangedEventArgs e)
        {
            PPGSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnIBISensorChanged(IBISensorChangedEventArgs e)
        {
            IBISensorChanged?.Invoke(this, e);
        }

        protected virtual void OnGSRSensorChanged(GSRSensorChangedEventArgs e)
        {
            GSRSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnTemperatureSensorChanged(TemperatureSensorChangedEventArgs e)
        {
            TemperatureSensorChanged?.Invoke(this, e);
        }

        protected virtual void OnTagCreated(TagCreatedEventArgs e)
        {
            TagCreated?.Invoke(this, e);
        }
        #endregion
    }
}

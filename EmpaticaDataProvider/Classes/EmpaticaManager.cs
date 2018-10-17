using System;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using EmpaticaDataProvider.ViewModel;


namespace EmpaticaDataProvider.Classes
{
    class EmpaticaManager
    {
        #region Events
        public class TagCreatedEventArgs : EventArgs
        {
            
            private int _Tag;
            public int Tag
            {
                get { return _Tag; }
                set { _Tag = value;}
            }
        }

        public class PPGSensorChangedEventArgs : EventArgs
        {
            private float _BloodVolumePulse;
            public float BloodVolumePulse
            {
                get { return _BloodVolumePulse; }
                set { _BloodVolumePulse = value; }
            }
        }

        public class IBISensorChangedEventArgs : EventArgs
        {
            private float _InterBeatInterval;
            public float InterBeatInterval
            {
                get { return _InterBeatInterval; }
                set { _InterBeatInterval = value;}
            }
            private float _HearthRateVariability;
            public float HearthRateVariability
            {
                get { return _HearthRateVariability; }
                set { _HearthRateVariability = value; }
            }
        }

        public class AccelerometerChangedEventArgs : EventArgs
        {
            private float _AccelerometerX;
            public float AccelerometerX
            {
                get { return _AccelerometerX; }
                set { _AccelerometerX = value; }
            }
            private float _AccelerometerY;
            public float AccelerometerY
            {
                get { return _AccelerometerY; }
                set { _AccelerometerY = value; }
            }

            private float _AccelerometerZ;
            public float AccelerometerZ
            {
                get { return _AccelerometerZ; }
                set { _AccelerometerZ = value;}
            }
        }

        public class GSRSensorChangedEventArgs : EventArgs
        {
            private float _GalvanicSkinResponse;
            public float GalvanicSkinResponse
            {
                get { return _GalvanicSkinResponse; }
                set { _GalvanicSkinResponse = value; }
            }
        }

        public class TemperatureSensorChangedEventArgs : EventArgs
        {
            private float _SkinTemperature;
            public float SkinTemperature
            {
                get { return _SkinTemperature; }
                set { _SkinTemperature = value; }
            }
        }
                       
        public event EventHandler<AccelerometerChangedEventArgs> AccelerometerChanged;
        public event EventHandler<IBISensorChangedEventArgs> IBISensorChanged;
        public event EventHandler<PPGSensorChangedEventArgs> PPGSensorChanged;
        public event EventHandler<GSRSensorChangedEventArgs> GSRSensorChanged;
        public event EventHandler<TemperatureSensorChangedEventArgs> TemperatureSensorChanged;
        public event EventHandler<TagCreatedEventArgs> TagCreatedEvent;
   
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
            TagCreatedEvent?.Invoke(this, e);
        }
        #endregion

        #region Methods
        public EmpaticaManager()
        {
            BLEServer.CheckBLEServer();
            StartTCPClients();
        }

        private void StartTCPClients()
        {       
            Thread EDPThread1 = new Thread(new ThreadStart(Datastreams.DataStreamAcc));
            //Thread EDPThread2 = new Thread(new ThreadStart(Datastreams.DataStreamBVP));
            //Thread EDPThread3 = new Thread(new ThreadStart(Datastreams.DataStreamGSR));
            //Thread EDPThread4 = new Thread(new ThreadStart(Datastreams.DataStreamIBI));
            //Thread EDPThread5 = new Thread(new ThreadStart(Datastreams.DataStreamTMP));
            //Thread EDPThread6 = new Thread(new ThreadStart(Datastreams.DataStreamTag));
            EDPThread1.Start();
            //EDPThread2.Start();
            //EDPThread3.Start();
            //EDPThread4.Start();
            //EDPThread5.Start();
            //EDPThread6.Start();

        }

        //}
        #endregion
    }
}

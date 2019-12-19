using Basestation.Common.Data;
using DataAcquisition;
using ShimmerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Basestation.DataAcquisition.Shimmer
{
    public class Shimmer
    {
        ShimmerLogAndStreamSystemSerialPort device;
        private List<string> _signalNames;
        private string _id;
        private string _comPort;
        private string _profile;
        private double _samplingRate;
        private int _enabledSensors;
        private int _range;
        byte[] _defaultECGReg1 = ShimmerBluetooth.SHIMMER3_DEFAULT_ECG_REG1;
        byte[] _defaultECGReg2 = ShimmerBluetooth.SHIMMER3_DEFAULT_ECG_REG2;

        public event EventHandler<NewEcgResponseEventArgs> NewEcgResponse;
        public event EventHandler<NewPpgResponseEventArgs> NewPpgResponse;

        //TODO: Debugging, remove
        //private EcgData lastData = null;
        //private List<double> deltaMisses = new List<double>();
        //private int sent = 0;

        public Shimmer(string id,
            string comPort,
            string profile,
            double samplingRate,
            int enabledSensors,
            int range,
            List<string> signalNames)
        {
            _id = id;
            _comPort = comPort;
            _profile = profile;
            _samplingRate = samplingRate;
            _enabledSensors = enabledSensors;
            _range = range;
            _signalNames = signalNames;

            device = new ShimmerLogAndStreamSystemSerialPort(_id, _comPort);

            device.UICallback += Update;
            device.Connect();

            if (device.GetState() == ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
            {
                device.WriteSensors(_enabledSensors);
                device.WriteEXGConfigurations(_defaultECGReg1, _defaultECGReg2);
                device.WriteSamplingRate(_samplingRate);
                device.WriteGSRRange(_range);

                System.Console.WriteLine("Initialization Done");
                Thread.Sleep(5000);

                device.StartStreaming();
            }
        }

        private void Update(object sender, EventArgs args)
        {
            CustomEventArgs eventArgs = (CustomEventArgs)args;
            int indicator = eventArgs.getIndicator();

            switch (indicator)
            {
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_STATE_CHANGE:
                    System.Diagnostics.Debug.Write(((ShimmerBluetooth)sender).GetDeviceName() + " State = " + ((ShimmerBluetooth)sender).GetStateString() + System.Environment.NewLine);
                    int state = (int)eventArgs.getObject();
                    if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTED)
                    {
                        Console.WriteLine("Connected");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_CONNECTING)
                    {
                        //Console.WriteLine("Connecting");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_NONE)
                    {
                        //Console.WriteLine("Disconnected");
                    }
                    else if (state == (int)ShimmerBluetooth.SHIMMER_STATE_STREAMING)
                    {
                        Console.WriteLine("Streaming");
                    }
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_NOTIFICATION_MESSAGE:
                    break;
                case (int)ShimmerBluetooth.ShimmerIdentifier.MSG_IDENTIFIER_DATA_PACKET:
                    ObjectCluster objectCluster = (ObjectCluster)eventArgs.getObject();

                    switch (_profile)
                    {
                        case "ecg":
                            var la_ra = objectCluster.GetData(Shimmer3Configuration.SignalNames.ECG_LA_RA, "CAL");
                            var ll_ra = objectCluster.GetData(Shimmer3Configuration.SignalNames.ECG_LL_RA, "CAL");
                            var vx_rl = objectCluster.GetData(Shimmer3Configuration.SignalNames.ECG_VX_RL, "CAL");
                            var timeECG = objectCluster.GetData(ShimmerConfiguration.SignalNames.SYSTEM_TIMESTAMP, "CAL");
                            var bat = objectCluster.GetData(Shimmer3Configuration.SignalNames.V_SENSE_BATT, "CAL");
                            var ecgres = new EcgData()
                            {
                                Timestamp = timeECG.Data,
                                SourceTimestamp = timeECG.Data,
                                La_Ra = la_ra.Data,
                                Ll_Ra = ll_ra.Data,
                                Vx_Rl = vx_rl.Data,
                                SampleRate = _samplingRate
                            };

                            //if (lastData != null)
                            //{
                            //    var delta = ecgres.Timestamp - lastData.Timestamp;


                            //    if (delta > 2.2)
                            //        deltaMisses.Add(delta);
                            //    lastData = ecgres;
                            //}

                            //if (lastData == null)
                            //    lastData = ecgres;

                            //sent++;

                            OnNewEcgMessage(ecgres);
                            break;

                        case "ppg":
                            var ppg = objectCluster.GetData(Shimmer3Configuration.SignalNames.INTERNAL_ADC_A13, "CAL");
                            var timePPG = objectCluster.GetData(ShimmerConfiguration.SignalNames.SYSTEM_TIMESTAMP, "CAL");
                            var ppgres = new PpgData()
                            {
                                Timestamp = timePPG.Data,
                                Ppg = ppg.Data,
                                SampleRate = _samplingRate

                            };
                            OnNewPpgMessage(ppgres);
                            break;

                        default:
                            break;
                    }
                    break;
            }
        }

        protected virtual void OnNewEcgMessage(EcgData response)
        {
            var e = new NewEcgResponseEventArgs() { Ecg = response };
            EventHandler<NewEcgResponseEventArgs> handler = NewEcgResponse;
            handler?.Invoke(this, e);
        }

        protected virtual void OnNewPpgMessage(PpgData response)
        {
            var e = new NewPpgResponseEventArgs() { Ppg = response };
            EventHandler<NewPpgResponseEventArgs> handler = NewPpgResponse;
            handler?.Invoke(this, e);
        }
    }


    public class NewEcgResponseEventArgs : EventArgs
    {
        public EcgData Ecg { get; set; }
    }

    public class NewPpgResponseEventArgs : EventArgs
    {
        public PpgData Ppg { get; set; }
    }
}

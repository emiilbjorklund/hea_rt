using System;
using System.Collections.Generic;
using System.Text;
using WiringPi;
using System.Threading.Tasks;
using System.Timers;

namespace Basestation.WarningsAndAlerts.Warnings
{
    public class DeviceStatusTrigger
    {
        private Timer timer;

        private int BUZZER_PIN = 12;
        private int GLED_PIN = 36;
        private int YLED_PIN = 40;
        private int RLED_PIN = 38;

        bool blink = false;
        bool buzz = false;
        private int onOFF;

        private MessageCode status;
        private MessageCode newStatus;

        public DeviceStatusTrigger()
        {
            if (Init.WiringPiSetupPhys() >= 0)
            {
                initPins();
                timer = new Timer();
                timer.Interval = 500;
                timer.Elapsed += ledBuzzerEvent;
                timer.Start();
                status = MessageCode.SysOk; // TODO: May need to be changed to PBIT implementation
                newStatus = MessageCode.SysOk;
                Console.WriteLine("Rpi has been configured");
            }
            else
                throw new Exception("Rpi could not initialize");
        }

        private void ledBuzzerEvent(object sender, ElapsedEventArgs e)
        {
            if (blink)
            {
                onOFF = (int)GPIO.GPIOpinvalue.High;
            }
            else
            {
                onOFF = (int)GPIO.GPIOpinvalue.Low;
            }

            updateNewStatus();
           
            if (status != newStatus)
            {
                GPIO.digitalWrite(GLED_PIN, (int)GPIO.GPIOpinvalue.Low);
                GPIO.digitalWrite(YLED_PIN, (int)GPIO.GPIOpinvalue.Low);
                GPIO.digitalWrite(RLED_PIN, (int)GPIO.GPIOpinvalue.Low);
                GPIO.digitalWrite(BUZZER_PIN, (int)GPIO.GPIOpinvalue.Low);

                status = newStatus;
                buzz = false;
            }
            
            switch (status)
            {
                case MessageCode.SysOk: // SYS OK
                    GPIO.digitalWrite(GLED_PIN, (int)GPIO.GPIOpinvalue.High);
                    timer.Interval = 500;
                    break;
                case MessageCode.SysError: // SYS ERROR
                    GPIO.digitalWrite(YLED_PIN, onOFF);
                    timer.Interval = 500;
                    if (buzz)
                    {
                        GPIO.digitalWrite(BUZZER_PIN, (int)GPIO.GPIOpinvalue.Low);
                    }
                    else
                    {
                        GPIO.digitalWrite(BUZZER_PIN, (int)GPIO.GPIOpinvalue.High);
                        buzz = true;
                    }
                    break;
                case MessageCode.SysFailure: // SYS FAILURE
                    timer.Interval = 500;
                    GPIO.digitalWrite(RLED_PIN, onOFF);
                    GPIO.digitalWrite(BUZZER_PIN, onOFF);
                    break;
                case MessageCode.ArrythmiaWarning: //ARRHYTHMIA
                    timer.Interval = 250;
                    GPIO.digitalWrite(RLED_PIN, onOFF);
                    GPIO.digitalWrite(GLED_PIN, onOFF);
                    GPIO.digitalWrite(YLED_PIN, onOFF);
                    GPIO.digitalWrite(BUZZER_PIN, onOFF);
                    break;
                case MessageCode.SuddenWarning: // SUDDEN CARDIAC
                    timer.Interval = 500;
                    GPIO.digitalWrite(RLED_PIN, onOFF);
                    GPIO.digitalWrite(GLED_PIN, onOFF);
                    GPIO.digitalWrite(YLED_PIN, onOFF);
                    GPIO.digitalWrite(BUZZER_PIN, (int)GPIO.GPIOpinvalue.High);
                    break;
                default:
                    throw new Exception("Message code not recognized");
                    //break;
            }
            blink = !blink;
        }

        private void updateNewStatus()
        {
            if (TriggeredServices.ContainsValue(MessageCode.SysFailure))
                newStatus = MessageCode.SysFailure;
            else if (TriggeredServices.ContainsValue(MessageCode.SuddenWarning))
                newStatus = MessageCode.SuddenWarning;
            else if (TriggeredServices.ContainsValue(MessageCode.ArrythmiaWarning))
                newStatus = MessageCode.ArrythmiaWarning;
            else if (TriggeredServices.ContainsValue(MessageCode.SysError))
                newStatus = MessageCode.SysError;
            else
                newStatus = MessageCode.SysOk;
        }


        private void initPins()
        {
            GPIO.pinMode(GLED_PIN, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(YLED_PIN, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(RLED_PIN, (int)GPIO.GPIOpinmode.Output);
            GPIO.pinMode(BUZZER_PIN, (int)GPIO.GPIOpinmode.Output);
            Console.WriteLine("Rpi gpio configured");
        }

        public Dictionary<string, MessageCode> TriggeredServices { get; set; } = new Dictionary<string, MessageCode>();

    }

}

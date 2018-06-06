using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace LBeaconLaserPointer.Modules.Motor
{
    public abstract class MotorController
    {
        protected GpioPin ControlIO1;
        protected GpioPin ControlIO2;
        protected int OneDegreeRoundCount;
        protected int Angle;
        protected int ChangeCount;


        protected void Stop()
        {
            ControlIO1.Write(GpioPinValue.High);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void PositiveRotation(int Angle)
        {
            this.Angle = Angle;
            ChangeCount = 0;

            ControlIO1.Write(GpioPinValue.High);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.Low);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void ReverseRotation(int Angle)
        {
            this.Angle = Angle;
            ChangeCount = 0;

            ControlIO1.Write(GpioPinValue.Low);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
        }

        protected void PinIn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            ChangeCount++;

            if (ChangeCount > OneDegreeRoundCount * Angle)
                Stop();
        }
    }

    public class VerticalMotor : MotorController
    {
        private GpioPin correctionIO;

        public VerticalMotor(GpioPin IO1, GpioPin IO2, GpioPin SwitchIO, GpioPin CorrectionIO, int RoundCount)
        {
            this.ControlIO1 = IO1;
            this.ControlIO2 = IO2;
            SwitchIO.SetDriveMode(GpioPinDriveMode.Input);
            SwitchIO.ValueChanged += PinIn_ValueChanged;
            correctionIO = CorrectionIO;
            correctionIO.SetDriveMode(GpioPinDriveMode.Input);
            this.OneDegreeRoundCount = RoundCount;
        }

        public void Correction()
        {
            correctionIO.ValueChanged += CorrectionIO_ValueChanged;

            ControlIO1.Write(GpioPinValue.Low);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
        }

        private void CorrectionIO_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            // 遮住是H
            if (sender.Read() == GpioPinValue.High)
            {
                Stop();
                correctionIO.ValueChanged -= CorrectionIO_ValueChanged;
            }
        }
    }

    public class HorizontalMotor : MotorController
    {
        public HorizontalMotor(GpioPin IO1, GpioPin IO2, GpioPin SwitchIO, int RoundCount)
        {
            this.ControlIO1 = IO1;
            this.ControlIO2 = IO2;
            SwitchIO.SetDriveMode(GpioPinDriveMode.Input);
            SwitchIO.ValueChanged += PinIn_ValueChanged;
            this.OneDegreeRoundCount = RoundCount;
        }
    }
}

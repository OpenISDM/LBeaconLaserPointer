using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.Threading;
using LBeaconLaserPointer.Modules.Distance;

namespace LBeaconLaserPointer.Modules.Motor
{
    public abstract class MotorController
    {
        protected GpioPin ControlIO1;
        protected GpioPin ControlIO2;
        protected GpioPin ControlIO3;
        protected GpioPin SIO;
        protected int OneDegreeRoundCount;
        protected int Angle;
        protected int ChangeCount;
        protected AutoResetEvent WaitEvent;


        protected void Stop()
        {
            ControlIO1.Write(GpioPinValue.High);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);

            WaitEvent.Set();
        }

        public void PositiveRotation(int Angle)
        {
            this.Angle = Angle;
            ChangeCount = 0;

            SIO.ValueChanged += PinIn_ValueChanged;

            ControlIO1.Write(GpioPinValue.High);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.Low);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO3.Write(GpioPinValue.High);
            ControlIO3.SetDriveMode(GpioPinDriveMode.Output);

            WaitEvent.WaitOne();
        }

        public void ReverseRotation(int Angle)
        {
            this.Angle = Angle;
            ChangeCount = 0;

            SIO.ValueChanged += PinIn_ValueChanged;

            ControlIO1.Write(GpioPinValue.Low);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO3.Write(GpioPinValue.High);
            ControlIO3.SetDriveMode(GpioPinDriveMode.Output);

            WaitEvent.WaitOne();
        }

        protected void PinIn_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            ChangeCount++;

            if (ChangeCount > OneDegreeRoundCount * Angle)
            {
                Stop();
                SIO.ValueChanged -= PinIn_ValueChanged;
            }
        }
    }

    public class VerticalMotor : MotorController
    {
        private GpioPin correctionIO;

        public VerticalMotor(GpioPin IO1, GpioPin IO2, GpioPin IO3, GpioPin SwitchIO, GpioPin CorrectionIO, int RoundCount)
        {
            this.ControlIO1 = IO1;
            this.ControlIO2 = IO2;
            this.ControlIO3 = IO3;
            SwitchIO.SetDriveMode(GpioPinDriveMode.Input);
            SIO = SwitchIO;
            correctionIO = CorrectionIO;
            correctionIO.SetDriveMode(GpioPinDriveMode.Input);
            this.OneDegreeRoundCount = RoundCount;
            this.WaitEvent = new AutoResetEvent(false);
        }

        public void Correction()
        {
            correctionIO.ValueChanged += CorrectionIO_ValueChanged;

            ControlIO1.Write(GpioPinValue.Low);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);

            ControlIO3.Write(GpioPinValue.High);
            ControlIO3.SetDriveMode(GpioPinDriveMode.Output);
            this.WaitEvent.WaitOne();


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
        protected double correctDistanece = 0;
        public HorizontalMotor(GpioPin IO1, GpioPin IO2, GpioPin IO3, GpioPin SwitchIO, int RoundCount)
        {
            this.ControlIO1 = IO1;
            this.ControlIO2 = IO2;
            this.ControlIO3 = IO3;
            SwitchIO.SetDriveMode(GpioPinDriveMode.Input);
            SIO = SwitchIO;
            this.OneDegreeRoundCount = RoundCount;
            this.WaitEvent = new AutoResetEvent(false);
        }

        public void Correction()
        {
            //ReverseRotation
            SIO.ValueChanged += CorrectionIO_ValueChanged_ReverseRotation;
            ControlIO1.Write(GpioPinValue.Low);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);
            ControlIO2.Write(GpioPinValue.High);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
            ControlIO3.Write(GpioPinValue.High);
            ControlIO3.SetDriveMode(GpioPinDriveMode.Output);
            this.WaitEvent.WaitOne();

            //PositiveRotation
            SIO.ValueChanged += CorrectionIO_ValueChanged_PositiveRotation;
            ControlIO1.Write(GpioPinValue.High);
            ControlIO1.SetDriveMode(GpioPinDriveMode.Output);
            ControlIO2.Write(GpioPinValue.Low);
            ControlIO2.SetDriveMode(GpioPinDriveMode.Output);
            ControlIO3.Write(GpioPinValue.High);
            ControlIO3.SetDriveMode(GpioPinDriveMode.Output);
            this.WaitEvent.WaitOne();

        }
        protected void CorrectionIO_ValueChanged_ReverseRotation(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            const double MAX_VALUE = 40;

            double positioningLiftDistance =
                Utility.ultraSonicSensor.GetDistanceInCentimeters;
            if (positioningLiftDistance > MAX_VALUE)
            {
                Stop();
                SIO.ValueChanged -= CorrectionIO_ValueChanged_ReverseRotation;
            }
        }
        protected void CorrectionIO_ValueChanged_PositiveRotation(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            const double MARGIN_OF_ERROR = 1.05;

            double positioningLeftDistance =
                Utility.ultraSonicSensor.GetDistanceInCentimeters;
            double positioningRightDistance =
                Utility.ultraSonicSensor2.GetDistanceInCentimeters;
            //垂直牆面停止
            if (positioningLeftDistance <= positioningRightDistance * MARGIN_OF_ERROR &&
                positioningLeftDistance * MARGIN_OF_ERROR >= positioningRightDistance)
            {
                Stop();
                SIO.ValueChanged -= CorrectionIO_ValueChanged_PositiveRotation;
            }
        }
    }
}

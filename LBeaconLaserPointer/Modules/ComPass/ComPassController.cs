using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using System.Threading;
using System.Diagnostics;

namespace LBeaconLaserPointer.Modules.Compass
{
    public static class CompassController
    {
        private static I2cDevice CompassDevice;
        private static readonly byte[] FirstDataRegister = new byte[] {0x00 };
        private static bool CompassSwitch;
        private static Task CompassTask;
        private static double CompassAzimuth = double.NaN;

        public static async void Init()
        {
            I2cConnectionSettings settings = new I2cConnectionSettings(0x0D)
            {
                BusSpeed = I2cBusSpeed.StandardMode
            };

            var controller = await I2cController.GetDefaultAsync();

            CompassDevice = controller.GetDevice(settings);
            CompassDevice.Write(new byte[] { 0x0B, 0x01 });
            CompassDevice.Write(new byte[] { 0x09, 0x1D });
        }

        public static void Run()
        {
            CompassSwitch = true;
            CompassTask = new Task(CompassWork);
            CompassTask.Start();
        }

        public static void Close()
        {
            CompassSwitch = false;
        }

        public static double GetAzimuth
        {
            get { return CompassAzimuth; }
        }

        private static void CompassWork()
        {
            while (CompassSwitch)
            {
                try
                {
                    var CompassData = new byte[6];
                    CompassDevice.WriteRead(FirstDataRegister, CompassData);


                    var xReading = 
                        (short)((CompassData[1] << 8) | CompassData[0]);
                    var zReading = 
                        (short)((CompassData[3] << 8) | CompassData[2]);
                    var yReading = 
                        (short)((CompassData[5] << 8) | CompassData[4]);

                    CompassAzimuth = ComputeAzimuth(xReading, yReading);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }

                Task.Delay(100).Wait();
            }

            CompassDevice.Dispose();
        }

        private static double ComputeAzimuth(int X, int Y)
        {
            double Azimuth = Math.Atan2(X, Y) * 180.0 / Math.PI;
            return Azimuth < 0 ? 360 + Azimuth : Azimuth;
        }
    }
}

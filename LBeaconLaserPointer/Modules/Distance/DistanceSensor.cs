using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace LBeaconLaserPointer.Modules.Distance
{
    public class DistanceSensor
    {
        private static SerialDevice SerialPort;
        private static byte[] RxBuffer = new byte[36];
        private static CancellationTokenSource ReadCancellationTokenSource;
        private static DataReader dataReaderObject;
        public static int Distance { get; private set; }
        public static int Strength { get; private set; }

        public async static void Init()
        {
            string DeviceSelector = SerialDevice.GetDeviceSelector("UART0");
            var deviceInformation = await DeviceInformation.FindAllAsync(DeviceSelector);
            SerialPort = await SerialDevice.FromIdAsync(deviceInformation[0].Id);

            SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
            SerialPort.BaudRate = 115200;
            SerialPort.DataBits = 8;

            ReadCancellationTokenSource = new CancellationTokenSource();
            ReadData();
        }

        private static async void ReadData()
        {
            try
            {
                if(SerialPort != null)
                {
                    dataReaderObject = new DataReader(SerialPort.InputStream);

                    while (true)
                        await ReadAsync(ReadCancellationTokenSource.Token);
                }
            }
            catch(TaskCanceledException tce)
            {
                Debug.WriteLine("Error from distance sensor, error message is " + tce.Message);
                CloseDevice();
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Error from distance sensor, error message is " + ex.Message);
            }
            finally
            {
                if(dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
            }
        }

        private static async Task ReadAsync(CancellationToken cancellationToken)
        {
            Task<UInt32> loadAsyncTask;
            uint ReadBufferLength = 1024;
            int count = 0;

            // If task cancellation was requested, comply
            cancellationToken.ThrowIfCancellationRequested();

            // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
            dataReaderObject.InputStreamOptions = InputStreamOptions.None;

            using (var childCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(childCancellationTokenSource.Token);

                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                if (bytesRead > 0)
                {
                    byte[] rxBuffer = new byte[1024];
                    dataReaderObject.ReadBytes(rxBuffer);
                    byte[] Data = new byte[9];

                    for (int i = 0; i < 35; i++)
                        if (rxBuffer[i] == 0x59)
                            if (rxBuffer[i + 1] == 0x59)
                            {
                                int checksum = 0;
                                Array.Copy(rxBuffer, i, Data, 0, 9);
                                for (int j = 0; j < 8; j++)
                                    checksum += Data[j];

                                checksum &= 0xff;
                                if (checksum == Data[8])
                                {
                                    if (count != 0)
                                    {
                                        Distance = Data[2] + (Data[3] << 8);
                                        Strength = Data[4] + (Data[5] << 8);
                                        break;
                                    }
                                    count++;
                                }
                            }
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        private static void CloseDevice()
        {
            if (SerialPort != null)
            {
                SerialPort.Dispose();
            }
            SerialPort = null;
        }
    }

    /// <summary>
    /// 超音波感測器物件
    /// </summary>
    public class UltraSonicSensor
    {
        private GpioPin TriggerPin { get; set; }
        private GpioPin EchoPin { get; set; }
        private Stopwatch timeWatcher;

        public UltraSonicSensor(GpioPin TriggerIO, GpioPin EchoIO)
        {
            GpioController controller = GpioController.GetDefault();
            timeWatcher = new Stopwatch();
            //initialize trigger pin.
            this.TriggerPin = TriggerIO;
            this.TriggerPin.SetDriveMode(GpioPinDriveMode.Output);
            this.TriggerPin.Write(GpioPinValue.Low);
            //initialize echo pin.
            this.EchoPin = EchoIO;
            this.EchoPin.SetDriveMode(GpioPinDriveMode.Input);
        }

        private double GetDistance()
        {
            ManualResetEvent mre = new ManualResetEvent(false);
            mre.WaitOne(100);
            Stopwatch pulseLength = new Stopwatch();
            Stopwatch TotalTime = new Stopwatch();

            TotalTime.Start();

            //Send pulse
            this.TriggerPin.Write(GpioPinValue.High);
            mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
            this.TriggerPin.Write(GpioPinValue.Low);

            //Recieve pusle
            while (this.EchoPin.Read() == GpioPinValue.Low && TotalTime.ElapsedMilliseconds < 5000)
            {
            }
            pulseLength.Start();

            while (this.EchoPin.Read() == GpioPinValue.High && TotalTime.ElapsedMilliseconds < 5000)
            {
            }
            pulseLength.Stop();

            if (TotalTime.ElapsedMilliseconds >= 5000)
                return -1;

            //Calculating distance
            TimeSpan timeBetween = pulseLength.Elapsed;
            //Debug.WriteLine(timeBetween.ToString());

            return timeBetween.TotalSeconds;
        }

        /// <summary>
        /// 取得距離(單位:公分)
        /// </summary>
        public double GetDistanceInCentimeters => GetDistance() * 17000;


        private double PulseIn(GpioPin echoPin, GpioPinValue value)
        {
            var t = Task.Run(() =>
            {
                //Recieve pusle
                while (this.EchoPin.Read() != value)
                {
                }
                timeWatcher.Start();

                while (this.EchoPin.Read() == value)
                {
                }
                timeWatcher.Stop();
                //Calculating distance
                double distance = timeWatcher.Elapsed.TotalSeconds * 17000;
                return distance;
            });
            bool didComplete = t.Wait(5000);
            if (didComplete)
            {
                return t.Result;
            }
            else
            {
                return 0.0;
            }
        }
    }
}

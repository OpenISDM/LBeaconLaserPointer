using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
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
            //SerialPort.Parity = SerialParity.None;
            //SerialPort.StopBits = SerialStopBitCount.One;
            //SerialPort.Handshake = SerialHandshake.None;

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
}

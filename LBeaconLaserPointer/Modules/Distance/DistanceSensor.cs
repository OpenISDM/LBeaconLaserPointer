using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace LBeaconLaserPointer.Modules.Distance
{
    public class DistanceSensor
    {
        public static int GetDistance()
        {
            Task<byte[]> GetDeviceBufferTask = GetDeviceBufferAsync();
            GetDeviceBufferTask.Wait();
            byte[] rxBuffer = GetDeviceBufferTask.Result;
            bool IsGetData = false;
            byte[] Data = new byte[9];

            while (!IsGetData)
            {
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
                                IsGetData = true;
                                break;
                            }
                        }
            }

            return Data[2] + Data[3] * 256;
        }

        private static async Task<byte[]> GetDeviceBufferAsync()
        {
            string DeviceSelector = SerialDevice.GetDeviceSelector("UART0");
            var deviceInformation = await DeviceInformation.FindAllAsync(DeviceSelector);
            byte[] rxBuffer = new byte[36];
            Debug.WriteLine(deviceInformation[0].Id);
            
            try
            {
                    SerialDevice SerialPort = await SerialDevice.FromIdAsync(deviceInformation[0].Id);
                    SerialPort.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    SerialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    SerialPort.BaudRate = 115200;

                    const uint maxReadLength = 36;
                    DataReader dataReader = new DataReader(SerialPort.InputStream);

                    uint bytesToRead = await dataReader.LoadAsync(maxReadLength);
                    dataReader.ReadBytes(rxBuffer);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            

            return rxBuffer;
        }
    }
}

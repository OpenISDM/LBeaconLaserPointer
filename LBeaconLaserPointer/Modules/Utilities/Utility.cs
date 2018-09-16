using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Windows.Devices.Gpio;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using LBeaconLaserPointer.Modules.Motor;
using LBeaconLaserPointer.Modules.Distance;
namespace LBeaconLaserPointer.Modules
{
    public class Utility
    {
        const int MOTER_A1_PIN = 5;
        const int MOTER_A2_PIN = 22;
        const int MOTER_PWMA_PIN = 27;
        const int MOTER_B1_PIN = 13;
        const int MOTER_B2_PIN = 19;
        const int MOTER_PWMB_PIN = 26;
        const int MOTER_STBY_PIN = 6;
        const int PHOTOINTERRUPTER_VERTICAL_CORRECT_PIN = 20;
        const int PHOTOINTERRUPTER_VERTICAL_PIN = 16;
        const int PHOTOINTERRUPTER_HORIZONTAL_PIN = 21;
        const int SONIC_SENSOR_TRIG = 23;
        const int SONIC_SENSOR_TRIG_ECHO = 24;
        const int SONIC_SENSOR2_TRIG = 25;
        const int SONIC_SENSOR2_TRIG_ECHO = 12;
        public static GpioController Gpio = GpioController.GetDefault();
        public static VerticalMotor verticalMotor =
                        new VerticalMotor(Gpio.OpenPin(MOTER_A1_PIN),
                                            Gpio.OpenPin(MOTER_A2_PIN),
                                            Gpio.OpenPin(MOTER_PWMA_PIN),
                                            Gpio.OpenPin(PHOTOINTERRUPTER_VERTICAL_PIN),
                                            Gpio.OpenPin(PHOTOINTERRUPTER_VERTICAL_CORRECT_PIN), 29);

        public static HorizontalMotor horizontalMotor =
                         new HorizontalMotor(Gpio.OpenPin(MOTER_B1_PIN),
                                             Gpio.OpenPin(MOTER_B2_PIN),
                                             Gpio.OpenPin(MOTER_PWMB_PIN),
                                             Gpio.OpenPin(PHOTOINTERRUPTER_HORIZONTAL_PIN), 29);

        public static UltraSonicSensor ultraSonicSensor = 
            new UltraSonicSensor(Gpio.OpenPin(SONIC_SENSOR_TRIG),
                                    Gpio.OpenPin(SONIC_SENSOR_TRIG_ECHO));

        public static UltraSonicSensor ultraSonicSensor2 =
            new UltraSonicSensor(Gpio.OpenPin(SONIC_SENSOR2_TRIG),
                                    Gpio.OpenPin(SONIC_SENSOR2_TRIG_ECHO));

    }
    public class RotateAngle
    {
        /// <summary>
        /// 垂直角度
        /// </summary>
        /// <param name="CenterPoint">Pointer 位置</param>
        /// <param name="TargetPoint">目標位置</param>
        /// <param name="Hight">高度 單位公尺</param>
        /// <returns></returns>
        public static double VerticalRotateAngle(GeoCoordinate CenterPoint, GeoCoordinate TargetPoint, double Hight)
        {
            return Math.Atan(Hight / CenterPoint.GetDistanceTo(TargetPoint)) * 180 / Math.PI;
        }

        /// <summary>
        /// 水平角度(含方向)
        /// </summary>
        /// <param name="CenterPoint">Pointer 位置</param>
        /// <param name="FacePoint">面向位置</param>
        /// <param name="TargetPoint">目標位置</param>
        /// <returns></returns>
        public static double HorizontalRotateAngle(GeoCoordinate CenterPoint, GeoCoordinate FacePoint, GeoCoordinate TargetPoint)
        {
            double DotProductANS = CosineAngle(CenterPoint, FacePoint, TargetPoint);
            double OuterProductANS = OuterProductAngle(CenterPoint, FacePoint, TargetPoint);

            if (OuterProductANS < 0)
                return -DotProductANS * 180 / Math.PI;
            else
                return DotProductANS * 180 / Math.PI;
        }

        /// <summary>
        /// 餘弦定理角度
        /// </summary>
        /// <param name="CenterPoint">Pointer 位置</param>
        /// <param name="FacePoint">面向位置</param>
        /// <param name="TargetPoint">目標位置</param>
        /// <returns></returns>
        public static double CosineAngle(GeoCoordinate CenterPoint, GeoCoordinate FacePoint, GeoCoordinate TargetPoint)
        {
            double CenterToTarget = CenterPoint.GetDistanceTo(TargetPoint);
            double CenterToFace = CenterPoint.GetDistanceTo(FacePoint);
            double FaceToTarget = FacePoint.GetDistanceTo(TargetPoint);

            return Math.Acos(
                (CenterToTarget * CenterToTarget + CenterToFace * CenterToFace - FaceToTarget * FaceToTarget) /
                (2 * CenterToTarget * CenterToFace));
        }

        /// <summary>
        /// 外積算角度
        /// </summary>
        /// <param name="CenterPoint">Pointer 位置</param>
        /// <param name="FacePoint">面向位置</param>
        /// <param name="TargetPoint">目標位置</param>
        /// <returns></returns>
        public static double OuterProductAngle(GeoCoordinate CenterPoint, GeoCoordinate FacePoint, GeoCoordinate TargetPoint)
        {
            double Xa, Xb, Ya, Yb;
            double Angle;

            Xa = FacePoint.Longitude - CenterPoint.Longitude;
            Ya = FacePoint.Latitude - CenterPoint.Latitude;

            Xb = TargetPoint.Longitude - CenterPoint.Longitude;
            Yb = TargetPoint.Latitude - CenterPoint.Latitude;

            double c = Math.Sqrt(Xa * Xa + Ya * Ya) * Math.Sqrt(Xb * Xb + Yb * Yb);
            Angle = Math.Asin((Xa * Yb - Xb * Ya) / c);

            return Angle;
        }
    }
    
}

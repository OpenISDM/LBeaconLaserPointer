using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoCoordinatePortable;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LBeaconLaserPointer.Modules
{
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
            return Math.Acos(Hight / CenterPoint.GetDistanceTo(TargetPoint));
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
    public class Dialog
    {
        /// <summary>
        /// 彈出對話窗
        /// </summary>
        /// <param name="title">標題</param>
        /// <param name="content">內容</param>
        public static async void Display(string title,string content)
        {
            SolidColorBrush Brush = new SolidColorBrush(Windows.UI.Colors.PaleTurquoise);
            ContentDialog subscribeDialog = new ContentDialog
            {
               // Background = Brush,
                Title = title,
                Content = content,
                CloseButtonText = "OK",
            };
            ContentDialogResult result = await subscribeDialog.ShowAsync();
        }
    }
}

using GeoCoordinatePortable;
using LBeaconLaserPointer.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.View
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class PointingPage : Page
    {
        public PointingPage()
        {
            this.InitializeComponent();
            BtnOK.Visibility = Visibility.Collapsed;
            GeoCoordinate CenterPoint = new GeoCoordinate(25.0415216287522, 121.614881758979);
            GeoCoordinate facePoint = new GeoCoordinate(25.0415216287522, 121.614884958979);
            GeoCoordinate targetPoint = new GeoCoordinate(25.0415526287522, 121.614844258979);
            int horizontalRotateAngle = (int)RotateAngle.HorizontalRotateAngle(
                                                CenterPoint,
                                                facePoint,
                                                targetPoint);
            int verticalRotateAngle = (int)RotateAngle.VerticalRotateAngle(
                                    CenterPoint,
                                    targetPoint,
                                    1.2);
            Utility.verticalMotor.Correction();
            Utility.verticalMotor.PositiveRotation(verticalRotateAngle);
            Utility.horizontalMotor.PositiveRotation(horizontalRotateAngle);

            BtnOK.Visibility = Visibility.Visible;
        }

            private void BtnOK_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(xaml.HomePage));
        }
    }
}

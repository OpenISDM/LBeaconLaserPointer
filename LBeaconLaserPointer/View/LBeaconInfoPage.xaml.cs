using GeoCoordinatePortable;
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
using LBeaconLaserPointer.Modules;
using LBeaconLaserPointer.Modules.Distance;
using LLP_API;
using LBeaconLaserPointer.xaml;
// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.View
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class LBeaconInfoPage : Page
    {
        static GeoCoordinate facePoint, CenterPoint, targetPoint;
        static string UUID;

        public LBeaconInfoPage()
        {
            this.InitializeComponent();
        }
       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            UUID = (string)e.Parameter;
            TextLBeaconID.Text = "LBeaconID: " + UUID;
            BeaconInformation BI = PointPage.BeaconInformations.Where(c => c.Id == Guid.Parse(UUID)).First();
            LaserPointerInformation LPI = PointPage.LaserPointerInformations.Where(c => c.Id == BI.LaserPointerInformationId).First();

            latitude.Text = BI.Latitude.ToString();
            longitude.Text = BI.Longitude.ToString();
            ReferencePoint.Text = LPI.Position;

            facePoint = new GeoCoordinate(LPI.FaceLatitude,LPI.FaceLongitude);
            CenterPoint = new GeoCoordinate(LPI.Latitude,LPI.Longitude);
            targetPoint = new GeoCoordinate(BI.Latitude,BI.Longitude);
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void BtnGoStart_Click(object sender, RoutedEventArgs e)
        {
           

        }
    }
}

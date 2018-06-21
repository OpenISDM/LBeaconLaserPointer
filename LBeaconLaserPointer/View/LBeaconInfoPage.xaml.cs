using GeoCoordinatePortable;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            latitude.Text ="經度: "+ BI.Latitude.ToString();
            longitude.Text = "緯度: "+ BI.Longitude.ToString();
            ReferencePoint.Text = "地點: "+ LPI.Position;

            facePoint = new GeoCoordinate(LPI.FaceLatitude, LPI.FaceLongitude);
            CenterPoint = new GeoCoordinate(LPI.Latitude, LPI.Longitude);
            targetPoint = new GeoCoordinate(BI.Latitude, BI.Longitude);

        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void BtnGoStart_Click(object sender, RoutedEventArgs e)
        {
            int horizontalRotateAngle = (int)RotateAngle.HorizontalRotateAngle(
                                                CenterPoint,
                                                facePoint,
                                                targetPoint);
            int verticalRotateAngle = (int)RotateAngle.VerticalRotateAngle(
                                    CenterPoint,
                                    targetPoint,
                                    DistanceSensor.Distance/100);

            latitude.Text = "垂直轉動(度): " + verticalRotateAngle.ToString();
            longitude.Text = "水平轉動(度): " + horizontalRotateAngle.ToString();
            ReferencePoint.Text = "偵測高度(cm): " + DistanceSensor.Distance.ToString();
            Utility.verticalMotor.Correction();
            Utility.verticalMotor.PositiveRotation(verticalRotateAngle);
            if(horizontalRotateAngle < 0)
                Utility.horizontalMotor.PositiveRotation(-horizontalRotateAngle);
            else
                Utility.horizontalMotor.ReverseRotation(horizontalRotateAngle);

            //ShowContentDialog();


            //GeoCoordinate[] PointGeoCoordinate = new GeoCoordinate[3];
            //PointGeoCoordinate[0] = CenterPoint;
            //PointGeoCoordinate[1] = facePoint;
            //PointGeoCoordinate[2] = targetPoint;
            //Frame.Navigate(typeof(PointingPage), PointGeoCoordinate);
        }
        private async void ShowContentDialog()
        {
            var dialog = new ContentDialog()
            {
                Title = "定位完成！!",
                PrimaryButtonText = "確定",
            };
            dialog.PrimaryButtonClick += (_s, _e) => { Frame.Navigate(typeof(HomePage)); };
            await dialog.ShowAsync();
        }
    }
}

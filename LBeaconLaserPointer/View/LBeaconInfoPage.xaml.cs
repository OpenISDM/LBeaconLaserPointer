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
            BeaconInformation BI = 
                PointPage.BeaconInformations.Where(
                    c => c.Id == Guid.Parse(UUID)).First();
            LaserPointerInformation LPI = 
                PointPage.LaserPointerInformations.Where(
                    c => c.Id == BI.LaserPointerInformationId).First();

            latitude.Text = "經度: " + BI.Latitude.ToString();
            longitude.Text = "緯度: " + BI.Longitude.ToString();
            ReferencePoint.Text = "地點: " + LPI.Position;

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


            //偵測數值有誤
            const double HEIGHT_MAX = 1100;
            const double HEIGHT_MIN = 60;
            const double DISTANCE_MAX = 50;
            double measureHeight =
                DistanceSensor.Distance;
            double measureLeftDistance =
                Utility.ultraSonicSensor2.GetDistanceInCentimeters;
            double measureRightDistance =
                Utility.ultraSonicSensor2.GetDistanceInCentimeters;

            if (measureHeight > HEIGHT_MAX || measureHeight < HEIGHT_MIN)
            {
                ShowErrorDialog("高度偵測錯誤！！請再試一次");
                return;
            }
            if (measureLeftDistance > DISTANCE_MAX ||
                measureRightDistance > DISTANCE_MAX)
            {
                ShowErrorDialog("距離偵測錯誤！！請面向牆壁後再試一次");
                return;
            }

            //取得角度
            int horizontalRotateAngle = 
                (int)RotateAngle.HorizontalRotateAngle(CenterPoint,
                                                       facePoint,
                                                       targetPoint);
            int verticalRotateAngle = 
                (int)RotateAngle.VerticalRotateAngle(CenterPoint,
                                                     targetPoint,
                                                     DistanceSensor.Distance / 100);
            //垂直校準
            Utility.verticalMotor.Correction();
            //水平校準
            Utility.horizontalMotor.Correction();
            //垂直轉動
            Utility.verticalMotor.PositiveRotation(verticalRotateAngle);
            //水平轉動
            if (horizontalRotateAngle < 0)
                Utility.horizontalMotor.PositiveRotation(-horizontalRotateAngle);
            else
                Utility.horizontalMotor.ReverseRotation(horizontalRotateAngle);

            ShowSuccessDialog();
        }
        private async void ShowSuccessDialog()
        {
            var dialog = new ContentDialog()
            {
                Title = "定位完成！!",
                PrimaryButtonText = "確定",
            };
            dialog.PrimaryButtonClick += (_s, _e) => { Frame.Navigate(typeof(HomePage)); };
            await dialog.ShowAsync();
        }
        private async void ShowErrorDialog(string title)
        {
            var dialog = new ContentDialog()
            {
                Title = title,
                PrimaryButtonText = "確定",
            };
            await dialog.ShowAsync();
        }
    }
}

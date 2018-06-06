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
        string temp;
        public LBeaconInfoPage()
        {
            this.InitializeComponent();
        }
       
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           
            UUID = (string)e.Parameter;
            TextLBeaconID.Text = "LBeaconID: " + UUID;
            facePoint = new GeoCoordinate(25,121);
            CenterPoint = new GeoCoordinate(24.5,121);
            targetPoint = new GeoCoordinate(24,120);
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

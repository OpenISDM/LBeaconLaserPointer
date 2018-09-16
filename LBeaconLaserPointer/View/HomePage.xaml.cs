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
using Windows.Devices.Gpio;
using LBeaconLaserPointer.View;
using System.Diagnostics;
using LBeaconLaserPointer.Modules.Distance;
using LBeaconLaserPointer.Modules;
using LBeaconLaserPointer.Modules.Motor;
// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.xaml
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
        }

        private void BtnSet_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Setpage));
        }

        private void BtnPoint_Click(object sender, RoutedEventArgs e)
        {

            Frame.Navigate(typeof(PointPage));
        }


    }
}

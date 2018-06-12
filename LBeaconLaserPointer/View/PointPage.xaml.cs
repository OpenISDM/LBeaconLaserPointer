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
using LBeaconLaserPointer.Modules.Utilities;
using LLP_API;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.xaml
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class PointPage : Page
    {
        public static List<BeaconInformation> BeaconInformations = new List<BeaconInformation>();
        public static List<LaserPointerInformation> LaserPointerInformations = new List<LaserPointerInformation>();
        
        public PointPage()
        {
            this.InitializeComponent();
            ListViewLocation.ItemsSource = LocalStorage.AllFileName();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if(Frame.CanGoBack) Frame.GoBack();
        }

        private void BtnGoNext_Click(object sender, RoutedEventArgs e)
        {
            if(ListViewLocation.SelectedItem != null)
            {
                string JsonString = LocalStorage.ReadOnFile(ListViewLocation.SelectedItem.ToString());
                Frame.Navigate(typeof(ScanBarcodePage), ListViewLocation.SelectedItem.ToString());
            }
        }
    }
}

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

namespace LBeaconLaserPointer.xaml
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class PointPage : Page
    {
        List<string> listLocation = new List<string>() {"test0", "test1", "test2", "test3", "test4", "test5", "test6", "test7" };
        
        public PointPage()
        {
            this.InitializeComponent();
            LbLocation.ItemsSource = listLocation;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if(Frame.CanGoBack) Frame.GoBack();
        }

        private void BtnGoNext_Click(object sender, RoutedEventArgs e)
        {
            if(LbLocation.SelectedItem!= null)
            Frame.Navigate(typeof(ScanBarcodePage), LbLocation.SelectedItem.ToString());
        }
    }
}

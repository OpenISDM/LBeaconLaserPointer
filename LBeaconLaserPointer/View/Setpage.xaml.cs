﻿using System;
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

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.xaml
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class Setpage : Page
    {
        public Setpage()
        {
            this.InitializeComponent();
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private async void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            await LocalStorage.CleanAllFileAsync();

            ContentDialog dialog = new ContentDialog
            {
                Title = "訊息",
                Content = "清除成功",
                PrimaryButtonText = "確定"
            };

            await dialog.ShowAsync();
        }

        private void BtnSync_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ScanBarcodePage), "同步");
        }
    }
}

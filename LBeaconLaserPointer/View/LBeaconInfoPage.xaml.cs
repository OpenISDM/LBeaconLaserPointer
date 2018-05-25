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
    public sealed partial class LBeaconInfoPage : Page
    {
        public LBeaconInfoPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string UUID = (string)e.Parameter;
            TextLBeaconID.Text = "LBeaconID: " + UUID;
        }

        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }

        private void BtnGoStart_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(LBeaconInfoPage));
            DisplaySubscribeDialog();
        }
        private async void DisplaySubscribeDialog()
        {
            ContentDialog subscribeDialog = new ContentDialog
            {
                Title = "START",
                Content = "Pointing",
                CloseButtonText = "Stop",
                PrimaryButtonText = "Subscribe",
                SecondaryButtonText = "Try it"
            };
            ContentDialogResult result = await subscribeDialog.ShowAsync();
        }
    }
}

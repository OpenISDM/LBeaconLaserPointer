using App3;
using LBeaconLaserPointer.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LLP_API;
using LBeaconLaserPointer.Modules.Utilities;
using Newtonsoft.Json;

// 空白頁項目範本已記錄在 https://go.microsoft.com/fwlink/?LinkId=234238

namespace LBeaconLaserPointer.xaml
{
    /// <summary>
    /// 可以在本身使用或巡覽至框架內的空白頁面。
    /// </summary>
    public sealed partial class ScanBarcodePage : Page
    {
        private MediaCapture _mediaCapture;
        private int _groupSelectionIndex;
        private string receiveStr;
        private bool firstOpen;
        private object firstOpenLock = new object();

        private List<MediaFrameReader> _sourceReaders = new List<MediaFrameReader>();
        private IReadOnlyDictionary<MediaFrameSourceKind, FrameRenderer> _frameRenderers;

        public ScanBarcodePage()
        {
            this.InitializeComponent();
            _frameRenderers = new Dictionary<MediaFrameSourceKind, FrameRenderer>()
            {
                { MediaFrameSourceKind.Color, new FrameRenderer(imageSource,this) }
            };
        }

        /// <summary>
        /// Disables the "Next Group" button while we switch to the next camera
        /// source and start reading frames.
        /// </summary>
        private async Task PickNextMediaSourceAsync()
        {
            try
            {
                await PickNextMediaSourceWorkerAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Switches to the next camera source and starts reading frames.
        /// </summary>
        private async Task PickNextMediaSourceWorkerAsync()
        {
            await CleanupMediaCaptureAsync();

            var allGroups = await MediaFrameSourceGroup.FindAllAsync();

            if (allGroups.Count == 0)
            {
                return;
            }

            // Pick next group in the array after each time the Next button is clicked.
            _groupSelectionIndex = (_groupSelectionIndex + 1) % allGroups.Count;
            var selectedGroup = allGroups[_groupSelectionIndex];

            try
            {
                // Initialize MediaCapture with selected group.
                // This can raise an exception if the source no longer exists,
                // or if the source could not be initialized.
                await InitializeMediaCaptureAsync(selectedGroup);
            }
            catch (Exception exception)
            {
                await CleanupMediaCaptureAsync();
                Debug.WriteLine(exception.Message);
                return;
            }

            // Set up frame readers, register event handlers and start streaming.
            var startedKinds = new HashSet<MediaFrameSourceKind>();
            foreach (MediaFrameSource source in _mediaCapture.FrameSources.Values)
            {
                MediaFrameSourceKind kind = source.Info.SourceKind;

                // Ignore this source if we already have a source of this kind.
                if (startedKinds.Contains(kind))
                {
                    continue;
                }

                // Look for a format which the FrameRenderer can render.
                string requestedSubtype = null;
                foreach (MediaFrameFormat format in source.SupportedFormats)
                {
                    requestedSubtype = FrameRenderer.GetSubtypeForFrameReader(kind, format);
                    if (requestedSubtype != null)
                    {
                        // Tell the source to use the format we can render.
                        await source.SetFormatAsync(format);
                        break;
                    }
                }
                if (requestedSubtype == null)
                {
                    // No acceptable format was found. Ignore this source.
                    continue;
                }

                MediaFrameReader frameReader = await _mediaCapture.CreateFrameReaderAsync(source, requestedSubtype);

                frameReader.FrameArrived += FrameReader_FrameArrived;
                _sourceReaders.Add(frameReader);

                MediaFrameReaderStartStatus status = await frameReader.StartAsync();
                if (status == MediaFrameReaderStartStatus.Success)
                {
                    startedKinds.Add(kind);
                }
                else
                {
                }
            }

            if (startedKinds.Count == 0)
            {

            }
        }

        /// <summary>
        /// Initializes the MediaCapture object with the given source group.
        /// </summary>
        /// <param name="sourceGroup">SourceGroup with which to initialize.</param>
        private async Task InitializeMediaCaptureAsync(MediaFrameSourceGroup sourceGroup)
        {
            if (_mediaCapture != null)
            {
                return;
            }

            // Initialize mediacapture with the source group.
            _mediaCapture = new MediaCapture();
            var settings = new MediaCaptureInitializationSettings
            {
                SourceGroup = sourceGroup,

                // This media capture can share streaming with other apps.
                SharingMode = MediaCaptureSharingMode.SharedReadOnly,

                // Only stream video and don't initialize audio capture devices.
                StreamingCaptureMode = StreamingCaptureMode.Video,

                // Set to CPU to ensure frames always contain CPU SoftwareBitmap images
                // instead of preferring GPU D3DSurface images.
                MemoryPreference = MediaCaptureMemoryPreference.Cpu
            };

            await _mediaCapture.InitializeAsync(settings);
        }

        /// <summary>
        /// Unregisters FrameArrived event handlers, stops and disposes frame readers
        /// and disposes the MediaCapture object.
        /// </summary>
        private async Task CleanupMediaCaptureAsync()
        {
            if (_mediaCapture != null)
            {
                using (var mediaCapture = _mediaCapture)
                {
                    _mediaCapture = null;

                    foreach (var reader in _sourceReaders)
                    {
                        if (reader != null)
                        {
                            reader.FrameArrived -= FrameReader_FrameArrived;
                            await reader.StopAsync();
                            reader.Dispose();
                        }
                    }
                    _sourceReaders.Clear();
                }
            }
        }

        /// <summary>
        /// Handles a frame arrived event and renders the frame to the screen.
        /// </summary>
        private void FrameReader_FrameArrived(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            // TryAcquireLatestFrame will return the latest frame that has not yet been acquired.
            // This can return null if there is no such frame, or if the reader is not in the
            // "Started" state. The latter can occur if a FrameArrived event was in flight
            // when the reader was stopped.
            using (var frame = sender.TryAcquireLatestFrame())
            {
                if (frame != null)
                {
                    var renderer = _frameRenderers[frame.SourceKind];
                    renderer.ProcessFrame(frame);

                }
            }
        }

        public async void NextStepAsync(string Value)
        {
            bool functionFirstIn = false;
            lock(firstOpenLock)
                if (firstOpen)
                {
                    firstOpen = false;
                    functionFirstIn = true;
                }

            if (functionFirstIn)
                switch (receiveStr)
                {
                    case "同步":
                        bool IsSave = false;
                        string[] Data = Value.Split("}.{");
                        var ServerData = ServerAPI.GetDataFromServer(Data[0]);
                        if (ServerData.Item1)
                        {
                            string BLJson = JsonConvert.SerializeObject(new
                            {
                                BeaconInformation = JsonConvert.SerializeObject(ServerData.Item2),
                                LaserPointerInformation = JsonConvert.SerializeObject(ServerData.Item3)
                            });
                            if (await LocalStorage.WriteToFileAsync(Data[1], BLJson))
                            {
                                IsSave = true;
                            }
                        }

                        if (IsSave)
                        {
                            await CleanupMediaCaptureAsync();
                            ShowContentDialog(true);
                        }
                        else
                        {
                            lock (firstOpenLock)
                                firstOpen = true;
                            ShowContentDialog(false);
                        }

                        break;
                    default:
                        await CleanupMediaCaptureAsync();
                        Frame.Navigate(typeof(LBeaconInfoPage), Value);
                        break;
                }
        }

        private async void ShowContentDialog(bool IsDownload)
        {
            ContentDialog dialog = new ContentDialog();
            if (IsDownload)
            {
                dialog.Title = "同步結果";
                dialog.Content = "同步成功";
                dialog.PrimaryButtonText = "確定";
                dialog.PrimaryButtonClick += (_s, _e) => { Frame.Navigate(typeof(PointPage)); };
            }
            else
            {
                dialog.Title = "同步結果";
                dialog.Content = "同步失敗";
                dialog.PrimaryButtonText = "確定";
            }
            
            await dialog.ShowAsync();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            await CleanupMediaCaptureAsync();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            lock (firstOpenLock)
                firstOpen = true;
            receiveStr = (string)e.Parameter;
            switch (receiveStr)
            {
                case "同步":
                    TextDescription.Text = "";
                    break;
                default:
                    TextDescription.Text = "地點: " + receiveStr;
                    break;
            }
            await PickNextMediaSourceAsync();
        }
       
        private void BtnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack) Frame.GoBack();
        }


    }
}

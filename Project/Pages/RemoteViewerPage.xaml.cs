using Kazyx.RemoteApi.AvContent;
using Kazyx.WPPMM.DataModel;
using Kazyx.WPPMM.PlaybackMode;
using Kazyx.WPPMM.Resources;
using Kazyx.WPPMM.Utils;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Reactive;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Devices.Geolocation;

namespace Kazyx.WPPMM.Pages
{
    public partial class RemoteViewerPage : PhoneApplicationPage
    {
        public RemoteViewerPage()
        {
            InitializeComponent();
        }

        private bool IsRemoteInitialized = false;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.New)
            {
                NavigationService.GoBack();
                return;
            }

            IsRemoteInitialized = false;
            UnsupportedMessage.Visibility = Visibility.Collapsed;

            GridSource = new DateGroupCollection();
            RemoteImageGrid.ItemsSource = GridSource;

            groups = new ThumbnailGroup();
            LocalImageGrid.DataContext = groups;

            SetVisibility(false);

            LoadLocalContents();

            // AddDummyContentsAsync();

            CameraManager.CameraManager.GetInstance().Status.PropertyChanged += status_PropertyChanged;
            PictureSyncManager.Instance.Failed += OnDLError;
            PictureSyncManager.Instance.Fetched += OnFetched;
            PictureSyncManager.Instance.Downloader.QueueStatusUpdated += OnFetchingImages;
        }

        ThumbnailGroup groups = null;

        private void LoadLocalContents()
        {
            var lib = new MediaLibrary();
            PictureAlbum CameraRoll = null;
            foreach (var album in lib.RootPictureAlbum.Albums)
            {
                if (album.Name == "Camera Roll")
                {
                    CameraRoll = album;
                    break;
                }
            }
            if (CameraRoll == null)
            {
                DebugUtil.Log("No camera roll. Going back");
                NavigationService.GoBack();
                return;
            }
            LoadThumbnails(CameraRoll);
        }

        private async void LoadThumbnails(PictureAlbum album)
        {
            ChangeProgressText("Loading camera roll images...");
            var group = new List<ThumbnailData>();
            await Task.Run(() =>
            {
                foreach (var pic in album.Pictures)
                {
                    group.Add(new ThumbnailData(pic));
                }
            });
            group.Reverse();

            Dispatcher.BeginInvoke(() =>
            {
                if (group != null)
                {
                    groups.Group = new ObservableCollection<ThumbnailData>(group);
                }
            });
            HideProgress();
        }

        private string CurrentUuid { set; get; }

        private async void AddDummyContentsAsync()
        {
            if (CurrentUuid == null)
            {
                CurrentUuid = DummyContentsGenerator.RandomUuid();
            }

            for (int i = 0; i < 1; i++)
            {
                foreach (var date in DummyContentsGenerator.RandomDateList(50))
                {
                    var list = new List<RemoteThumbnail>();
                    foreach (var content in DummyContentsGenerator.RandomContentList(50))
                    {
                        list.Add(new RemoteThumbnail(CurrentUuid, date, content));
                    }
                    await Task.Delay(500);
                    Dispatcher.BeginInvoke(() =>
                    {
                        if (GridSource != null)
                        {
                            GridSource.AddRange(list);
                        }
                    });
                }
                await Task.Delay(500);
            }
        }

        private CancellationTokenSource Canceller;

        private DateGroupCollection GridSource;

        private bool CheckRemoteCapability()
        {
            var cm = CameraManager.CameraManager.GetInstance();
            if (cm.CurrentDeviceInfo == null)
            {
                DebugUtil.Log("Device not found");
                return false;
            }
            CurrentUuid = cm.CurrentDeviceInfo.UDN;

            if (cm.AvContentApi == null)
            {
                DebugUtil.Log("AvContent service is not supported");
                return false;
            }

            return true;
        }

        private async void InitializeRemote()
        {
            IsRemoteInitialized = true;
            var cm = CameraManager.CameraManager.GetInstance();
            try
            {
                ChangeProgressText("Chaging camera state...");
                await PlaybackModeUtility.MoveToContentTransferModeAsync(cm.CameraApi, cm.Status);

                ChangeProgressText("Checking storage capability...");
                if (!await PlaybackModeUtility.IsStorageSupportedAsync(cm.AvContentApi))
                {
                    DebugUtil.Log("storage scheme is not supported");
                    UpdateTitleHeader("storage scheme is not supported");
                    //GoBack();
                    return;
                }

                ChangeProgressText("Checking storage uri...");
                var storages = await PlaybackModeUtility.GetStoragesUriAsync(cm.AvContentApi);
                if (storages.Count == 0)
                {
                    DebugUtil.Log("No storages");
                    UpdateTitleHeader("No storages");
                    //GoBack();
                    return;
                }

                Canceller = new CancellationTokenSource();

                ChangeProgressText("Fetching date list...");
                await PlaybackModeUtility.GetDateListAsEventsAsync(cm.AvContentApi, storages[0], OnDateListUpdated, Canceller);
            }
            catch (Exception e)
            {
                UpdateTitleHeader(e.GetType().ToString());
                DebugUtil.Log(e.StackTrace);
                //GoBack();
            }
        }

        private void UpdateTitleHeader(string text)
        {
            DebugUtil.Log(text);
            Dispatcher.BeginInvoke(() =>
            {
                TitleHeader.Text = text;
            });
        }

        private async void OnDateListUpdated(DateListEventArgs args)
        {
            foreach (var date in args.DateList)
            {
                try
                {
                    ChangeProgressText("Fetching contents...");
                    await PlaybackModeUtility.GetContentsOfDayAsEventsAsync(
                        CameraManager.CameraManager.GetInstance().AvContentApi, date, true, OnContentListUpdated, Canceller);
                    HideProgress();
                }
                catch (Exception e)
                {
                    DebugUtil.Log(e.StackTrace);
                    //GoBack();
                }
            }
        }

        private void OnContentListUpdated(ContentListEventArgs args)
        {
            var list = new List<RemoteThumbnail>();
            foreach (var content in args.ContentList)
            {
                list.Add(new RemoteThumbnail(CameraManager.CameraManager.GetInstance().CurrentDeviceInfo.UDN, args.DateInfo, content));
            }

            Dispatcher.BeginInvoke(() =>
            {
                if (GridSource != null)
                {
                    GridSource.AddRange(list);
                }
            });
        }

        private void HideProgress()
        {
            Dispatcher.BeginInvoke(() =>
            {
                progress.IsVisible = false;
            });
        }

        private void ChangeProgressText(string text)
        {
            DebugUtil.Log(text);
            Dispatcher.BeginInvoke(() =>
            {
                progress.Text = text;
                progress.IsVisible = true;
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PictureSyncManager.Instance.Failed -= OnDLError;
            PictureSyncManager.Instance.Fetched -= OnFetched;
            PictureSyncManager.Instance.Downloader.QueueStatusUpdated -= OnFetchingImages;
            CameraManager.CameraManager.GetInstance().Status.PropertyChanged -= status_PropertyChanged;
            if (Canceller != null)
            {
                Canceller.Cancel();
            }
            if (GridSource != null)
            {
                GridSource.Clear();
                GridSource = null;
            }

            if (groups != null && groups.Group != null)
            {
                groups.Group.Clear();
                groups = null;
            }

            HideProgress();

            if (CurrentUuid != null)
            {
                ThumbnailCacheLoader.INSTANCE.DeleteCacheDirectory(CurrentUuid);
            }
            CurrentUuid = null;

            if (e.NavigationMode != NavigationMode.Back)
            {
                CameraManager.CameraManager.GetInstance().Refresh();
            }

            base.OnNavigatedFrom(e);
        }

        private void OnFetched(Picture pic, Geoposition pos)
        {
            DebugUtil.Log("ViewerPage: OnFetched");
            Dispatcher.BeginInvoke(() =>
            {
                var groups = LocalImageGrid.DataContext as ThumbnailGroup;
                if (groups == null)
                {
                    return;
                }
                groups.Group.Insert(0, new ThumbnailData(pic));
            });
        }

        private void OnDLError(ImageDLError error)
        {
            DebugUtil.Log("ViewerPage: OnDLError");
        }

        void status_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            DebugUtil.Log("ViewerPage: status_PropertyChanged");
            switch (e.PropertyName)
            {
                case "PictureUrls":
                    OnPictureUrlsUpdated(CameraManager.CameraManager.GetInstance().Status.PictureUrls);
                    break;
                default:
                    break;
            }
        }

        private void OnPictureUrlsUpdated(string[] urls)
        {
            DebugUtil.Log("ViewerPage: OnPictureUrlsUpdated");
            if (urls == null)
            {
                return;
            }
            if (!ApplicationSettings.GetInstance().IsPostviewTransferEnabled)
            {
                DebugUtil.Log("Postview transfer is disabled");
                return;
            }
            foreach (var url in urls)
            {
                try
                {
                    var uri = new Uri(url);
                    PictureSyncManager.Instance.Enque(uri);
                }
                catch (UriFormatException)
                {
                    DebugUtil.Log("UriFormatException: " + url);
                }
            }
        }

        private void OnFetchingImages(int count)
        {
            if (count != 0)
            {
                ChangeProgressText(AppResources.ProgressMessageFetching);
            }
            else
            {
                HideProgress();
            }
        }

        private async void ThumbnailImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (IsViewingDetail)
            {
                return;
            }
            ChangeProgressText("Opening images...");
            var img = sender as Image;
            var thumb = img.DataContext as ThumbnailData;
            await Task.Run(() =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    using (var strm = thumb.picture.GetImage())
                    {
                        using (var replica = new MemoryStream())
                        {
                            strm.CopyTo(replica); // Copy to the new stream to avoid stream crash issue.
                            if (replica.Length <= 0)
                            {
                                return;
                            }
                            replica.Seek(0, SeekOrigin.Begin);

                            _bitmap = new BitmapImage();
                            _bitmap.SetSource(replica);
                            InitBitmapBeforeOpen();
                            DetailImage.Source = _bitmap;
                            SetVisibility(true);
                        }
                    }
                });
            });
        }

        private void ImageGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var selector = sender as LongListSelector;
            selector.ItemsSource = GridSource;
        }

        private void ImageGrid_Unloaded(object sender, RoutedEventArgs e)
        {
            var selector = sender as LongListSelector;
            selector.ItemsSource = null;
        }

        private async void ImageGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var content = (sender as LongListSelector).SelectedItem as RemoteThumbnail;
            if (content != null)
            {
                UpdateTitleHeader(content.Source.Name + " - " + content.Source.ContentType);
                switch (content.Source.ContentType)
                {
                    case ContentKind.StillImage:
                        ChangeProgressText("Fetching detail image...");
                        try
                        {
                            using (var strm = await Downloader.GetResponseStreamAsync(new Uri(content.Source.LargeUrl)))
                            {
                                var replica = new MemoryStream();

                                strm.CopyTo(replica); // Copy to the new stream to avoid stream crash issue.
                                if (replica.Length <= 0)
                                {
                                    return;
                                }
                                replica.Seek(0, SeekOrigin.Begin);

                                Dispatcher.BeginInvoke(() =>
                                {
                                    try
                                    {
                                        _bitmap = new BitmapImage();
                                        _bitmap.SetSource(replica);
                                        InitBitmapBeforeOpen();
                                        DetailImage.Source = _bitmap;
                                        SetVisibility(true);
                                    }
                                    finally
                                    {
                                        if (replica != null)
                                        {
                                            replica.Dispose();
                                        }
                                    }
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            DebugUtil.Log(ex.StackTrace);
                            UpdateTitleHeader("Failed to fetch detail image");
                            HideProgress();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void SetVisibility(bool visible)
        {
            if (visible)
            {
                progress.IsVisible = false;
                IsViewingDetail = true;
                viewport.Visibility = Visibility.Visible;
                DetailImage.Visibility = Visibility.Visible;
                TouchBlocker.Visibility = Visibility.Visible;
                RemoteImageGrid.IsEnabled = false;
                LocalImageGrid.IsEnabled = false;
            }
            else
            {
                progress.IsVisible = false;
                IsViewingDetail = false;
                DetailImage.Visibility = Visibility.Collapsed;
                TouchBlocker.Visibility = Visibility.Collapsed;
                viewport.Visibility = Visibility.Collapsed;
                RemoteImageGrid.IsEnabled = true;
                LocalImageGrid.IsEnabled = true;
            }
        }

        void InitBitmapBeforeOpen()
        {
            DebugUtil.Log("Before open");
            _scale = 0;
            CoerceScale(true);
            _scale = _coercedScale;

            ResizeImage(true);
        }
        const double MaxScale = 1.0;

        double _scale = 1.0;
        double _minScale;
        double _coercedScale;
        double _originalScale;

        Size _viewportSize;
        bool _pinching;
        Point _screenMidpoint;
        Point _relativeMidpoint;

        BitmapImage _bitmap;

        private void viewport_ManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
        {
            _pinching = false;
            _originalScale = _scale;
        }

        private void viewport_ManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!_pinching)
                {
                    _pinching = true;
                    var center = e.PinchManipulation.Original.Center;
                    _relativeMidpoint = new Point(center.X / DetailImage.ActualWidth, center.Y / DetailImage.ActualHeight);

                    var xform = DetailImage.TransformToVisual(viewport);
                    _screenMidpoint = xform.Transform(center);
                }

                _scale = _originalScale * e.PinchManipulation.CumulativeScale;

                CoerceScale(false);
                ResizeImage(false);
            }
            else if (_pinching)
            {
                _pinching = false;
                _originalScale = _scale = _coercedScale;
            }
        }

        private void viewport_ManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
        {
            _pinching = false;
            _scale = _coercedScale;
        }

        private void viewport_ViewportChanged(object sender, System.Windows.Controls.Primitives.ViewportChangedEventArgs e)
        {
            var newSize = new Size(viewport.Viewport.Width, viewport.Viewport.Height);
            if (newSize != _viewportSize)
            {
                _viewportSize = newSize;
                CoerceScale(true);
                ResizeImage(false);
            }
        }

        void ResizeImage(bool center)
        {
            if (_coercedScale != 0 && _bitmap != null)
            {
                double newWidth = canvas.Width = Math.Round(_bitmap.PixelWidth * _coercedScale);
                double newHeight = canvas.Height = Math.Round(_bitmap.PixelHeight * _coercedScale);

                xform.ScaleX = xform.ScaleY = _coercedScale;

                viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

                if (center)
                {
                    viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((newWidth - viewport.ActualWidth) / 2),
                            Math.Round((newHeight - viewport.ActualHeight) / 2)
                            ));
                }
                else
                {
                    var newImgMid = new Point(newWidth * _relativeMidpoint.X, newHeight * _relativeMidpoint.Y);
                    var origin = new Point(newImgMid.X - _screenMidpoint.X, newImgMid.Y - _screenMidpoint.Y);
                    viewport.SetViewportOrigin(origin);
                }
            }
        }

        void CoerceScale(bool recompute)
        {
            if (recompute && _bitmap != null && viewport != null)
            {
                // Calculate the minimum scale to fit the viewport 
                var minX = viewport.ActualWidth / _bitmap.PixelWidth;
                var minY = viewport.ActualHeight / _bitmap.PixelHeight;

                _minScale = Math.Min(minX, minY);
                DebugUtil.Log("Minimum scale: " + _minScale);
            }

            _coercedScale = Math.Min(MaxScale, Math.Max(_scale, _minScale));
            //DebugUtil.Log("Coerced scale: " + _coercedScale);
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, CancelEventArgs e)
        {
            if (IsViewingDetail)
            {
                ReleaseDetail();
                e.Cancel = true;
            }
        }

        private void ReleaseDetail()
        {
            if (DetailImage.Source != null)
            {
                DetailImage.Source = null;
            }
            _bitmap = null;
            SetVisibility(false);
        }

        private bool IsViewingDetail = false;

        private async void ImageGrid_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            if (e.ItemKind == LongListSelectorItemKind.Item)
            {
                var content = e.Container.Content as RemoteThumbnail;
                if (content != null)
                {
                    await content.FetchThumbnailAsync();
                }
            }
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pivot = sender as Pivot;
            if (pivot.SelectedIndex != 1)
            {
                return;
            }

            if (CheckRemoteCapability())
            {
                if (!IsRemoteInitialized)
                {
                    InitializeRemote();
                }
            }
            else
            {
                ShowToast("Storage access is not supported\nby your camera device");
                UnsupportedMessage.Visibility = Visibility.Visible;
            }
        }

        private void ShowToast(string message)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ToastMessage.Text = message;
                ToastApparance.Begin();
            });
        }

        private void ToastApparance_Completed(object sender, EventArgs e)
        {
            Scheduler.Dispatcher.Schedule(() =>
            {
                ToastDisApparance.Begin();
            }, TimeSpan.FromSeconds(3));
        }
    }
}

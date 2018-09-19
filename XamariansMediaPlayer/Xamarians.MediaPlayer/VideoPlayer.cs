using System;
using System.Linq;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms;

namespace Xamarians.MediaPlayers
{
    public class VideoPlayer : Grid
    {

        BufferSlider timelineBar;
        BufferSlider bufferingSlider;
        NativePlayer nativePlayer;
        int currentVideoIndex = 0;
        bool isPortrait = false;
        bool _seekChanged = false;
        readonly VideoModel model;


        #region Properties

        public static readonly BindableProperty AutoPlayProperty =
            BindableProperty.Create("AutoPlay", typeof(bool), typeof(BufferSlider), false);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create("Source", typeof(Array), typeof(VideoPlayer), propertyChanged: OnSourcePropertyChanged);

        public static readonly BindableProperty MuteProperty =
            BindableProperty.Create("Mute", typeof(bool), typeof(VideoPlayer), false);

        //bool _isAutoPlay;
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set
            {
                SetValue(AutoPlayProperty, value);
                // _isAutoPlay = (bool)GetValue(AutoPlayProperty);
            }
        }

        //bool _isMute;
        public bool Mute
        {
            get { return (bool)GetValue(MuteProperty); }
            set
            {
                SetValue(MuteProperty, value);
                //_isMute = (bool)GetValue(MuteProperty);
            }
        }

        public string[] Source
        {
            get { return (string[])GetValue(SourceProperty); }
            set
            {
                SetValue(SourceProperty, value);
            }
        }

        public bool IsPlaying
        {
            get { return model.IsPlaying; }
        }

        public int SourceCount
        {
            get { return Source?.Count() ?? 0; }
        }

        public bool IsPrepared
        {
            get { return model.IsPrepared; }
        }

        private bool CanPlay
        {
            get
            {
                return nativePlayer != null;
            }
        }

        #endregion

        #region Events

        public event EventHandler<double> SeekBarPositionChanged;
        public event EventHandler Prepared;
        public event EventHandler Completed;
        public event EventHandler<string> ErrorOcurred;
        public event EventHandler<bool> PlayStatusChanged;
        public event EventHandler DisplaySeekbar;


        #endregion

        public VideoPlayer()
        {
            model = new VideoModel();
            RowSpacing = 0;
            RowDefinitions = new RowDefinitionCollection
            {
                // NATIVE VIDEO PLAYER
              //  new RowDefinition(),
                // SEEK BAR
                new RowDefinition(){ Height = GridLength.Star },
            };
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition() { Width = GridLength.Star }
            };
            InitializeView();
            //InitializeSeekbarAutoHide();
            BindingContext = model;
        }

        private static void OnSourcePropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var bindable = obj as VideoPlayer;
            if (bindable == null)
                return;
            //var sourceList = ((IEnumerable)newValue).Cast<string>().ToList();
            bindable.UpdatePlayerSource(0);

            //if (!string.IsNullOrWhiteSpace(sourceList[0]))
            //{
            //    if (sourceList.Count() == 1)
            //        bindable.model.HasPrev = bindable.model.HasNext = false;

            //    if (sourceList.Count() > 1)
            //    {
            //        bindable.model.HasPrev = false; bindable.model.HasNext = true;
            //    }

            //    bindable.nativePlayer.Source = sourceList[0];
            //    bindable.model.FileName = sourceList[0].Split(new char[] { '\\', '/' }).LastOrDefault();
            //}
            //else
            //{
            //    bindable.nativePlayer?.Stop();
            //    bindable.model.FileName = string.Empty;
            //}
        }

        #region Private Methods

        private void InitializeView()
        {
            // SET ACTIVITY INDICATOR
            var loader = new ActivityIndicator() { Color = Color.White, WidthRequest = 50, HeightRequest = 50, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            loader.SetBinding(ActivityIndicator.IsRunningProperty, "IsBusy");
            // INITIALIZE NATIVE PLAYER
            nativePlayer = new NativePlayer();
            nativePlayer.SeekBarPositionChanged += OnVideoPlayerSeekBarPositionChange;
            nativePlayer.Prepared += OnVideoPlayerPrepared;
            nativePlayer.Completed += OnVideoPlayerCompleted;
            nativePlayer.Error += OnVideoPlayerError;
            nativePlayer.BuffringPositionChanged += OnVideoPlayerBuffringPositionChanged;
            nativePlayer.DisplaySeekbar += OnDisplaySeekbar;
            nativePlayer.OnTapped(() => { model.DisplaySeekbar = !model.DisplaySeekbar; });
            Children.Add(nativePlayer, 0, 0);
            Children.Add(loader, 0, 0);

            // INTIALIZE LABEL TO DISPLAY FILENAME
            var lblFileName = new Label()
            {
                Margin = 10,
                LineBreakMode = LineBreakMode.MiddleTruncation,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Start,
                HorizontalTextAlignment = TextAlignment.Center
            };
            lblFileName.SetBinding(IsVisibleProperty, "DisplaySeekbar");
            lblFileName.SetBinding(Label.TextProperty, "FileName");
            Children.Add(lblFileName, 0, 0);

            // INTIALIZE TIMELINE CONTAINER
            InitlializeTimelineControls();
        }

        private void InitlializeTimelineControls()
        {
            timelineBar = new BufferSlider() { Minimum = 0, Value = 0, Maximum = 100, };
            timelineBar.ValueChanged += OnVideoTimelineSliderValueChanged;
            timelineBar.StopedDraging += (sender, e) => {
                var tt = e;
            };
            bufferingSlider = new BufferSlider()
            {
                Minimum = 0,
                Value = 0,
                Maximum = 100,
                IsSliderThumbVisible = false,
            };

            var btnPlay = new Button
            {
                WidthRequest = 40,
                BackgroundColor = Color.Transparent,
                FontSize = 20,
                TextColor = Color.White,
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAPlay
            };
            btnPlay.SetBinding(Button.TextProperty, "IsPlaying", converter: new BoolToTextConverter(FontAwesome.FAPause, FontAwesome.FAPlay));
            btnPlay.Clicked += delegate { Play(); };

            var btnSound = new Button
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.White,
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAVolumeUp,
                BackgroundColor = Color.Transparent,
            };
            btnSound.Clicked += OnButtonSoundClicked;
            btnSound.SetBinding(Button.TextProperty, "IsMute", converter: new BoolToTextConverter(FontAwesome.FAVolumeOff, FontAwesome.FAVolumeUp));

            var lblCurrentPosition = new Label()
            {
                Text = "00:00",
                Margin = new Thickness(10, 0, 0, 0),
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.StartAndExpand
            };
            string format = model.Duration > TimeSpan.FromHours(1) ? "{0:hh\\:mm\\:ss}" : "{0:mm\\:ss}";

            lblCurrentPosition.SetBinding(Label.TextProperty, "CurrentPosition", stringFormat: format);

            var lblDuration = new Label()
            {
                Text = "00:00",
                Margin = new Thickness(0, 0, 10, 0),
                TextColor = Color.White,
                HorizontalOptions = LayoutOptions.EndAndExpand
            };

            lblDuration.SetBinding(Label.TextProperty, "Duration", stringFormat: format);

            var btnFullScreen = new Button()
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.White,
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAExpand,
                BackgroundColor = Color.Transparent,
            };
            btnFullScreen.SetBinding(Button.TextProperty, "IsFullScreen", converter: new BoolToTextConverter(FontAwesome.FAExpand, FontAwesome.FACompress));
            btnFullScreen.Clicked += OnButtonFullScreenClicked;

            var btnStepForward = new Button()
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.White,
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAStepForward,
                BackgroundColor = Color.Transparent,
            };
            btnStepForward.SetBinding(Button.IsEnabledProperty, "HasNext");
            btnStepForward.Clicked += OnButtonStepForwardClicked;

            var btnStepBackward = new Button()
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.White,
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAStepBackward,
                BackgroundColor = Color.Transparent,
            };
            btnStepBackward.SetBinding(Button.IsEnabledProperty, "HasPrev");
            btnStepBackward.Clicked += OnButtonStepBackwardClicked;


            var gridTimeline = new Grid
            {
                RowSpacing = 0,
                BackgroundColor = Color.FromHex("75000000"),
                VerticalOptions = LayoutOptions.EndAndExpand,
                RowDefinitions = new RowDefinitionCollection()
                {
                    // Seekbar
                    new RowDefinition { Height=GridLength.Auto},
                    // Duration
                    new RowDefinition { Height=GridLength.Auto},
                    // Media Controls
                    new RowDefinition { Height=GridLength.Auto},
                }
            };
            gridTimeline.SetBinding(IsVisibleProperty, "DisplaySeekbar");

            // Seekbar
            gridTimeline.Children.Add(bufferingSlider, 0, 5, 0, 1);
            gridTimeline.Children.Add(timelineBar, 0, 5, 0, 1);
            // Duration
            gridTimeline.Children.Add(lblCurrentPosition, 0, 2, 1, 2);
            gridTimeline.Children.Add(lblDuration, 3, 5, 1, 2);
            // Media Controls
            gridTimeline.Children.Add(btnSound, 0, 1, 2, 3);
            gridTimeline.Children.Add(btnStepBackward, 1, 2, 2, 3);
            //  gridTimeline.Children.Add(loader, 2, 3, 2, 3);
            gridTimeline.Children.Add(btnPlay, 2, 3, 2, 3);
            gridTimeline.Children.Add(btnStepForward, 3, 4, 2, 3);
            gridTimeline.Children.Add(btnFullScreen, 4, 5, 2, 3);

            Children.Add(gridTimeline, 0, 0);
        }

        private void OnDisplaySeekbar(object sender, EventArgs e)
        {
            model.DisplaySeekbar = !model.DisplaySeekbar;
        }
        private void SetBusy(bool value)
        {
            model.IsBusy = value;
        }

        private void OnVideoPlayerError(object sender, PlayerErrorEventArgs e)
        {
            SetBusy(false);
            model.IsPlaying = false;
            nativePlayer.Stop();
            ErrorOcurred?.Invoke(this, e.Message);
        }

        private void OnVideoPlayerBuffringPositionChanged(object sender, int values)
        {
            bufferingSlider.Value = values;
        }

        private void UpdatePlayerSource(int index)
        {
            if (SourceCount > index)
            {
                currentVideoIndex = index;
                model.FileName = Source[index].Split(new char[] { '\\', '/' }).LastOrDefault();
                SetBusy(true);
                nativePlayer.Source = Source[index];
            }
            else
            {
                currentVideoIndex = 0;
                model.FileName = string.Empty;
                nativePlayer.Source = null;
            }
            model.CurrentPosition = TimeSpan.Zero;
            model.Duration = TimeSpan.Zero;
            timelineBar.Value = 0;
            model.HasNext = SourceCount > 1 && index < SourceCount - 1;
            model.HasPrev = SourceCount > 0 && index > 0;
            _isDurationSet = false;
        }

        private void OnVideoTimelineSliderValueChanged(object sender, ValueChangedEventArgs e)
        {
            var oldValue = (e.OldValue / 100) * ((nativePlayer.Duration) / 1000);
            var newValue = (e.NewValue / 100) * ((nativePlayer.Duration) / 1000);
            nativePlayer.Seek((int)((e.NewValue / 100) * ((nativePlayer.Duration) / 1000)));
            if (!model.IsPlaying)
            {
                return;
            }
            if ((oldValue - newValue) > 1.5 || (oldValue - newValue) < -1.5)
            {
                nativePlayer.Seek((int)((e.NewValue / 100) * ((nativePlayer.Duration) / 1000)));
                _seekChanged = true;
                if (bufferingSlider.Value <= timelineBar.Value)
                {
                    SetBusy(true);
                }
                else
                {
                    SetBusy(false);
                }
            }
        }


        private void OnVideoPlayerCompleted(object sender, EventArgs e)
        {
            Completed?.Invoke(this, EventArgs.Empty);
            model.IsPlaying = false;
        }

        private void OnVideoPlayerPrepared(object sender, EventArgs e)
        {
            model.IsPrepared = true;
            SetBusy(false);
            model.Duration = TimeSpan.FromMilliseconds(nativePlayer.Duration);
            model.CurrentPosition = TimeSpan.Zero;
            model.DisplaySeekbar = true;
            Prepared?.Invoke(this, EventArgs.Empty);
            if (Mute)
            {
                model.IsMute = true;
                SetVolume();
            }
            else
            {
                model.IsMute = false;
                SetVolume();
            }
            if (AutoPlay)
            {
                model.IsPlaying = true;
                model.IsPrepared = true;
                nativePlayer.Play();
            }
        }

        bool _isDurationSet = false;
        private void OnVideoPlayerSeekBarPositionChange(object sender, double position)
        {
            if (!model.IsPrepared && Math.Abs(position) <= 0)
                return;

            var values = position / nativePlayer.Duration;
            if (!_isDurationSet)
            {
                OnVideoPlayerDurationChange(this, nativePlayer.Duration);
                _isDurationSet = true;
            }
            Device.BeginInvokeOnMainThread(() =>
            {
                timelineBar.Value = values * 100;
                model.CurrentPosition = TimeSpan.FromMilliseconds(position);
                if (_seekChanged)
                {
                    SetBusy(false);
                    _seekChanged = false;
                }
            });
        }

        void OnVideoPlayerDurationChange(object sender, double duration)
        {
            model.Duration = TimeSpan.FromMilliseconds(duration);
        }

        #region Video Controls
        private void OnButtonSoundClicked(object sender, EventArgs e)
        {
            model.IsMute = !model.IsMute;
            SetVolume();
        }

        private void SetVolume()
        {
            nativePlayer.SetVolume(model.IsMute);
        }

        private void OnButtonStepBackwardClicked(object sender, EventArgs e)
        {
            if (currentVideoIndex <= 0)
            {
                model.IsPrepared = false;
                return;
            }
            UpdatePlayerSource(currentVideoIndex - 1);
            nativePlayer.IsCompleted = false;
        }


        private void OnButtonStepForwardClicked(object sender, EventArgs e)
        {
            nativePlayer.IsCompleted = false;
            if (currentVideoIndex >= SourceCount - 1)
            {
                model.IsPrepared = false;
                return;
            }
            UpdatePlayerSource(currentVideoIndex + 1);
        }

        private void OnButtonFullScreenClicked(object sender, EventArgs e)
        {

            isPortrait = !isPortrait;
            nativePlayer.SetScreen(isPortrait);
            model.IsFullScreen = isPortrait;
        }

        #endregion

        #endregion



        public void Play()
        {
            if (nativePlayer.IsCompleted)
            {
                model.IsPlaying = true;
                UpdatePlayerSource(currentVideoIndex);
                nativePlayer.IsCompleted = false;
                nativePlayer.Play();
            }
            else
            {
                if (nativePlayer.IsPlaying)
                {
                    Pause();
                }
                else
                {


                    if (nativePlayer.Play())
                    {
                        model.IsPlaying = true;
                        SetBusy(false);
                    }
                }
            }
        }

        public void Pause()
        {
            nativePlayer.Pause();
            model.IsPlaying = false;
        }
    }

    public enum PlayStatus
    {
        Error = 0, Prepared = 1, Completed = 2,
    }
}

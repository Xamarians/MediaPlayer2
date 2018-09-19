using Android.App;
using Android.Content.PM;
using Android.Media;
using Android.Views;
using Android.Widget;
using System;
using System.ComponentModel;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(NativePlayer), typeof(Xamarians.MediaPlayers.Droid.NativePlayerRenderer))]
namespace Xamarians.MediaPlayers.Droid
{
    internal class NativePlayerRenderer : ViewRenderer<NativePlayer, RelativeLayout>, IVideoPlayer
    {
        static VideoView _videoView;
        bool _prepared;
        System.Timers.Timer timer;
        System.Timers.Timer bufferTimer;
        static Activity _activity;
        static MediaPlayer _mediaPlayer;

        public static void Init(Activity activity)
        {
            _activity = activity;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<NativePlayer> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                return;
            var context = Xamarin.Forms.Forms.Context;

            // Set Native Control
            var relativeLayout = new RelativeLayout(context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
            };
            relativeLayout.SetPadding(0, 0, 0, 0);
            relativeLayout.SetBackgroundColor(Android.Graphics.Color.Black);
            SetNativeControl(relativeLayout);
            Element.SetNativeContext(this);
            // Create Video View
            InitVideoView();

            // Start the MediaController
            SetSource();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (NativePlayer.SourceProperty.PropertyName.Equals(e.PropertyName))
            {
                SetSource();
            }
        }


        #region Private Methods

        private void InitVideoView()
        {
            _videoView = new VideoView(Context);
            _videoView.Holder.SetKeepScreenOn(true);
            _videoView.Prepared += VideoView_Prepared;
            _videoView.Error += _videoView_Error;
            _videoView.Completion += _videoView_Completion;
            _videoView.Info += _videoView_Info;
            _mediaPlayer = new MediaPlayer();
            // _videoView.SetOnPreparedListener(new OnPreparedListener());

            var lv = new RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
            lv.AddRule(LayoutRules.CenterInParent);
            _videoView.LayoutParameters = lv;
            Control.AddView(_videoView);

            timer = new System.Timers.Timer(1000) { AutoReset = true };
            timer.Elapsed += (s, e) =>
            {
                Element.OnSeekBarPositionChanged(_videoView.CurrentPosition);
            };

            bufferTimer = new System.Timers.Timer(1000) { AutoReset = true };
            bufferTimer.Elapsed += (s, e) =>
            {
                Element.OnBuffringChanged(_videoView.BufferPercentage);

                if (_videoView.BufferPercentage == 100)
                    bufferTimer.Stop();
            };

            _activity.RequestedOrientation = ScreenOrientation.Sensor;
        }

        private void InitMediaController()
        {
            MediaController mediacontroller = new MediaController(Context, false);
            mediacontroller.SetAnchorView(_videoView);
            //_videoView.SetMediaController(mediacontroller);
        }

        private void SetSource()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Element.Source))
                    return;
                _prepared = false;
                if (Element.Source.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                    || Element.Source.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                    _videoView.SetVideoURI(Android.Net.Uri.Parse(Element.Source));
                else
                {
                    _videoView.SetVideoPath(Element.Source);
                }
                _videoView.RequestFocus();
                //  _videoView.SetOnPreparedListener(new VideoLoop());
                //_videoView.SetOnPreparedListener(new OnPreparedListener());
            }
            catch (Java.Lang.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                Element.OnError(e.Message);
            }
        }

        #endregion

        #region INativePlayer

        public int Duration
        {
            get
            {
                return _prepared ? _videoView.Duration : 0;
            }
        }

        public int CurrentPosition
        {
            get
            {
                return _prepared ? _videoView.CurrentPosition : 0;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _prepared ? _videoView.IsPlaying : false;
            }
        }


        public bool Play()
        {
            if (!_prepared)
                return false;
            _videoView.Start();
            timer.Start();
            bufferTimer.Start();
            return true;
        }

        public bool Pause()
        {
            if (!_prepared) return false;
            if (_videoView.CanPause())
            {
                _videoView.Pause();
                timer.Stop();
                return true;
            }
            return false;
        }

        public bool Stop()
        {
            if (!_prepared) return false;
            _videoView.StopPlayback();
            return true;
        }

        public bool Seek(int seconds)
        {
            if (!_prepared) return false;
            _videoView.SeekTo(seconds * 1000);
            return true;
        }

        public void SetVolume(bool isMute)
        {
            if (isMute)
                _mediaPlayer.SetVolume(0, 0);
            else
                _mediaPlayer.SetVolume(1, 1);
        }

        public bool SetScreen(bool isPortrait)
        {
            if (_activity == null)
                return false;

            if (isPortrait)
            {
                _activity.Window.AddFlags(WindowManagerFlags.Fullscreen);
                _activity.RequestedOrientation = ScreenOrientation.SensorLandscape;
            }
            else
            {
                _activity.RequestedOrientation = ScreenOrientation.FullSensor;
            }
            return true;
        }

        #endregion

        #region Events

        private void VideoView_Prepared(object sender, System.EventArgs e)
        {
            _mediaPlayer = sender as MediaPlayer;
            _prepared = true;
            if (Element.AutoPlay)
                Play();
            Element?.OnPrepare();
        }

        private void _videoView_Info(object sender, Android.Media.MediaPlayer.InfoEventArgs e)
        {
            //Element?.OnControlsVisibility(e.What == MediaInfo.BufferingStart ? true : false);
            // progressBar.Visibility = e.What == MediaInfo.BufferingStart ? Android.Views.ViewStates.Visible : Android.Views.ViewStates.Invisible;
        }

        private void _videoView_Completion(object sender, System.EventArgs e)
        {
            Element?.OnCompletion();
        }

        private void _videoView_Error(object sender, Android.Media.MediaPlayer.ErrorEventArgs e)
        {
            Element?.OnError(e.What.ToString());
        }

        #endregion
    }
}

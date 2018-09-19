using AVFoundation;
using AVKit;
using Foundation;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UIKit;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(NativePlayer), typeof(Xamarians.MediaPlayers.iOS.VideoPlayerRenderer))]
namespace Xamarians.MediaPlayers.iOS
{
    public class VideoPlayerRenderer : ViewRenderer<NativePlayer, UIView>, IVideoPlayer
    {
        AVPlayer _player;
        AVPlayerViewController _playerController;
        bool _prepared;
        System.Timers.Timer _timer;
        bool _isPlaying = false;
        //NativePlayer videoPlayerView;

        public static new void Init()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<NativePlayer> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                return;
            // Set Native Control
            _playerController = new AVPlayerViewController();
            _playerController.ShowsPlaybackControls = false;
            var tapGesture = new UITapGestureRecognizer(OnTap) { NumberOfTapsRequired = 1 };
            _playerController.View.UserInteractionEnabled = false;
            _playerController.ContentOverlayView.AddGestureRecognizer(tapGesture);

            SetNativeControl(_playerController.View);
            Element.SetNativeContext(this);
            SetSource(Element.Source);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (NativePlayer.SourceProperty.PropertyName.Equals(e.PropertyName))
            {
                SetSource(Element.Source);
            }
        }

        void OnTap()
        {
            Element.OnDisplaySeekbar();
        }

        #region Private Methods

        //private void SetBuffering()
        //{
        //    var loadedTimeRanges = this._player.CurrentItem.LoadedTimeRanges;
        //    var timeRange = loadedTimeRanges[0].CMTimeRangeValue;
        //    var startSeconds = timeRange.Start;
        //    var durationSeconds = timeRange.Duration;
        //    var result = startSeconds + durationSeconds;
        //}

        public void SetSource(string source)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                    return;
                _prepared = false;
                if (_player != null)
                {
                    _player.Dispose();
                    _player = null;
                }

                _timer = new System.Timers.Timer(1000) { AutoReset = true };
                _timer.Elapsed += (s, e) =>
                {
                    if (_isPlaying)
                        Element.OnSeekBarPositionChanged(CurrentPosition);

                    Element.OnBuffringChanged(CurrentPosition);
                };

                AVPlayerItem playerItem = null;
                if (source.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase)
                    || source.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                    playerItem = new AVPlayerItem((NSUrl.FromString(source)));
                else
                    playerItem = new AVPlayerItem(NSUrl.FromFilename(source));
                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidVideoFinishPlaying, playerItem);
                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidVideoErrorOcurred, playerItem);
                NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidVideoErrorOcurred, playerItem);

                _player = new AVPlayer(playerItem);
                _player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
                _playerController.Player = _player;
                _prepared = true;

                if (Element.AutoPlay)
                    _player.Play();

                if (_player.Error != null)
                {
                    Element.OnError(_playerController?.Player?.Error?.LocalizedDescription);
                }
                OnPrepared();
            }
            catch (Exception e)
            {
                Element.OnError(e.Message);
            }
        }

        #endregion

        #region INativePlayer

        public int Duration
        {
            get
            {
                if (!_prepared || _player == null)
                    return 0;
                else
                    return (int)_player.CurrentItem.Duration.Seconds * 1000;
            }
        }

        public int CurrentPosition
        {
            get
            {
                if (!_prepared || _player == null)
                    return 0;
                else
                    return (int)_player?.CurrentItem.CurrentTime.Seconds * 1000;
            }
        }


        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

        public bool Play()
        {
            if (!_prepared || _player == null)
                return false;
            _isPlaying = true;
            _timer.Start();
            _player.Play();
            return true;
        }

        private async void OnPrepared()
        {
            await Task.Delay(1000);
            Element.OnPrepare();
        }

        public bool Pause()
        {
            if (!_prepared || _player == null)
                return false;
            _isPlaying = false;
            _timer.Stop();
            _player.Pause();
            return true;
        }

        public bool Stop()
        {
            if (!_prepared || _player == null)
                return false;
            _isPlaying = false;
            _player.Pause();
            _timer.Stop();
            return true;
        }

        public bool Seek(int seconds)
        {
            if (!_prepared) return false;
            _player.Seek(CoreMedia.CMTime.FromSeconds(seconds, 2000));
            return true;
        }

        public void SetVolume(bool isMute)
        {
            if (_player != null)
                _player.Muted = isMute;
        }

        public bool SetScreen(bool isPortrait)
        {
            UIInterfaceOrientation uiOrientation;
            if (isPortrait)
                uiOrientation = UIInterfaceOrientation.LandscapeLeft;
            else
                uiOrientation = UIInterfaceOrientation.Portrait;

            NSNumber n = new NSNumber((int)uiOrientation);
            NSString key = new NSString("orientation");
            UIDevice.CurrentDevice.SetValueForKey(n, key);
            return true;
        }


        #endregion

        #region Events

        private void DidVideoFinishPlaying(NSNotification obj)
        {

            Element.OnCompletion();
            if (_player != null)
            {
                Element.IsCompleted = true;
            }
            _timer.Stop();
            _isPlaying = false;
            _player.Pause();
        }

        private void DidVideoErrorOcurred(NSNotification obj)
        {
            Element.OnError(_player.Error?.Description ?? "Unable to play video.");
        }


        //private void DidVideoPrepared(NSNotification obj)
        //{
        //    try
        //    {
        //        if (_player.Status == AVPlayerStatus.ReadyToPlay)
        //        {
        //            Element.OnPrepare();
        //            //if (Element.AutoPlay)
        //            //{
        //            //    Play();
        //            //}
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}


        #endregion
    }
}

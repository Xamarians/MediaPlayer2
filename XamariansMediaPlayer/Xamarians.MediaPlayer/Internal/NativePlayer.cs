using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

[assembly: Preserve(AllMembers = true)]
namespace Xamarians.MediaPlayers.Internal
{
    public class NativePlayer : View
    {
        IVideoPlayer _videoPlayer;

        #region Properties

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(string), typeof(VideoPlayer), null);
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(VideoPlayer), false);
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set { SetValue(AutoPlayProperty, value); }
        }

        public int Duration
        {
            get { return _videoPlayer == null ? 0 : _videoPlayer.Duration; }
        }

        public int CurrentPosition
        {
            get { return _videoPlayer == null ? 0 : _videoPlayer.CurrentPosition; }
        }

        public bool IsPlaying
        {
            get { return _videoPlayer == null ? false : _videoPlayer.IsPlaying; }
        }
        public bool IsCompleted
        {
            get;
            set;
        }

        #endregion

        #region Events

        public event EventHandler<double> SeekBarPositionChanged;
        public event EventHandler Completed;
        public event EventHandler<PlayerErrorEventArgs> Error;
        public event EventHandler Prepared;
        public event EventHandler<int> BuffringPositionChanged;
        public event EventHandler DisplaySeekbar;
        #endregion

        public NativePlayer()
        {

        }

        #region Methods

        public bool Pause()
        {
            return _videoPlayer?.Pause() ?? false;
        }

        public bool Play()
        {
            return _videoPlayer?.Play() ?? false;
        }

        public bool Stop()
        {
            return _videoPlayer?.Stop() ?? false;
        }

        public bool Seek(int seconds)
        {
            return _videoPlayer?.Seek(seconds) ?? false;
        }

        public void SetVolume(bool isMute)
        {
            _videoPlayer?.SetVolume(isMute);
        }

        public void SetScreen(bool isPortrait)
        {
            _videoPlayer?.SetScreen(isPortrait);
        }

        #endregion

        #region Event Handlers

        public void SetNativeContext(IVideoPlayer player)
        {
            _videoPlayer = player;
        }

        public void OnBuffringChanged(int bufferPercentage)
        {
            BuffringPositionChanged?.Invoke(this, bufferPercentage);
        }

        public void OnSeekBarPositionChanged(int position)
        {
            SeekBarPositionChanged?.Invoke(this, position);
        }

        public void OnError(string error)
        {
            Error?.Invoke(this, new PlayerErrorEventArgs(error));
        }

        public void OnCompletion()
        {
            Completed?.Invoke(this, EventArgs.Empty);
        }

        public void OnPrepare()
        {
            Prepared?.Invoke(this, EventArgs.Empty);
        }
        public void OnDisplaySeekbar()
        {
            DisplaySeekbar?.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}

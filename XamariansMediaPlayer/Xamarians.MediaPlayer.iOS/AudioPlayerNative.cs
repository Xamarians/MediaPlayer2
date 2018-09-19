using AVFoundation;
using Foundation;
using System;
using System.Threading.Tasks;

[assembly: Xamarin.Forms.DependencyAttribute(typeof(Xamarians.MediaPlayers.iOS.AudioPlayerNative))]
namespace Xamarians.MediaPlayers.iOS
{
    public class AudioPlayerNative : IAudioPlayer
    {
        System.Timers.Timer _timer;
        bool _prepared;
        AVPlayer audioPlayer;
        bool _isPlaying = false;
        #region Interface Implemenation

        public event EventHandler<double> SeekPositionChanged;
        public event EventHandler<double> SetDuration;
        public event EventHandler<int> BuffringPositionChanged;
        public event EventHandler Completed;

        public int CurrentPosition
        {
            get
            {
                return _prepared ? (int)audioPlayer.CurrentItem.CurrentTime.Seconds * 1000 : 0;
			}
        }

        public int Duration
        {
            get
            {
				return _prepared ? (int)audioPlayer.CurrentItem.Duration.Seconds * 1000 : 0;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _isPlaying;
            }
        }

		public AudioPlayerNative()
		{
			 
		}

        public void LoadAsync(string source)
        {
            AVAsset asset;

            if (string.IsNullOrWhiteSpace(source))
				return;
			_prepared = false;
			if (audioPlayer != null)
			{
				audioPlayer.Dispose();
				audioPlayer = null;
			}
            _prepared = false;
            _timer = new System.Timers.Timer(1000) { AutoReset = true };

            if (source.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) 
                || source.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase))
                asset = AVAsset.FromUrl(NSUrl.FromString(source));
            else
                asset = AVAsset.FromUrl(NSUrl.FromFilename(source));

            var playerItem = new AVPlayerItem(asset);
			audioPlayer = new AVPlayer(playerItem);

			NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, DidVideoFinishPlaying, playerItem);
			//NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.ItemFailedToPlayToEndTimeNotification, DidVideoErrorOcurred, playerItem);
			//NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.NewErrorLogEntryNotification, DidVideoErrorOcurred, playerItem);

            _timer.Elapsed += (s, e) =>
            {
                if (SeekPositionChanged != null)
                {
                    SeekPositionChanged.Invoke(this, CurrentPosition);
                }
            };
            _prepared = true;
            SetTotalDuration();
		}

        public bool Play()
		{
            if (!_prepared)
                return false;
			_isPlaying = true;
			audioPlayer.Play();
			_timer.Start();
			OnPrepared();
            return true;
		}

		private async void OnPrepared()
		{
			await Task.Delay(2000);
			//Element.OnPrepare();
		}

		public void Pause()
		{
			if (!_prepared)
				return;
			audioPlayer.Pause();
			_isPlaying = false;
			_timer.Stop();
		}

		public void Stop()
		{
			if (!_prepared)
				return;
			_isPlaying = false;
			audioPlayer.Pause();
			_timer.Stop();
		}

		public void Seek(int seconds)
		{
			if (!_prepared) return;
			audioPlayer.Seek(CoreMedia.CMTime.FromSeconds(seconds, 10000));
		}

		public void SetVolume(bool isMute)
		{
			if (audioPlayer != null)
				audioPlayer.Muted = isMute;
		}

		public void Dispose()
        {
            Dispose(true);
        }

        private async void SetTotalDuration()
        {
            await Task.Delay(2000);
            SetDuration?.Invoke(this, Duration);
        }

        #endregion

        private bool disposedValue = false; // To detect redundant calls
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    audioPlayer.Dispose();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

		private void DidVideoFinishPlaying(NSNotification obj)
		{
            Completed?.Invoke(this,null);
			_timer.Stop();
		}

       
    }
}
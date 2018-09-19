using System;
using Xamarians.MediaPlayers;

[assembly: Xamarin.Forms.Dependency(typeof(AudioPlayerNative))]
namespace Xamarians.MediaPlayers
{
    public class AudioPlayerNative : IAudioPlayer
    {
        System.Timers.Timer timer;
        bool prepare;
        Android.Media.MediaPlayer player;

        #region Interface Implemenation

        public event EventHandler<double> SeekPositionChanged;
        public event EventHandler Completed;
        public event EventHandler<int> BuffringPositionChanged;
        public event EventHandler<double> SetDuration;

        public int CurrentPosition
        {
            get
            {
                return player.CurrentPosition;
            }
        }

        public int Duration
        {
            get
            {
                return player.Duration;
            }
        }

        public bool IsPlaying
        {
            get
            {
                if (player != null)
                    return player.IsPlaying;
                else
                    return false;
            }
        }

        public AudioPlayerNative()
        {
            player = new Android.Media.MediaPlayer();
            player.Prepared += (s, e) => { prepare = true; };
            player.SetScreenOnWhilePlaying(true);

            timer = new System.Timers.Timer(1000) { AutoReset = true };
            timer.Elapsed += (s, e) =>
            {
                if (SeekPositionChanged != null)
                {
                    SeekPositionChanged.Invoke(this, player.CurrentPosition);
                }
                var timeDiff = (player.Duration - player.CurrentPosition);
                if (timeDiff < 1000 &&  player.Duration!= 1886146040)
                {
                    if (Completed != null)
                    {
                        timer.Stop();
                        Completed.Invoke(this, null);
                    }
                }
            };
        }

        public void LoadAsync(string filePath)
        {
            if (filePath == null)
                return;
            player.Reset();
            player.SetDataSource(filePath);
            player.PrepareAsync();
            player.BufferingUpdate += Player_BufferingUpdate;
        }

        private void Player_BufferingUpdate(object sender, Android.Media.MediaPlayer.BufferingUpdateEventArgs e)
        {
            if (BuffringPositionChanged != null)
            {
                BuffringPositionChanged.Invoke(this, e.Percent);
            }
        }


        public bool Play()
        {
            if (!prepare)
                return false;
            try
            {
                player.Start();
                timer.Start();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Pause()
        {
            if (!prepare)
                return;
            player.Pause();
            timer.Stop();

        }

        public void Stop()
        {
            if (!prepare)
                return;
            try
            {
                if (!player.IsPlaying)
                    return;
                player.Stop();
                timer.Stop();
            }
            catch
            {

            }
        }

        public void Dispose()
        {
            Dispose(true);
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
                    player.Release();
                    player.Dispose();
                }
                // TODO: set large fields to null.
                disposedValue = true;
            }
        }

        public void Seek(int seconds)
        {
            player.SeekTo(seconds * 1000);
        }
    }
}
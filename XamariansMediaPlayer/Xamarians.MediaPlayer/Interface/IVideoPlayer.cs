using System;
using System.Threading.Tasks;

namespace Xamarians.MediaPlayers
{
    public interface IVideoPlayer
    {
        int CurrentPosition { get; }
        int Duration { get; }
        bool Play();
        bool Pause();
        bool Stop();
        bool IsPlaying { get; }
        bool Seek(int seconds);
        void SetVolume(bool isMute);
        bool SetScreen(bool isPortrait);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarians.MediaPlayers
{
   public interface IAudioPlayer : IDisposable
    {
		event EventHandler<double> SetDuration;
		event EventHandler<double> SeekPositionChanged;
        event EventHandler<int> BuffringPositionChanged;
        
        event EventHandler Completed;

        int CurrentPosition { get; }
        int Duration { get; }
        bool IsPlaying { get; }

        void LoadAsync(string filePath);
        void Seek(int Seconds);

        bool Play();
        void Pause();
        void Stop();
    }
	
}

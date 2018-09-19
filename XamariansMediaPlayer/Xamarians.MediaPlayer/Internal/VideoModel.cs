using System;
using System.ComponentModel;

namespace Xamarians.MediaPlayers
{
    internal class VideoModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetProperty<T>(ref T field, T value, string propertyName)
        {
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool _displaySeekbar;
        public bool DisplaySeekbar
        {
            get { return _displaySeekbar; }
            set
            {
                SetProperty(ref _displaySeekbar, value, nameof(DisplaySeekbar));
            }
        }

        bool _hasNext;
        public bool HasNext
        {
            get { return _hasNext; }
            set { _hasNext = value; OnPropertyChanged("HasNext"); }
        }

        bool _hasPrev;
        public bool HasPrev
        {
            get { return _hasPrev; }
            set { _hasPrev = value; OnPropertyChanged("HasPrev"); }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; OnPropertyChanged("IsBusy"); }
        }

        string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; OnPropertyChanged("FileName"); }
        }

        TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { _duration = value; OnPropertyChanged(nameof(Duration)); }
        }

        TimeSpan _currentPosition;
        public TimeSpan CurrentPosition
        {
            get { return _currentPosition; }
            set { _currentPosition = value; OnPropertyChanged(nameof(CurrentPosition)); }
        }

        bool _isPlaying;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set { _isPlaying = value; OnPropertyChanged("IsPlaying"); }
        }

        bool _isPrepared;
        public bool IsPrepared
        {
            get { return _isPrepared; }
            set { _isPrepared = value; OnPropertyChanged("IsPrepared"); }
        }

        bool _isMute;
        public bool IsMute
        {
            get { return _isMute; }
            set { _isMute = value; OnPropertyChanged("IsMute"); }
        }

        bool _isFullScreen;
        public bool IsFullScreen
        {
            get { return _isFullScreen; }
            set { _isFullScreen = value; OnPropertyChanged("IsFullScreen"); }
        }

    }
}

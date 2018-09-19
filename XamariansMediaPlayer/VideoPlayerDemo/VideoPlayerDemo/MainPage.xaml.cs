using System;
using Xamarin.Forms;

namespace VideoPlayerDemo
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void VideoPlayer_Completed(object sender, EventArgs e)
        {
        }

        private void VideoPlayer_Prepared(object sender, EventArgs e)
        {
            videoPlayer.Mute = true;
        }

        private async void VideoPlayer_ErrorAsync(object sender, EventArgs e)
        {
            var error = e as Xamarians.MediaPlayers.PlayerErrorEventArgs;
            await Application.Current.MainPage.DisplayAlert("", error.Message, "Ok");
        }

        private void VideoPlayer_SeekBarPositionChanged(object sender, EventArgs e)
        {
            var error = e as ValueChangedEventArgs;

            var t = error.OldValue;
        }
    }
}

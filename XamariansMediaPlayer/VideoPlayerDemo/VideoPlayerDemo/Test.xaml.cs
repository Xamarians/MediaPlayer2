using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VideoPlayerDemo
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Test : ContentPage
    {
        public Test()
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

        //private void VideoPlayer_SeekBarPositionChanged(object sender, EventArgs e)
        //{
        //    var error = e as ValueChangedEventArgs;

        //   // var t = error.OldValue;
        //}

        private void videoPlayer_SeekBarPositionChanged_1(object sender, double e)
        {

        }
    }
}
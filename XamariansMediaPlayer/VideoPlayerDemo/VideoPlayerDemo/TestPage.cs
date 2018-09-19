
using Xamarin.Forms;

namespace VideoPlayerDemo
{
    public class TestPage : ContentPage
    {
        public TestPage()
        {
            //var audioPlayer = new Xamarians.MediaPlayers.AudioPlayer()
            //{
            //    IsLoopPlay = true,
            //};
            //audioPlayer.Source = new string[]
            //{
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3",
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3",
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3",
            //    "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-6.mp3",
            //};
            //audioPlayer.ContentSource = new StackLayout
            //{
            //    VerticalOptions = LayoutOptions.CenterAndExpand,
            //    HorizontalOptions = LayoutOptions.FillAndExpand,
            //    Children =
            //    {
            //        new Image { HeightRequest=200,WidthRequest=200, Source="xamarians_logo.png" }
            //    }
            //};


            var videoPlayer = new Xamarians.MediaPlayers.VideoPlayer() { };
          //  videoPlayer.Source = "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4";

            //           new string[]
            //          {
            //              // "file:///private/var/mobile/Containers/Data/Application/5B426D9D-5091-469E-BE76-B9F748AA704D/tmp/trim.CEE273AE-F4F7-4F99-81D0-29AF3DE254DE.MOV",              
            //              // "file:///private/var/mobile/Containers/Data/Application/5B426D9D-5091-469E-BE76-B9F748AA704D/tmp/trim.29649882-54D6-4458-94A7-32487E80A7F0.MOV",
            //               "SampleVideo.mp4",
            //              "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4",
            //               "http://techslides.com/demos/sample-videos/small.mp4",
            //              "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4",
            //   "SampleVideo.mp4",
            //"http://techslides.com/demos/sample-videos/small.mp4",
            //              "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4",
            //               "http://techslides.com/demos/sample-videos/small.mp4",
            //              "http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4",
            //               //"https://www.youtube.com/watch?v=W5MZevEH5Ns",
            //          };

            Content = videoPlayer;
        }
    }
}

using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms;

namespace Xamarians.MediaPlayers
{
    public class AudioPlayer : Grid
    {

        static IAudioPlayer audioPlayer;
        static Grid audioControl, audioView;
        Slider audioSlider;
        BufferSlider bufferingSlider;
        static Button btnPlay, btnStepForward, btnStepBackward;
        Label lblDuration, lblCurrentPosition;
        static Label lblFileName;
        static int audioSourceNum = 0;
        ActivityIndicator loader;
        static bool _canPlay = false;
        static bool _isAutoPlay = false;
        bool _seekChanged = false, isPrepared=false;

        public static readonly BindableProperty ContentSourceProperty = BindableProperty.Create("ContentSource", typeof(View), typeof(AudioPlayer), propertyChanged: OnContentSourcePropertyChanged);
        private static void OnContentSourcePropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var view = obj as AudioPlayer;
            var content = newValue as View;
            if (view != null && audioView != null && content != null)
            {
                audioView.Children.Add(content, 0, 1, 0, 1);
            }
        }

        public View ContentSource
        {
            get { return (View)GetValue(ContentSourceProperty); }
            set { SetValue(ContentSourceProperty, value); }
        }

        public static readonly BindableProperty AutoPlayProperty = BindableProperty.Create("AutoPlay", typeof(bool), typeof(BufferSlider), false);
        public bool AutoPlay
        {
            get { return (bool)GetValue(AutoPlayProperty); }
            set
            {
                SetValue(AutoPlayProperty, value);
                _isAutoPlay = (bool)GetValue(AutoPlayProperty);
            }
        }

        public static readonly BindableProperty IsLoopPlayProperty = BindableProperty.Create("IsLoopPlay", typeof(bool), typeof(BufferSlider), false);
        public bool IsLoopPlay
        {
            get { return (bool)GetValue(IsLoopPlayProperty); }
            set { SetValue(IsLoopPlayProperty, value); }
        }

        public static readonly BindableProperty SourceProperty = BindableProperty.Create("Source", typeof(string[]), typeof(AudioPlayer), propertyChanged: OnSourcePropertyChanged);

        private static void OnSourcePropertyChanged(BindableObject obj, object oldValue, object newValue)
        {
            var view = obj as AudioPlayer;
            string[] sourceArr = ((IEnumerable)newValue).Cast<object>().Select(x => x.ToString()).ToArray();

            if (view != null && audioPlayer != null && sourceArr[audioSourceNum] != null)
            {
                lblFileName.Text = sourceArr[audioSourceNum];
                if (sourceArr.Count() == 1)
                    btnStepBackward.IsEnabled = btnStepForward.IsEnabled = false;
                audioPlayer.LoadAsync(sourceArr[audioSourceNum]);
                _canPlay = true;
            }
        }

        public string[] Source
        {
            get { return (string[])GetValue(SourceProperty); }
            set
            {
                SetValue(SourceProperty, value);
                if (_canPlay && AutoPlay)
                    BtnPlay_Clicked(this, null);
            }
        }

        public static readonly BindableProperty PlayerBackgroundColorProperty = BindableProperty.Create("PlayerBackgroundColor", typeof(Color), typeof(AudioPlayer), Color.FromHex("#f7f2ca"));
        public Color PlayerBackgroundColor
        {
            get { return (Color)GetValue(PlayerBackgroundColorProperty); }
            set
            {
                SetValue(PlayerBackgroundColorProperty, value);
                if (audioControl != null)
                {
                    audioControl.BackgroundColor = (Color)GetValue(PlayerBackgroundColorProperty);
                }
            }
        }

        public AudioPlayer()
        {
            SetAudioPlayerView();
            RowSpacing = 0;
        }

        private void SetAudioPlayerView()
        {
            audioView = new Grid
            {
                BackgroundColor = Color.FromHex("#bcbaba"),
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height=new GridLength(1, GridUnitType.Star)  },
                    new RowDefinition { Height=100}
                }
            };

            lblFileName = new Label()
            {
                Margin = new Thickness(15, 0),
                LineBreakMode = LineBreakMode.MiddleTruncation,
                TextColor = Color.FromHex("605e5e"),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center
            };


            audioPlayer = DependencyService.Get<IAudioPlayer>(DependencyFetchTarget.NewInstance);
            audioPlayer.SeekPositionChanged += AudioPlayer_SeekPositionChanged;
            audioPlayer.Completed += AudioPlayer_Completed;
            audioPlayer.BuffringPositionChanged += AudioPlayer_BuffringPositionChanged;
            audioPlayer.SetDuration += AudioPlayer_SetDuration;

            loader = new ActivityIndicator();

            btnPlay = new Button
            {
                WidthRequest = 40,
                BackgroundColor = Color.Transparent,
                FontSize = 20,
                TextColor = Color.FromHex("#424242"),
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAPlay,
                VerticalOptions = LayoutOptions.StartAndExpand,
            };
            btnPlay.Clicked += BtnPlay_Clicked;

            audioSlider = new BufferSlider()
            {
                Minimum = 0,
                Value = 0,
                Maximum = 100,
            };
            audioSlider.ValueChanged += AudioSlider_ValueChanged;

            bufferingSlider = new BufferSlider()
            {
                Minimum = 0,
                Value = 0,
                Maximum = 100,
                IsSliderThumbVisible = false,
            };

            lblCurrentPosition = new Label() { Text = "00:00", Margin = new Thickness(10, 0, 0, 0), TextColor = Color.FromHex("605e5e"), HorizontalOptions = LayoutOptions.StartAndExpand };
            lblDuration = new Label() { Text = "00:00",  Margin = new Thickness(0, 0, 10, 0), TextColor = Color.FromHex("605e5e"), HorizontalOptions = LayoutOptions.EndAndExpand };

            btnStepForward = new Button()
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.FromHex("#424242"),
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAStepForward,
                BackgroundColor = Color.Transparent,
            };
            btnStepForward.Clicked += BtnStepForward_Clicked;

            btnStepBackward = new Button()
            {
                WidthRequest = 40,
                FontSize = 20,
                TextColor = Color.FromHex("#424242"),
                FontFamily = "FontAwesome",
                Text = FontAwesome.FAStepBackward,
                BackgroundColor = Color.Transparent,
                IsEnabled = false,
            };
            btnStepBackward.Clicked += BtnStepBackward_Clicked;

            audioControl = new Grid
            {
                Padding = 10,
                HeightRequest = 110,
                RowSpacing = 0,
                BackgroundColor = PlayerBackgroundColor,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition { Height=30},
                    new RowDefinition { Height=20},
                    new RowDefinition { Height=40},
                },

                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width=80},
                    new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width=new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition { Width=80},
                }
            };
            audioControl.Children.Add(lblFileName, 0, 5, 0, 1);
            audioControl.Children.Add(bufferingSlider, 0, 5, 1, 2);
            audioControl.Children.Add(audioSlider, 0, 5, 1, 2);
            audioControl.Children.Add(lblCurrentPosition, 0, 1, 2, 3);
            audioControl.Children.Add(btnStepBackward, 1, 2, 2, 3);
            audioControl.Children.Add(loader, 2, 3, 2, 3);
            audioControl.Children.Add(btnPlay, 2, 3, 2, 3);
            audioControl.Children.Add(btnStepForward, 3, 4, 2, 3);
            audioControl.Children.Add(lblDuration, 4, 5, 2, 3);

            audioView.Children.Add(audioControl, 0, 1, 1, 2);
            Children.Add(audioView);
        }

      


        private void AudioSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            var oldValue = (e.OldValue / 100) * ((audioPlayer.Duration) / 1000);
            var newValue = (e.NewValue / 100) * ((audioPlayer.Duration) / 1000);

            if ((oldValue - newValue) > 2 || (oldValue - newValue) < -2)
            {
                audioPlayer.Seek((int)((e.NewValue / 100) * ((audioPlayer.Duration) / 1000)));
                _seekChanged = true;
                if (bufferingSlider.Value <= audioSlider.Value || _isDurationSet)
                {
                    btnPlay.IsVisible = false;
                    loader.IsRunning = true;
                }
                else
                {
                    btnPlay.IsVisible = true;
                    loader.IsRunning = false;
                }
            }
        }

        private void BtnPlay_Clicked(object sender, EventArgs e)
        {
            if (audioPlayer.IsPlaying)
            {
                Pause();
                btnPlay.Text = FontAwesome.FAPlay;
            }
            else
            {
                Play();
                btnPlay.Text = FontAwesome.FAPause;
            }
        }

		bool _isDurationSet = false;

		private void AudioPlayer_SeekPositionChanged(object sender, double position)
        {
            if (!isPrepared && Math.Abs(position) <= 0)
                return;

            var values = position / audioPlayer.Duration;
			if (!_isDurationSet)
			{
				AudioPlayer_SetDuration(this, audioPlayer.Duration);
				_isDurationSet = true;
                Device.BeginInvokeOnMainThread(() =>
                {
                    loader.IsRunning = false;
                    btnPlay.IsVisible = true;
                });
			}
            Device.BeginInvokeOnMainThread(() =>
            {
                audioSlider.Value = values * 100;
                lblCurrentPosition.Text = string.Format("{0:mm\\:ss}", TimeSpan.FromMilliseconds(position));
                if (_seekChanged)
                {
                    btnPlay.IsVisible = true;
                    loader.IsRunning = false;
                    _seekChanged = false;
                }
            });
        }

		private void ResetPlayer()
		{
			try
			{
				Device.BeginInvokeOnMainThread(() =>
				{
					loader.IsRunning = true;
					btnPlay.IsVisible = false;
					audioSlider.Value = 0;
					bufferingSlider.Value = 0;
					lblCurrentPosition.Text = "00:00";
					lblDuration.Text = "00:00";

					lblFileName.Text = Source[audioSourceNum];
					bufferingSlider.Value = 0;
					if (Source.Count() - 1 == audioSourceNum)
						btnStepForward.IsEnabled = false;
					else
						btnStepForward.IsEnabled = true;

					if (audioSourceNum == 0)
						btnStepBackward.IsEnabled = false;
					else
						btnStepBackward.IsEnabled = true;

					if (Source.Count() == 1)
						btnStepBackward.IsEnabled = btnStepForward.IsEnabled = false;

					audioPlayer.LoadAsync(Source[audioSourceNum]);
					_isDurationSet = false;
					Play();
				});
			}
			catch
			{
			}
		}


        int count = 0;
        private void Play()
        {
			//if (_isAutoPlay)
			//{
			//    btnPlay.Text = FontAwesome.FAPause;
			//    _isAutoPlay = !_isAutoPlay;
			//    await Task.Delay(2500);
			//}

	            Device.BeginInvokeOnMainThread(async() =>
				   {
                       loader.IsRunning = true;
                       btnPlay.IsVisible = false;
			            int i = 1;
			            if (audioPlayer.Play())
			            {
			                while (!audioPlayer.IsPlaying && i <= 5)
			                {
			                    i++;
			                    await Task.Delay(1000);
			                    audioPlayer.Play();
			                }
		               
		                    if (audioPlayer.IsPlaying)
		                    {
		                        lblDuration.Text = string.Format("{0:mm\\:ss}", TimeSpan.FromMilliseconds(audioPlayer.Duration));
		                        btnPlay.Text = FontAwesome.FAPause;
		                    }
		                    else
		                    {
		                        btnPlay.Text = FontAwesome.FAPlay;
		                        audioSlider.Value = 0;
		                        lblDuration.Text = "00:00";
		                    }
			            }
			            else
			            {
			                if (count <= 5)
			                {
			                    count++;
			                    await Task.Delay(1000);
			                    Play();
			                    return;
			                }

			                Pause();
			                btnPlay.Text = FontAwesome.FAPlay;
			                lblDuration.Text = "00:00";
			            }
				   });

			//Device.BeginInvokeOnMainThread(() =>
			//{
			//    loader.IsRunning = false;
			//    btnPlay.IsVisible = true;
			//});
		}

        private void Pause()
        {
            audioPlayer.Pause();
        }

        private void StopAudio()
        {
            Pause();
            if (audioPlayer != null)
            {
                try
                {
                    audioPlayer.Stop();
                }
                catch { }
            }
        }


		private void BtnStepForward_Clicked(object sender, EventArgs e)
		{
			if (audioSourceNum >= Source.Count() - 1)
			{
				isPrepared = false;
				Device.BeginInvokeOnMainThread(() =>
				{
					btnPlay.Text = FontAwesome.FAPause;
				});
				return;
			}

			audioSourceNum++;
			ResetPlayer();
		}

		private void BtnStepBackward_Clicked(object sender, EventArgs e)
		{
			if (audioSourceNum <= 0)
				return;

			audioSourceNum--;
			ResetPlayer();
		}

		void AudioPlayer_SetDuration(object sender, double duration)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				lblDuration.Text = string.Format("{0:mm\\:ss}", TimeSpan.FromMilliseconds(duration));
			});
		}

		private void AudioPlayer_BuffringPositionChanged(object sender, int e)
		{
			bufferingSlider.Value = e;
		}

		private void AudioPlayer_Completed(object sender, EventArgs e)
		{
			if (IsLoopPlay)
				audioSourceNum--;
			BtnStepForward_Clicked(this, null);
		}

    }
}

using System;
using Xamarin.Forms;

namespace Xamarians.MediaPlayers.Internal
{
    public class BufferSlider : Slider
    {

        public event EventHandler<double> StopedDraging;
        public void OnStoppedDrag(Double value)
        {
            StopedDraging.Invoke(this, value);
        }
        public static readonly BindableProperty SliderColorProperty = BindableProperty.Create("SliderColor", typeof(Color), typeof(BufferSlider), Color.FromHex("#f7f2ca"));
        public static readonly BindableProperty IsSliderThumbVisibleProperty = BindableProperty.Create("IsSliderThumbVisible", typeof(bool), typeof(BufferSlider), true);

        public Color SliderColor
        {
            get { return (Color)GetValue(SliderColorProperty); }
            set { SetValue(SliderColorProperty, value); }
        }

        public bool IsSliderThumbVisible
        {
            get { return (bool)GetValue(IsSliderThumbVisibleProperty); }
            set { SetValue(IsSliderThumbVisibleProperty, value); }
        }
    }
}

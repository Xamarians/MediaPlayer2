using Android.Graphics;
using System.ComponentModel;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.ExportRenderer(typeof(BufferSlider), typeof(Xamarians.MediaPlayers.Droid.TimelineBarRederer))]
namespace Xamarians.MediaPlayers.Droid
{
    public class TimelineBarRederer : SliderRenderer
    {
        BufferSlider element;

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
            element = (BufferSlider)this.Element;
            if (Control == null || element == null)
                return;
            SetSliderColorAndThumb();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (BufferSlider.SliderColorProperty.PropertyName.Equals(e.PropertyName))
            {
                //SetSliderColor();
            }
            if (BufferSlider.IsSliderThumbVisibleProperty.PropertyName.Equals(e.PropertyName))
            {
               // SetSliderThumb();
            }
        }

        private void SetSliderColorAndThumb()
        {
            if (!element.IsSliderThumbVisible)
            {
                Control.Thumb.SetColorFilter(Android.Graphics.Color.Transparent, PorterDuff.Mode.Clear);
                Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(130, 111, 120), PorterDuff.Mode.SrcIn);
            }
            else
            {
                Control.Thumb.ClearColorFilter();
                //Control.Thumb.SetColorFilter(Android.Graphics.Color.DarkRed, PorterDuff.Mode.Darken);
                Control.ProgressDrawable.SetColorFilter(Android.Graphics.Color.Rgb(244, 66, 146), PorterDuff.Mode.SrcIn);
            }
        }
    }
}
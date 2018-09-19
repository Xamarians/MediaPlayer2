using System;
using Xamarin.Forms.Platform.iOS;
using Xamarians.MediaPlayers.Internal;
using Xamarin.Forms;

[assembly:ExportRenderer(typeof(BufferSlider), typeof(Xamarians.MediaPlayers.iOS.ExtendedSliderRenderer))]
namespace Xamarians.MediaPlayers.iOS
{
    public class ExtendedSliderRenderer : SliderRenderer
    {
        BufferSlider element;
        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);
            element = (BufferSlider)this.Element;
            if (Control == null || element == null)
                return;
            Control.Continuous = false;
            Control.TouchUpInside += Control_TouchUpInside;
            SetSliderColorAndThumb();

        }
        private void Control_TouchUpInside(object sender, EventArgs e)
        {
            element.OnStoppedDrag(Control.Value);
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private void SetSliderColor()
        {

        }

        private void SetSliderThumb()
        {
            if (!element.IsSliderThumbVisible)
            {
                Control.SizeThatFits(new CoreGraphics.CGSize(5, 5));
                Control.SetThumbImage(new UIKit.UIImage("slider.png"), UIKit.UIControlState.Normal);
            }
            else
            {
                Control.SizeThatFits(new CoreGraphics.CGSize(5, 5));
                Control.SetThumbImage(new UIKit.UIImage("slider_black.png"), UIKit.UIControlState.Normal);
            }

        }

        private void SetSliderColorAndThumb()
        {
            if (!element.IsSliderThumbVisible)
            {
                Control.SizeThatFits(new CoreGraphics.CGSize(25, 25));
                Control.SetThumbImage(new UIKit.UIImage("slider_black.png"), UIKit.UIControlState.Normal);
            }
            else
            {
                Control.SizeThatFits(new CoreGraphics.CGSize(25, 25));
                Control.SetThumbImage(new UIKit.UIImage("slider.png"), UIKit.UIControlState.Normal);
            }
        }

    }
}

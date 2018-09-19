using Android.App;

namespace Xamarians.MediaPlayers.Droid
{
    public class PlateformService
    {
        public static void Init(Activity activity)
        {
            NativePlayerRenderer.Init(activity);
        }
    }
}
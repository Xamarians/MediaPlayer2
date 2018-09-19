using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;

namespace VideoPlayerDemo.Droid
{
    [Activity(Label = "Video Player", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;
            AndroidEnvironment.UnhandledExceptionRaiser += AndroidEnvironment_UnhandledExceptionRaiser;
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            Xamarians.MediaPlayers.Droid.PlateformService.Init(this);
            LoadApplication(new App());
        }

        private void AndroidEnvironment_UnhandledExceptionRaiser(object sender, RaiseThrowableEventArgs e)
        {
            e.Handled = true;
            var alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle("Exception");
            alertDialog.SetMessage(e.Exception.Message + "____" + e.Exception.ToString());
            alertDialog.SetNeutralButton("Ok", (s, ee) =>
            {
            });
            alertDialog.Show();
        }
    }
}


using System;

using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ConIView.Droid
{
    [Activity( Label = "ConIView", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IDatabaseFileProvider
    {
        [Obsolete]
        public string DatabaseDirectory => Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

        protected override void OnCreate( Bundle savedInstanceState )
        {
            base.OnCreate( savedInstanceState );
            global::Xamarin.Forms.Forms.Init( this, savedInstanceState );
            FFImageLoading.Forms.Platform.CachedImageRenderer.Init( true );
            App.FileProvider = this;
            LoadApplication( new App() );
        }
    }
}

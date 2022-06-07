using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;

namespace ConIView.Droid
{
	[Activity( Label = "WildSpy", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IPictureFileProvider
	{
		/// <summary>
		/// Return the path to the first mounted SD card if there is one.
		/// </summary>
		[Obsolete]
		public string ExternalDirectory
		{
			get
			{
				string directory = "";

				List<string> candidateProcMountEntries = System.IO.File.ReadAllText( "/proc/mounts" ).Split( '\n', '\r' ).ToList();
				string bestCandidate = candidateProcMountEntries.FirstOrDefault( s => ( s.IndexOf( "storage", StringComparison.OrdinalIgnoreCase ) >= 0 ) &&
					( s.IndexOf( "emulated", StringComparison.OrdinalIgnoreCase ) < 0 ) && ( s.IndexOf( "sdcard", StringComparison.OrdinalIgnoreCase ) >= 0 ) );

				if ( string.IsNullOrWhiteSpace( bestCandidate ) == false )
				{
					List<string> cardEntry = bestCandidate.Split( ' ' ).ToList();
					directory = cardEntry.FirstOrDefault( s => s.IndexOf( "/storage/", StringComparison.OrdinalIgnoreCase ) >= 0 );
				}

				return directory;
			}
		}

		/// <summary>
		/// Return the path to the internal storage for the picture files. This is used if there is no external storage or the files are not found 
		/// on the external storage
		/// </summary>
		[Obsolete]
		public string InternalDirectory => Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

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

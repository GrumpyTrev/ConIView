using Xamarin.Forms;
using ConIView.Views;
using MediaManager;
using ConIView.Data;

namespace ConIView
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			CrossMediaManager.Current.Init();

			FFImageLoading.ImageService.Instance.Initialize( new FFImageLoading.Config.Configuration()
			{
				ExecuteCallbacksOnUIThread = true
			} );

			Storage.GetItems( null );

			MainPage = new NavigationPage( new ListPage() )
			{
				BarTextColor = Color.White,
				BarBackgroundColor = ( Color )Current.Resources[ "primaryGreen" ]
			};
		}

		public static IDatabaseFileProvider FileProvider { get; set; } = null;
	}
}

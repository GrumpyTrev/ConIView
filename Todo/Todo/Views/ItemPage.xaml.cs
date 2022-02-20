using System;
using System.Collections.Generic;
using ConIView.Models;
using MediaManager;
using Xamarin.Forms;

namespace ConIView.Views
{
	public partial class ItemPage : ContentPage
	{
		public ItemPage() => InitializeComponent();

		protected override void OnAppearing()
		{
			base.OnAppearing();

			carouselView.ItemsSource = ( ( ImageSet )BindingContext ).ImageDetails;
		}

		private async void OnCancelClicked( object sender, EventArgs e ) => await Navigation.PopAsync();

		private async void OnIdentClicked( object sender, EventArgs e )
		{
			ImageSet context = BindingContext as ImageSet;
			await Navigation.PushAsync( new NotesPage()
			{
				BindingContext = context.ImageDetails[ 0 ].NotePages[ "ident" ]
			} ); ;
		}

		private async void Sound_Clicked( object sender, EventArgs e )
		{
			ImageDetail itemShowing = ( ( ImageSet )BindingContext ).ImageDetails[ carouselView.Position ];
			List<MediaManager.Library.IMediaItem> itemsToPlay = new List<MediaManager.Library.IMediaItem>();

			foreach ( string soundFile in itemShowing.Sounds )
			{
				itemsToPlay.Add( await CrossMediaManager.Current.Extractor.CreateMediaItem( soundFile ) );
			}

			_ = CrossMediaManager.Current.Play( itemsToPlay );
		}

		/// <summary>
		/// Called when an image has been loaded by the FFImageLoading library
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnImageLoaded( object sender, FFImageLoading.Forms.CachedImageEvents.SuccessEventArgs e )
		{
			View senderView = sender as View;

			Console.WriteLine( $"Image loaded current {e.ImageInformation.CurrentWidth} x {e.ImageInformation.CurrentHeight}" +
							   $" original {e.ImageInformation.OriginalWidth} x {e.ImageInformation.OriginalHeight}" +
							   $" path {e.ImageInformation.Path} View size {senderView.Width} x {senderView.Height}" +
							   $" request {senderView.WidthRequest} x {senderView.HeightRequest}" );

			// Only adjust the height of the view containing the image if the height of the image is less than it's width. If the image's
			// height is greater than it's width then it will fully occupy the view
			// Don't adjust if the view hasn't been displayed yet
			if ( senderView.WidthRequest != -1 )
			{
				if ( e.ImageInformation.CurrentHeight < e.ImageInformation.CurrentWidth )
				{
					// Determine what the height will be when the image is scaled 
					int requiredHeight = ( int )( e.ImageInformation.CurrentHeight * ( senderView.WidthRequest / e.ImageInformation.CurrentWidth ) );

					// If it's not already this height then set it now
					if ( requiredHeight != senderView.HeightRequest )
					{
						senderView.HeightRequest = requiredHeight;
						Console.WriteLine( $"Height change requested for path {e.ImageInformation.Path} height { requiredHeight}" );
					}
				}
			}
			else
			{
				_ = FFImageLoading.ImageService.Instance.InvalidateCacheEntryAsync( e.ImageInformation.CacheKey, FFImageLoading.Cache.CacheType.All );
			}
		}
	}
}

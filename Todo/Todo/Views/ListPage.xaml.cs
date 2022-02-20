using System;
using System.Linq;
using ConIView.Data;
using ConIView.Models;
using Xamarin.Forms;

namespace ConIView.Views
{
	public partial class ListPage : ContentPage
	{
		public ListPage() => InitializeComponent();

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// This called on every return from the ItemPage, so check if the listview has already been loaded
			if ( listView.ItemsSource == null )
			{
				// First time through - get the data
				Storage.GetItems( ItemsAvailable );
			}
		}

		private void ItemsAvailable()
		{
			// If there is only one list, or if no list has been selected by the user, then select the first list
			if ( ( Storage.ImageCollections.Count == 1 ) || ( selectedList == -1 ) )
			{
				selectedList = 0;
			}

			ImageCollection selectedItems = Storage.ImageCollections[ selectedList ];

			listView.ItemsSource = selectedItems.ImageSets;

			// Make sure that the listview is scrolled to the top
			if ( selectedItems.ImageSets.Count > 0 )
			{
				listView.ScrollTo( selectedItems.ImageSets[ 0 ], ScrollToPosition.Start, false );
			}

			Title = Storage.ImageCollections[ selectedList ].Title;
		}

		private async void OnListItemSelected( object sender, ItemTappedEventArgs e )
		{
			if ( e.Item != null )
			{
				await Navigation.PushAsync( new ItemPage
				{
					BindingContext = e.Item as ImageSet
				} );
			}
		}

		private async void OnSelectList( object sender, EventArgs e )
		{
			// Display an action sheet with all the names of the available lists
			string listName = await DisplayActionSheet( "Choose list", "Cancel", null, Storage.ImageCollections.Select( item => item.Title ).ToArray() );
			ImageCollection selectedItems = Storage.ImageCollections.FirstOrDefault( item => item.Title == listName );

			if ( selectedItems != null )
			{
				selectedList = Storage.ImageCollections.IndexOf( selectedItems );

				ItemsAvailable();
			}
		}

		private static int selectedList = -1;

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
	}
}

﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConIView.Models;
using System.Linq;
using System;

namespace ConIView.Data
{
	internal class Storage
	{
		public static async void GetItems( Action itemsAvailable )
		{
			// Is the data already available
			if ( ImageCollections == null )
			{
				// Data not available, are we already in the process of getting the data
				if ( readingData == false )
				{
					readingData = true;

					// Start reading the data off the main thread
					await Task.Run( () =>
					{
						// Build up the data in a local collection
						List<ImageCollection> localCollection = new List<ImageCollection>();

						// Get the paths of all the collections in the storage folder
						string[] collectionFolders = Directory.GetDirectories( Constants.StoreFolder );

						// Now read in each collection. 
						foreach ( string folderName in collectionFolders )
						{
							// Create a new ImageCollection for this folder and add it to the main collection
							ImageCollection images = new ImageCollection
							{
								Title = Path.GetFileName( folderName )
							};
							localCollection.Add( images );

							// Get all the image sets in the folder and sort them alphabetically
							string[] setFolders = Directory.GetDirectories( folderName );
							Array.Sort( setFolders );

							// Now read in the ImageSets
							foreach ( string itemName in setFolders )
							{
								string folderShortName = Path.GetFileName( itemName );
								Dictionary<string, NotesDetail> notePages = new Dictionary<string, NotesDetail>();

								// Get the pathnames of all the files in this directory and sort them
								List<string> fileNames = Directory.GetFiles( itemName ).ToList();
								fileNames.Sort();

								// Get the names of all the imgage files
								List<string> itemContents = fileNames.Where( item => ( Path.GetExtension( item ) == ".jpg" ) ||
										( Path.GetExtension( item ) == ".png" ) ).ToList();

								// Check if there are any sound files
								List<string> soundFiles = fileNames.Where( item => Path.GetExtension( item ) == ".mp3" ).ToList();

								// Check for any text files.
								List<string> textFiles = fileNames.Where( item => Path.GetExtension( item ) == ".txt" ).ToList();

								// If there is an 'ident' file then extract the identity fields from it
								string identFile = textFiles.SingleOrDefault( item => Path.GetFileNameWithoutExtension( item ) == "ident" );
								IdentNotes ident = IdentNotes.ExtractNotes( identFile );

								// Add the ident notes to the note pages collection
								if ( ident != null )
								{
									notePages[ "ident" ] = new NotesDetail() { Name = folderShortName + " - Ident", Notes = ident.OtherNotes };
								}

								// Create an ImageSet for this folder and add to the ImageCollection
								ImageSet mainItem = new ImageSet
								{
									Name = folderShortName,
									Image = itemContents[ 0 ],
									Science = ident.Science,
									Family = ident.Family,
									Length = ident.Length,
									Sounds = soundFiles
								};

								images.ImageSets.Add( mainItem );

								// Add a ItemDetail entry for each image file
								foreach ( string imageName in itemContents )
								{
									mainItem.ImageDetails.Add( new ImageDetail()
									{
										Name = GetImageName( imageName ),
										Image = imageName,
										NotePages = notePages,
										Sounds = soundFiles,
										Ident = ident
									} );
								}
							}
						}

						// Copy to the public collection
						ImageCollections = localCollection;

						readingData = false;
					} );

					// Data now available, call the delegate
					itemsAvailable?.Invoke();
				}
			}
			else
			{
				// Data already availbael so call the delegate 
				itemsAvailable?.Invoke();
			}
		}

		/// <summary>
		/// The data read from storage
		/// </summary>
		public static List<ImageCollection> ImageCollections { get; private set; }

		/// <summary>
		/// Get the image name from the path, removing any preceeding number prefix and capitalize the first word
		/// </summary>
		/// <param name="imagePath"></param>
		/// <returns></returns>
		private static string GetImageName( string imagePath )
		{
			string imageName = Path.GetFileNameWithoutExtension( imagePath );

			int index = imageName.IndexOf( '_' );
			if ( index != -1 )
			{
				if ( int.TryParse( imageName.Substring( 0, index ), out _ ) == true )
				{
					imageName = imageName.Substring( index + 1 );
				}
			}

			return imageName.First().ToString().ToUpper() + imageName.Substring( 1 );
		}

		/// <summary>
		/// Keep track of whether or not the collection data is beomg read
		/// </summary>
		private static bool readingData = false;
	}
}

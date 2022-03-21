using System.Collections.Generic;
using System.IO;

namespace ConIView.Models
{
	/// <summary>
	/// The ImageCollection class represents a collection of related subjects, each subject is represented by an ImageSet class
	/// </summary>
	public class ImageCollection
	{
		/// <summary>
		/// The title of this collection shown when displaying the contents of tbe collection and when choosing a collection to display
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// These are the ImageSet instances making up the collection
		/// </summary>
		public List<ImageSet> ImageSets { get; } = new List<ImageSet>();
	}

	/// <summary>
	/// The ImageSet class holds a number of images of a particular subject in an ImageCollection
	/// </summary>
	public class ImageSet
	{
		/// <summary>
		/// Name of this ImageSet
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Information pulled out of the 'ident' file associated with the subject
		/// </summary>
		public string Science { get; set; }
		public string Family { get; set; }
		public string Length { get; set; }

		/// <summary>
		/// Properties allowing the availability of the key values to be bound
		/// </summary>
		public bool ShowScience => Science.Length > 0;
		public bool ShowFamily => Family.Length > 0;
		public bool ShowLength => Length.Length > 0;

		/// <summary>
		/// Name of the main image used to represent the subject. This is the first image from the ImageDetails collection
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// Collection of sound files held for the subject.
		/// This is common to all subject images
		/// </summary>
		public List<string> Sounds { get; set; }

		/// <summary>
		/// Are there any sounds held for the subject
		/// </summary>
		public bool HasSounds => Sounds != null;

		/// <summary>
		/// The collection of all the images held for the subject 
		/// </summary>
		public List<ImageDetail> ImageDetails { get; } = new List<ImageDetail>();
	}

	/// <summary>
	/// An image associated with a subjewct
	/// </summary>
	public class ImageDetail
	{
		/// <summary>
		/// The name of this image - derived from the image file name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Collection of info files held for the subject. Indexed by file name.
		/// This is common to all subject images
		/// </summary>
		public Dictionary<string, NotesDetail> NotePages { get; set; } = null;

		/// <summary>
		/// The info pulled out of the 'ident' file. Common to all subject images
		/// </summary>
		public IdentNotes Ident { get; set; }

		/// <summary>
		/// Name of the image file
		/// </summary>
		public string Image { get; set; }

		/// <summary>
		/// Collection of sound files held for the subject.
		/// This is common to all subject images
		/// </summary>
		public List<string> Sounds { get; set; }

		/// <summary>
		/// Are there any sounds held for the subject
		/// </summary>
		public bool HasSounds => Sounds != null;
	}

	/// <summary>
	/// Represents a note/info file held for the subject
	/// </summary>
	public class NotesDetail
	{
		/// <summary>
		/// Name of the file
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// File path
		/// </summary>
		public string Notes { get; set; }
	}

	public class IdentNotes
	{
		/// <summary>
		/// The key values pulled out of the 'ident' file.
		/// </summary>
		public string Science { get; set; } = "";
		public string Family { get; set; } = "";
		public string Status { get; set; } = "";
		public string Conservation { get; set; } = "";
		public string Breeding { get; set; } = "";
		public string Wintering { get; set; } = "";
		public string Length { get; set; } = "";
		public string Wingspan { get; set; } = "";
		public string Weight { get; set; } = "";

		/// <summary>
		/// Properties allowing the availability of the key values to be bound
		/// </summary>
		public bool ShowScience => Science.Length > 0;
		public bool ShowFamily => Family.Length > 0;
		public bool ShowStatus => Status.Length > 0;
		public bool ShowConservation => Conservation.Length > 0;
		public bool ShowBreeding => Breeding.Length > 0;
		public bool ShowWintering => Wintering.Length > 0;
		public bool ShowLength => Length.Length > 0;
		public bool ShowWingspan => Wingspan.Length > 0;
		public bool ShowWeight => Weight.Length > 0;

		/// <summary>
		/// The rest of the ident file
		/// </summary>
		public string OtherNotes { get; set; } = "";

		/// <summary>
		/// Extract all the known key values from the file
		/// </summary>
		/// <param name="notesFile"></param>
		/// <returns></returns>
		public static IdentNotes ExtractNotes( string notesFile )
		{
			IdentNotes ident = new IdentNotes();

			if ( File.Exists( notesFile ) == true )
			{
				// Any lines that do not start with a keyword are added to this collection
				List<string> nonKeywordLines = new List<string>();

				IEnumerable<string> lines = File.ReadLines( notesFile );
				foreach ( string line in lines )
				{
					// Look for keyword lines
					if ( line.StartsWith( "science:" ) == true )
					{
						ident.Science = line.Substring( "science:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "family:" ) == true )
					{
						ident.Family = line.Substring( "family:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "status:" ) == true )
					{
						ident.Status = line.Substring( "status:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "conservation:" ) == true )
					{
						ident.Conservation = line.Substring( "conservation:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "breeding:" ) == true )
					{
						ident.Breeding = line.Substring( "breeding:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "wintering:" ) == true )
					{
						ident.Wintering = line.Substring( "wintering:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "length:" ) == true )
					{
						ident.Length = line.Substring( "length:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "wingspan:" ) == true )
					{
						ident.Wingspan = line.Substring( "wingspan:".Length ).TrimStart();
					}
					else if ( line.StartsWith( "weight:" ) == true )
					{
						ident.Weight = line.Substring( "weight:".Length ).TrimStart();
					}
					else
					{
						// Not a keyword
						nonKeywordLines.Add( line );
					}
				}

				// Put all the non-keyword lines back together
				ident.OtherNotes = string.Join( System.Environment.NewLine, nonKeywordLines );
			}

			return ident;
		}

	}
}

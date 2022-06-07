namespace ConIView
{
	public interface IPictureFileProvider
	{
		/// <summary>
		/// Return the path to the first mounted SD card if there is one.
		/// </summary>
		string ExternalDirectory { get; }

		/// <summary>
		/// Return the path to the internal storage for the picture files. This is used if there is no external storage or the files are not found 
		/// on the external storage
		/// </summary>
		string InternalDirectory { get; }
	}
}

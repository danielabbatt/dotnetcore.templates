namespace Templates.WindowsService.Models
{
	/// <summary>
	/// Integrator Configuration
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// The default filename
		/// </summary>
		public const string DefaultFilename = "appsettings.json";

		/// <summary>
		/// The name of the loaded configuration file, can be overwritten at the command line to load a different file
		/// </summary>
		public string ConfigFile { get; set; }
	}
}

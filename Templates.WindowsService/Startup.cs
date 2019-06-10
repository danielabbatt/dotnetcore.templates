using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System.Collections.Generic;
using System.IO;
using Templates.WindowsService.Models;

namespace Templates.WindowsService
{
	internal static class Startup
	{
		internal static void BuildConfig(IConfigurationBuilder configurationBuilder, string[] args)
		{
			var appsettingsFilename = Configuration.DefaultFilename;
			// If specifying any command line arguments, the first argument must be the path to the appsettings.json file ConfigFile: xxx
			if (args.Length > 0)
			{
				appsettingsFilename = args[0];
			}
			// Convert appsettingsFilename to absolute path for the ConfigurationBuilder to be able to find it
			appsettingsFilename = Path.GetFullPath(appsettingsFilename);

			configurationBuilder
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile(appsettingsFilename, false, false)
				// Set which filename we loaded from
				.AddInMemoryCollection(new List<KeyValuePair<string, string>> {
					new KeyValuePair<string, string>(nameof(Configuration.ConfigFile), appsettingsFilename)
				});
		}

		internal static void ConfigureLogging(HostBuilderContext context, ILoggingBuilder builder)
		{
			// Set up SeriLog
			var config = context.Configuration.GetSection("Logging");
			var loggerConfiguration = new LoggerConfiguration()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
				.ReadFrom.Configuration(config);
#if DEBUG
			loggerConfiguration.WriteTo.Debug();
#endif

			Log.Logger = loggerConfiguration.CreateLogger();

			// Enable using Serilog for the ILogger Microsoft extensions
			builder.AddSerilog(dispose: true);
		}

		internal static void ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
		{
			// Enable injection of configuration using the "Options" pattern
			services.AddOptions();
			services.Configure<Configuration>(hostContext.Configuration);

			// Add the main application in so it can be created and used by the MainService
			services.AddTransient<Application>();

			// This is the main hosted service
			services.AddHostedService<MainService>();
		}
	}
}

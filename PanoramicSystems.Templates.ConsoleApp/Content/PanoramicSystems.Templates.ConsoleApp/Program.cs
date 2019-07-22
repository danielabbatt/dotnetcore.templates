using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PanoramicSystems.Templates.ConsoleApp.Models;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PanoramicSystems.Templates.ConsoleApp
{
	internal static class Program
	{
		private static async Task Main(string[] args)
		{
			// Set up basic logging
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();

			try
			{
				// Set up the serviceprovider
				var serviceCollection = new ServiceCollection();
				var configurationRoot = ConfigureServices(serviceCollection, "appsettings.json");
				var serviceProvider = serviceCollection.BuildServiceProvider();

				// Set Console Title from config
				Console.Title = $"{configurationRoot.GetSection("Configuration:ConsoleWindowTitle").Value ?? "ApplicationName"} v{ThisAssembly.AssemblyFileVersion}";

				// Load Serilog configuration
				Log.Logger = new LoggerConfiguration()
					.ReadFrom.Configuration(configurationRoot)
					.Enrich.FromLogContext()
					.CreateLogger();

				var cancellationTokenSource = new CancellationTokenSource();
				Console.WriteLine("STARTING APP");
				await serviceProvider.GetService<IApplicationName>().RunAsync(cancellationTokenSource.Token).ConfigureAwait(false);
				Console.WriteLine("DONE");
			}
			catch (Exception e)
			{
				Log.Error(e, e.Message);
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private static IConfigurationRoot ConfigureServices(IServiceCollection serviceCollection, string configFilePath)
		{
			// Add SeriLog based logging
			serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

			// Load configuration
			var configuration = new ConfigurationBuilder()
				 .SetBasePath(Directory.GetCurrentDirectory())
				 .AddJsonFile(configFilePath, false)
				 .Build();

			// Support IOptions<Configuration>
			serviceCollection.AddOptions();
			serviceCollection.Configure<Configuration>(configuration.GetSection("Configuration"));

			// Add any transient/singleton services here

			// Add the app
			serviceCollection.AddTransient<IApplicationName, ApplicationName>();

			// Return configuration for immediate use during init
			return configuration;
		}
	}
}

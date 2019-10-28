using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Templates.ConsoleApp.Models;
using Templates.ConsoleApp.Models.Exceptions;

namespace Templates.ConsoleApp
{
	internal static class Program
	{
		private static async Task<int> Main(string[] args)
		{
			Serilog.Debugging.SelfLog.Enable(msg =>
			{
				Debug.WriteLine(msg);
				Console.Error.WriteLine(msg);
			});

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

				using var cancellationTokenSource = new CancellationTokenSource();
				Console.WriteLine($"STARTING APP with args: {string.Join(", ", args)}");

				await serviceProvider
					.GetService<IApplicationName>()
					.RunAsync(cancellationTokenSource.Token)
					.ConfigureAwait(false);

				Console.WriteLine("DONE");
				return ExitCode.Ok;
			}
			catch (ConfigurationException ex)
			{
				Log.Error("**" + ex.Message + "**");
				return ExitCode.ConfigurationException;
			}
			catch (Exception ex)
			{
				var dumpPath = Path.GetTempPath();
				var dumpFile = $"ApplicationName-Error-{Guid.NewGuid()}.txt";
				File.WriteAllText(Path.Combine(dumpPath, dumpFile), ex.ToString());
				Log.Error(ex.Message);
				return ExitCode.UnexpectedException;
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

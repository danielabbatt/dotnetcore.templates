using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Templates.WindowsService.Exceptions;

namespace Templates.WindowsService
{
	/// <summary>
	/// The main console application
	/// </summary>
	public static class Program
	{
		public const string ProductName = "ExampleService";
		internal static bool IsRunningAsService => Console.IsInputRedirected;

		/// <summary>
		/// Main execution class
		/// </summary>
		/// <param name="args">Command line arguments</param>
		public static async Task<int> Main(string[] args)
		{
			try
			{
				Serilog.Debugging.SelfLog.Enable(msg =>
				{
					Debug.WriteLine(msg);
					Console.Error.WriteLine(msg);
				});

				// Set up initial basic logging to trap errors starting up
				var loggerConfiguration = new LoggerConfiguration().WriteTo.Console();

				if (IsRunningAsService)
				{
					// Update paths to refer to where the service is loaded from for loading config files
					var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
					var pathToContentRoot = Path.GetDirectoryName(pathToExe);
					Directory.SetCurrentDirectory(pathToContentRoot);

					loggerConfiguration.WriteTo.EventLog(
						Program.ProductName,
						eventIdProvider: new SerilogEventLogIdProvider(),
						manageEventSource: true);
				}

				Log.Logger = loggerConfiguration.CreateLogger();

				var host = new HostBuilder()
					.ConfigureAppConfiguration(configurationBuilder => Startup.BuildConfig(configurationBuilder, args))
					.ConfigureLogging(Startup.ConfigureLogging)
					.ConfigureServices(Startup.ConfigureServices);

				if (IsRunningAsService)
				{
					await host.RunAsServiceAsync().ConfigureAwait(false);
				}
				else
				{
					await host.RunConsoleAsync().ConfigureAwait(false);
				}

				return (int)ExitCode.Ok;
			}
			catch (OperationCanceledException)
			{
				// This is normal for using CTRL+C to exit the run
				Log.Error("** Execution run cancelled - exiting **");
				return (int)ExitCode.RunCancelled;
			}
			catch (ConfigurationException ex)
			{
				Log.Error("**" + ex.Message + "**");
				return (int)ExitCode.ConfigurationException;
			}
			catch (Exception ex)
			{
				var dumpPath = Path.GetTempPath();
				var dumpFile = $"{ProductName}-Error-{Guid.NewGuid()}.txt";
				File.WriteAllText(Path.Combine(dumpPath, dumpFile), ex.ToString());
				Log.Error(ex.Message);
				return (int)ExitCode.UnexpectedException;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}

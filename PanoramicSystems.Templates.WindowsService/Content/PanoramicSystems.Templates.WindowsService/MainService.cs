using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PanoramicSystems.Templates.WindowsService
{
	public class MainService : IHostedService, IDisposable
	{
		private readonly Microsoft.Extensions.Logging.ILogger _logger;
		private readonly Logger _eventLog;
		private Task _executingTask;
		private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
		private readonly Application _application;

		public MainService(ILogger<MainService> logger, Application application)
		{
			_logger = logger;
			// Only create an eventLog logger if running as a service
			if (Program.IsRunningAsService)
			{
				_eventLog = new LoggerConfiguration()
				.WriteTo.EventLog(Program.ProductName, eventIdProvider: new SerilogEventLogIdProvider(), manageEventSource: true)
				.CreateLogger();
			}
			_application = application;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var message = $"Starting Service Version {ThisAssembly.AssemblyFileVersion}...";
			if (Program.IsRunningAsService)
			{
				_eventLog.Information(message);
			}
			_logger.LogInformation(message);
			_executingTask = ExecuteAsync(_stoppingCts.Token);
			return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
		}

		protected Task ExecuteAsync(CancellationToken stoppingToken)
		{
			_logger.LogInformation($"MainService executing at: {DateTimeOffset.UtcNow}");
			return _application.RunAsync(stoppingToken);
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			var message = $"Stopping Service Version {ThisAssembly.AssemblyFileVersion}...";
			if (Program.IsRunningAsService)
			{
				_eventLog.Information(message);
			}
			_logger.LogInformation(message);
			if (_executingTask != null)
			{
				try
				{
					_stoppingCts.Cancel();
				}
				finally
				{
					await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(false);
					message = $"Stopped Service Version {ThisAssembly.AssemblyFileVersion}...";
					if (Program.IsRunningAsService)
					{
						_eventLog.Information(message);
					}
					_logger.LogInformation(message);
				}
			}
		}

		public void Dispose()
		{
			_logger.LogInformation("Disposed.");
			_stoppingCts.Cancel();
			//base.Dispose();
		}
	}
}

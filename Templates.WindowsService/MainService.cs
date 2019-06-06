using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.WindowsService
{
	public partial class MainService : ServiceBase, IHostedService
	{
		private const string EventLogSourceName = Program.ProductName;
		private readonly Microsoft.Extensions.Logging.ILogger _logger;
		private readonly Logger _eventLog;
		private Task _executingTask;
		private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
		private readonly Application _application;

		public MainService(ILogger<MainService> logger, Application application)
		{
			_logger = logger;
			_eventLog = new LoggerConfiguration()
				.WriteTo.EventLog(EventLogSourceName, manageEventSource: true)
				.CreateLogger();
			_application = application;
		}

		/// <summary>
		/// Tasks to perform on start
		/// </summary>
		protected override void OnStart(string[] args)
		{
			_eventLog.Information($"Starting Service Version {ThisAssembly.AssemblyFileVersion}...");
			StartAsync(default)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
			_eventLog.Information("Service Started.");
		}

		/// <summary>
		/// Tasks to perform on stop
		/// </summary>
		protected override void OnStop()
		{
			_eventLog.Information("Service Stopping...");
			StopAsync(default)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
			_eventLog.Information("Service Stopped.");
		}

		/// <summary>
		/// Tasks to perform on shutdown
		/// </summary>
		protected override void OnShutdown()
		{
			_eventLog.Information("Service Stopping due to shutdown...");
			StopAsync(default)
				.ConfigureAwait(false)
				.GetAwaiter()
				.GetResult();
			_eventLog.Information("Service Stopped due to shutdown.");
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation($"Starting Service Version {ThisAssembly.AssemblyInformationalVersion}...");

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
			_logger.LogInformation("Stopping...");
			if (_executingTask != null)
			{
				try
				{
					_stoppingCts.Cancel();
				}
				finally
				{
					await Task.WhenAny(_executingTask, Task.Delay(-1, cancellationToken)).ConfigureAwait(false);
					_logger.LogInformation("Stopped.");
				}
			}
		}

		new public void Dispose()
		{
			_logger.LogInformation("Disposed.");
			_stoppingCts.Cancel();
			base.Dispose();
		}
	}
}

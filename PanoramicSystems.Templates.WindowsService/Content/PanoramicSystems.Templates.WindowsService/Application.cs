using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PanoramicSystems.Templates.WindowsService
{
	public class Application
	{
		private readonly ILogger<Application> _logger;

		public Application(ILogger<Application> logger) => _logger = logger;

		public async Task RunAsync(CancellationToken stoppingToken)
		{
			var executionCount = 0;
			var loopDelay = TimeSpan.FromSeconds(5);
			try
			{
				while (true)
				{
					_logger.LogInformation($"Doing something: {++executionCount}");
					await Task.Delay(loopDelay, stoppingToken).ConfigureAwait(false);
				}
			}
			catch (Exception ex)
			{
				switch (ex)
				{
					case TaskCanceledException taskCanceledException:
						if (taskCanceledException.CancellationToken == stoppingToken)
						{
							// We're here because our main token caused the cancellation
						}
						break;
					case OperationCanceledException operationCanceledException:
						if (operationCanceledException.CancellationToken == stoppingToken)
						{
							// We're here because our main token caused the cancellation
						}
						break;
					default:
						// A cancellation we weren't expecting has occurred
						_logger.LogCritical($"A critical error occurred during execution: {ex}");
						break;
				}
			}
			finally
			{
				_logger.LogInformation("Leaving execution.");
			}
		}
	}
}

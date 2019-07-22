using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.ConsoleApp
{
	public class ApplicationName : IApplicationName
	{
		private readonly ILogger _logger;

		public ApplicationName(ILogger<ApplicationName> logger) => _logger = logger;

		public async Task RunAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Application started.");

			await Task.Delay(1000).ConfigureAwait(false);

			_logger.LogInformation("Application finished.");
		}
	}
}

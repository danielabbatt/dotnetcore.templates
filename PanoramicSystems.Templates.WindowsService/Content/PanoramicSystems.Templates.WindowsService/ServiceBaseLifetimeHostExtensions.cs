using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PanoramicSystems.Templates.WindowsService
{
	public static class ServiceBaseLifetimeHostExtensions
	{
		public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
			=> hostBuilder.ConfigureServices((_, services) => services.AddSingleton<IHostLifetime, ServiceBaseLifetime>());

		public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
			=> hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
	}
}

using System.Threading;
using System.Threading.Tasks;

namespace PanoramicSystems.Templates.ConsoleApp
{
	public interface IApplicationName
	{
		Task RunAsync(CancellationToken cancellationToken);
	}
}

using System.Threading;
using System.Threading.Tasks;

namespace Templates.ConsoleApp
{
	public interface IApplicationName
	{
		Task RunAsync(CancellationToken cancellationToken);
	}
}

using Microsoft.Extensions.Hosting;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Templates.WindowsService
{
	// https://github.com/aspnet/Hosting/blob/2a98db6a73512b8e36f55a1e6678461c34f4cc4d/samples/GenericHostSample/ServiceBaseLifetime.cs

	public class ServiceBaseLifetime : ServiceBase, IHostLifetime
	{
		private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

		public ServiceBaseLifetime()
		{
		}

		public Task WaitForStartAsync(CancellationToken cancellationToken)
		{
			cancellationToken.Register(() => _delayStart.TrySetCanceled());
			new Thread(Run).Start(); // Otherwise this would block and prevent IHost.StartAsync from finishing.
			return _delayStart.Task;
		}

		private void Run()
		{
			try
			{
				Run(this); // This blocks until the service is stopped.
				_delayStart.TrySetException(new InvalidOperationException("Stopped without starting"));
			}
			catch (Exception ex)
			{
				_delayStart.TrySetException(ex);
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			Stop();
			return Task.CompletedTask;
		}

		// Called by base.Run when the service is ready to start.
		protected override void OnStart(string[] args)
		{
			_delayStart.TrySetResult(null);
			base.OnStart(args);
		}
	}
}

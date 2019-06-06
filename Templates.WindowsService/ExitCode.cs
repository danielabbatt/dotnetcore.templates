using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Templates.WindowsService
{
	internal enum ExitCode
	{
		UnexpectedException = -1,
		Ok = 0,
		ConfigurationException = 1,
		RunCancelled = 3
	}
}

using Serilog.Events;
using Serilog.Sinks.EventLog;

namespace Templates.WindowsService
{
	public class SerilogEventLogIdProvider : IEventIdProvider
	{
		public ushort ComputeEventId(LogEvent logEvent) => 1;
	}
}

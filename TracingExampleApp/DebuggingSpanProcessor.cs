using System.Threading;
using System.Threading.Tasks;
using OpenTelemetry.Trace.Export;

namespace TracingExampleApp
{
    internal class DebuggingSpanProcessor : SpanProcessor
    {
        public override void OnEnd(SpanData span)
        {
        }

        public override void OnStart(SpanData span)
        {
        }

        public override Task ShutdownAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
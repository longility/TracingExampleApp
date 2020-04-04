using System;
using System.Collections.Generic;
using System.Threading;
using OpenTelemetry.Trace;
using OpenTelemetry.Trace.Configuration;

namespace TracingExampleApp
{
    internal class TestJaeger
    {
        internal static object Run(string host, int port)
        {
            // Create a tracer.
            using var tracerFactory = TracerFactory.Create(
                builder => builder.UseJaeger(o =>
                {
                    o.ServiceName = "jaeger-test-1";
                    o.AgentHost = host;
                    o.AgentPort = port;
                }));
            var tracer = tracerFactory.GetTracer("jaeger-test-2");

            Run(tracer);

            return null;
        }

        internal static void Run(Tracer tracer)
        {
            // Create a scoped span. It will end automatically when using statement ends
            using (tracer.StartActiveSpan("Main", out var span))
            {
                span.SetAttribute("custom-attribute", 55);
                Console.WriteLine("About to do a busy work");
                for (int i = 0; i < 10; i++)
                {
                    DoWork(i, tracer);
                }
            }
        }

        private static void DoWork(int i, Tracer tracer)
        {
            // Start another span. If another span was already started, it'll use that span as the parent span.
            // In this example, the main method already started a span, so that'll be the parent span, and this will be
            // a child span.
            using (tracer.StartActiveSpan("DoWork", out var span))
            {
                // Simulate some work.
                try
                {
                    Console.WriteLine("Doing busy work");
                    Thread.Sleep(1000);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // Set status upon error
                    span.Status = Status.Internal.WithDescription(e.ToString());
                }

                // Annotate our span to capture metadata about our operation
                var attributes = new Dictionary<string, object>();
                attributes.Add("use", "demo");
                attributes.Add("iteration", i);
                span.AddEvent(new Event("Invoking DoWork", attributes));
            }
        }
    }
}

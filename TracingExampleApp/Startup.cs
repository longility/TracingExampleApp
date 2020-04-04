using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace.Configuration;

namespace TracingExampleApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddOpenTelemetry(builder =>
            {
                builder
                .SetSampler(new OpenTelemetry.Trace.Samplers.AlwaysOnSampler())
                .UseJaeger(o =>
                {

                    o.ServiceName = "tracing-example-app";
                    o.AgentHost = "tracing";
                    o.AgentPort = 6831;
                })
                .SetResource(Resources.CreateServiceResource("tracing-example-app"))
                .AddProcessorPipeline(pipelineBuilder => pipelineBuilder.AddProcessor(_ => new DebuggingSpanProcessor()))
                .AddRequestCollector()
                .AddDependencyCollector();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

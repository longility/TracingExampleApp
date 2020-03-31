using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            TestJaeger.Run("tracing", 6831);

            services.AddOpenTelemetry(builder =>
            {
                builder
                .SetSampler(new OpenTelemetry.Trace.Samplers.AlwaysSampleSampler())
                .UseJaeger(o =>
                {

                    o.ServiceName = "tracing-example-app";
                    o.AgentHost = "tracing";
                    o.AgentPort = 6831;
                })
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

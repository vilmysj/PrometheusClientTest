using Microsoft.AspNetCore.Builder;
using Prometheus.Client.AspNetCore;

namespace PrometheusClientTest
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UsePrometheusServer(q =>
            {
                q.MapPath = "/metrics";
                q.UseDefaultCollectors = false;
            });
        }
    }
}
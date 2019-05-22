using System;
using Microsoft.Owin.Hosting;
using Owin;
using Prometheus.Client.Owin;

namespace PrometheusClientTest
{
    public class WebApi
    {
        public static IDisposable Start()
        {
            StartOptions startOptions = new StartOptions($"http://*:3000/");
            return WebApp.Start(startOptions, BuildApp);
        }

        private static void BuildApp(IAppBuilder builder)
        {
            builder.UsePrometheusServer(q =>
            {
                q.MapPath = "/metrics";
                q.UseDefaultCollectors = false;
            });
        }
    }
}
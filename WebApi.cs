using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Owin.Hosting;
using Owin;
using Prometheus.Client;
using Prometheus.Client.Owin;

namespace PrometheusClientTest
{
    public class WebApi
    {
        public static IDisposable Start()
        {
            CreateMetricForApplicationBuildInfo();
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

        private static void CreateMetricForApplicationBuildInfo()
        {
            Assembly executingAssembly = Assembly.GetEntryAssembly();

            string version = FileVersionInfo.GetVersionInfo(executingAssembly.Location).ProductVersion.TrimStart('v');
            string framework = executingAssembly
                .GetCustomAttributes(typeof(TargetFrameworkAttribute), false)
                .OfType<TargetFrameworkAttribute>().FirstOrDefault()?.FrameworkName.ToLowerInvariant();

            Gauge info = Metrics.CreateGauge(
                "application_build_info",
                "Application build info",
                "application", "product_version", "framework_name");

            info.WithLabels(executingAssembly.FullName, version, framework).Set(0);
        }
    }
}
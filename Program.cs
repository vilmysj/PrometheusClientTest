using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Prometheus.Client;

namespace PrometheusClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildWebHost(args).RunAsync();
            StartTest();
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://*:3000")
                .UseStartup<Startup>()
                .Build();

        private static void StartTest()
        {
            Console.WriteLine("Service started");

            CreateMultipleMetrics();
            Console.WriteLine("Metrics created");
            StartTestLoop();

            Console.ReadLine();
        }

        private static async void StartTestLoop()
        {
            HttpClient client = new HttpClient();

            bool keepRunning = true;
            int iteration = 1;

            while (keepRunning)
            {
                Console.WriteLine(iteration++);
                Task<string> resultTask1 = client.GetStringAsync("http://localhost:3000/metrics");
                Task<string> resultTask2 = client.GetStringAsync("http://localhost:3000/metrics");

                await Task.WhenAll(resultTask1, resultTask2).ConfigureAwait(false);

                if (resultTask1.Result != resultTask2.Result)
                {
                    string result1Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result1.txt");
                    File.WriteAllText(result1Path, resultTask1.Result);

                    string result2Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "result2.txt");
                    File.WriteAllText(result2Path, resultTask2.Result);

                    Console.WriteLine($"Results are stored in: {result1Path}, {result2Path}.");
                    keepRunning = false;
                }
            }
        }


        private static void CreateMultipleMetrics()
        {
            Metrics.CreateHistogram(
                    "database_client_method_duration_seconds",
                    "The duration of method processed by a database client.",
                    "database", "collection", "method")
                .WithLabels("testDB", "testCollection", "testMethod")
                .Observe(0.99);

            Metrics.CreateHistogram(
                    "http_client_request_duration_seconds",
                    "The duration of request processed by a http client.",
                    "method", "code")
                .WithLabels("testMethod", "testCode")
                .Observe(0.77);

            Metrics.CreateHistogram(
                    "http_server_request_duration_seconds",
                    "The duration of HTTP requests processed by an application.",
                    "method", "controller", "action", "code")
                .WithLabels("testMethod", "testController", "testAction", "testCode")
                .Observe(0.88);

            Metrics.CreateCounter(
                "http_client_request_error_total",
                "The number of failed HTTP requests",
                "method", "code")
                .WithLabels("testMethod", "testCode")
                .Inc();
        }
    }
}

using System;

namespace PrometheusClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            StartWebService();
        }

        private static void StartWebService()
        {
            using (WebApi.Start())
            {
                Console.WriteLine("Service started");
                Console.ReadLine();
            }
        }
    }
}

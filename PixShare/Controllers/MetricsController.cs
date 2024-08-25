using Prometheus;
using System.Web.Mvc;
using System.IO;
using System;
using System.Threading.Tasks;

namespace PixShare.Controllers
{
    public class MetricsController : Controller
    {
        // Define custom metrics
        private static readonly Counter AppRequestsTotal = Prometheus.Metrics.CreateCounter(
            "app_requests_total", "Total number of requests received");

        private static readonly Gauge AppActiveRequests = Prometheus.Metrics.CreateGauge(
            "app_active_requests", "Number of active requests");

        private static readonly Histogram AppRequestDuration = Prometheus.Metrics.CreateHistogram(
            "app_request_duration_seconds", "Histogram of request duration");

        public async Task<ActionResult> Index()
        {
            // Increment the request counter
            AppRequestsTotal.Inc();

            // Simulate active requests
            AppActiveRequests.Set(1); // Always ensure there's at least 1 active request

            // Simulate work and record duration
            using (AppRequestDuration.NewTimer())
            {
                await Task.Delay(100); // Simulate some processing delay
            }

            // Decrement the active requests (if simulating a real environment, this would match your actual request handling)
            AppActiveRequests.Dec();

            // Set the content type to plain text for Prometheus to scrape the metrics
            Response.ContentType = "text/plain";

            // Export the collected metrics
            using (var writer = new StreamWriter(Response.OutputStream))
            {
                await Prometheus.Metrics.DefaultRegistry.CollectAndExportAsTextAsync(writer.BaseStream);
            }

            return new EmptyResult();
        }

        public ActionResult Test()
        {
            return Content("Metrics Controller is working");
        }
    }
}

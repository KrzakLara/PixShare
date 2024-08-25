using Prometheus;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Owin;

public class MetricsMiddleware : OwinMiddleware
{
    private static readonly Counter RequestCounter = Metrics.CreateCounter("http_requests_total", "Total number of HTTP requests");
    private static readonly Histogram RequestDuration = Metrics.CreateHistogram("http_request_duration_seconds", "Duration of HTTP requests in seconds");

    public MetricsMiddleware(OwinMiddleware next) : base(next)
    {
    }

    public async override Task Invoke(IOwinContext context)
    {
        // Increment the request counter
        RequestCounter.Inc();

        // Start a stopwatch to measure the duration
        var stopwatch = Stopwatch.StartNew();

        // Proceed with the next middleware in the pipeline
        await Next.Invoke(context);

        // Stop the stopwatch and observe the duration
        stopwatch.Stop();
        RequestDuration.Observe(stopwatch.Elapsed.TotalSeconds);
    }
}

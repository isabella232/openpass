using App.Metrics;
using App.Metrics.Apdex;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.Timer;

namespace OpenPass.IdController.Models.Metric
{
    public static class MetricsRegistry
    {
        public static CounterOptions CreateCounterOptions(string metricName) => new CounterOptions
        {
            Name = metricName,
            MeasurementUnit = Unit.Calls
        };

        public static MeterOptions CreateMeterOptions(string metricName) => new MeterOptions
        {
            Name = metricName,
            MeasurementUnit = Unit.Calls
        };

        public static GaugeOptions CreateGaugeOptions(string metricName) => new GaugeOptions
        {
            Name = metricName,
            MeasurementUnit = Unit.Bytes
        };

        public static TimerOptions CreateTimerOptions(string metricName) => new TimerOptions
        {
            Name = metricName,
            MeasurementUnit = Unit.Requests,
            DurationUnit = TimeUnit.Milliseconds,
            RateUnit = TimeUnit.Milliseconds
        };

        public static HistogramOptions CreateHistogramOptions(string metricName) => new HistogramOptions
        {
            Name = metricName,
            MeasurementUnit = Unit.Bytes
        };

        public static ApdexOptions CreateApdexOptions(string metricName) => new ApdexOptions
        {
            Name = metricName,
            ApdexTSeconds = 0.5 // Adjust based on your SLA
        };
    }
}

using System;
using App.Metrics;
using OpenPass.IdController.Models.Metric;

namespace OpenPass.IdController.Helpers
{
    public interface IMetricHelper
    {
        void SendCounterMetric(string metricName);

        void SendMeterMetric(string metricName);

        void SendGaugeMetric(string metricName, long value);

        void SendTimerMetric(string metricName, Action actionToTrack);

        void SendHistogramMetric(string metricName, long value);

        void SendApdexMetric(string metricName, Action actionToTrack);
    }

    public class MetricHelper : IMetricHelper
    {
        private readonly IMetrics _metrics;

        public MetricHelper(IMetrics metrics)
        {
            _metrics = metrics;
        }

        public void SendCounterMetric(string metricName)
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.CreateCounterOptions(metricName));
        }

        public void SendMeterMetric(string metricName)
        {
            _metrics.Measure.Meter.Mark(MetricsRegistry.CreateMeterOptions(metricName));
        }

        public void SendGaugeMetric(string metricName, long value)
        {
            _metrics.Measure.Gauge.SetValue(MetricsRegistry.CreateGaugeOptions(metricName), value);
        }

        public void SendTimerMetric(string metricName, Action actionToTrack)
        {
            using (_metrics.Measure.Timer.Time(MetricsRegistry.CreateTimerOptions(metricName)))
            {
                actionToTrack();
            }
        }

        public void SendHistogramMetric(string metricName, long value)
        {
            _metrics.Measure.Histogram.Update(MetricsRegistry.CreateHistogramOptions(metricName), value);
        }

        public void SendApdexMetric(string metricName, Action actionToTrack)
        {
            using (_metrics.Measure.Apdex.Track(MetricsRegistry.CreateApdexOptions(metricName)))
            {
                actionToTrack();
            }
        }
    }
}

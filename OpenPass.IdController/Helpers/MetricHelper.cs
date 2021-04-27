using Metrics;

namespace OpenPass.IdController.Helpers
{
    public interface IMetricHelper
    {
        void SendCounterMetric(string metric);
    }

    public class MetricHelper : IMetricHelper
    {
        private readonly IMetricsRegistry _metricsRegistry;

        public MetricHelper(IMetricsRegistry metricRegistry)
        {
            _metricsRegistry = metricRegistry;
        }

        public void SendCounterMetric(string metric)
        {
            _metricsRegistry.GetOrRegister(metric, () => new Counter(Granularity.CoarseGrain)).Increment();
        }
    }
}

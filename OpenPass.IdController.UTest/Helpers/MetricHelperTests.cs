using App.Metrics;
using App.Metrics.Apdex;
using App.Metrics.Counter;
using App.Metrics.Gauge;
using App.Metrics.Histogram;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Moq;
using NUnit.Framework;
using OpenPass.IdController.Helpers;
using OpenPass.IdController.Models.Metric;

namespace OpenPass.IdController.UTest.Helpers
{
    [TestFixture]
    public class MetricHelperTests
    {
        private readonly Mock<IMetrics> _metricsMock;
        private readonly MetricHelper _metricHelper;

        public MetricHelperTests()
        {
            _metricsMock = new Mock<IMetrics>();
            _metricHelper = new MetricHelper(_metricsMock.Object);
        }

        [Test]
        public void SendCounterMetric_IncrementMetricTwice_ShouldCallCounterMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            var counterOptions = MetricsRegistry.CreateCounterOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Counter.Increment(counterOptions)).Verifiable();

            // Act
            _metricHelper.SendCounterMetric(metricName);
            _metricHelper.SendCounterMetric(metricName);

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Counter.Increment(It.Is<CounterOptions>(c => c.Name.Equals(metricName))), Times.Exactly(2));
        }

        [Test]
        public void SendMeterMetric_MarkMetricTwice_ShouldCallMeterMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            var meterOptions = MetricsRegistry.CreateMeterOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Meter.Mark(meterOptions)).Verifiable();

            // Act
            _metricHelper.SendMeterMetric(metricName);
            _metricHelper.SendMeterMetric(metricName);

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Meter.Mark(It.Is<MeterOptions>(c => c.Name.Equals(metricName))), Times.Exactly(2));
        }

        [Test]
        public void SendHistogramMetric_UpdateMetricTwice_ShouldCallHistogramMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            var histogramOptions = MetricsRegistry.CreateHistogramOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Histogram.Update(histogramOptions, It.IsAny<long>())).Verifiable();

            // Act
            _metricHelper.SendHistogramMetric(metricName, It.IsAny<long>());
            _metricHelper.SendHistogramMetric(metricName, It.IsAny<long>());

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Histogram.Update(It.Is<HistogramOptions>(c => c.Name.Equals(metricName)), It.IsAny<long>()), Times.Exactly(2));
        }

        [Test]
        public void SendApdexMetric_TrackMetricTwice_ShouldCallApdexMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            var apdexOptions = MetricsRegistry.CreateApdexOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Apdex.Track(apdexOptions)).Verifiable();

            var counter = 1;
            void ActionToPerform()
            { counter++; }

            // Act
            _metricHelper.SendApdexMetric(metricName, ActionToPerform);
            _metricHelper.SendApdexMetric(metricName, ActionToPerform);

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Apdex.Track(It.Is<ApdexOptions>(c => c.Name.Equals(metricName))), Times.Exactly(2));

            Assert.AreEqual(3, counter);
        }

        [Test]
        public void SendTimerMetric_TrackMetricTwice_ShouldCallTimerMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            var timerOptions = MetricsRegistry.CreateTimerOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Timer.Time(timerOptions)).Verifiable();

            var counter = 1;
            void ActionToPerform()
            { counter++; }

            // Act
            _metricHelper.SendTimerMetric(metricName, ActionToPerform);
            _metricHelper.SendTimerMetric(metricName, ActionToPerform);

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Timer.Time(It.Is<TimerOptions>(c => c.Name.Equals(metricName))), Times.Exactly(2));

            Assert.AreEqual(3, counter);
        }

        [Test]
        public void SendGaugeMetric_TrackMetricTwice_ShouldCallTimerMetricTwoTimes()
        {
            // Arrange
            var metricName = "test1";
            long workingSet64 = 12;
            var gaugeOptions = MetricsRegistry.CreateGaugeOptions(metricName);

            _metricsMock.Setup(x => x.Measure.Gauge.SetValue(gaugeOptions, workingSet64)).Verifiable();

            // Act
            _metricHelper.SendGaugeMetric(metricName, workingSet64);
            _metricHelper.SendGaugeMetric(metricName, workingSet64);

            // Assert
            _metricsMock
                .Verify(x => x.Measure.Gauge.SetValue(It.Is<GaugeOptions>(c => c.Name.Equals(metricName)), workingSet64), Times.Exactly(2));
        }
    }
}

namespace Splat.ApplicationInsights
{
    using System;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    public class ApplicationInsightsFeatureUsageTracking : IFeatureUsageTrackingSession<Guid>
    {
        private readonly TelemetryClient _telemetry;

        public ApplicationInsightsFeatureUsageTracking(string featureName) : this(featureName, Guid.Empty)
        {
        }

        internal ApplicationInsightsFeatureUsageTracking(string featureName, Guid parentReference)
        {
            this.FeatureName = featureName;
            this.FeatureReference = Guid.NewGuid();
            this.ParentReference = parentReference;
            this._telemetry = Locator.Current.GetService<TelemetryClient>();

            TrackEvent("Feature Usage Start");
        }

        public Guid FeatureReference { get; }

        public Guid ParentReference { get; }

        public string FeatureName { get; }

        public void Dispose()
        {
            TrackEvent("Feature Usage End");
        }

        public IFeatureUsageTrackingSession SubFeature(string description)
        {
            return new ApplicationInsightsFeatureUsageTracking(description, this.ParentReference);
        }

        public void OnException(Exception exception)
        {
            var telemetry = new ExceptionTelemetry(exception);
            PrepareEventData(telemetry);

            this._telemetry.TrackException(telemetry);
        }

        private void TrackEvent(string eventName)
        {
            var eventTelemetry = new EventTelemetry(eventName);
            PrepareEventData(eventTelemetry);

            this._telemetry.TrackEvent(eventTelemetry);
        }

        private void PrepareEventData<TTelemetry>(TTelemetry eventTelemetry) where TTelemetry : ISupportProperties
        {
            eventTelemetry.Properties.Add("Name", FeatureName);
            eventTelemetry.Properties.Add("Reference", FeatureReference.ToString());

            if (ParentReference != Guid.Empty)
            {
                eventTelemetry.Properties.Add("ParentReference", ParentReference.ToString());
            }
        }
    }
}
using UnityEngine;
using GameDevToi.ThirdLib.Core;
// Uncomment khi có Firebase SDK
// using Firebase.Analytics;

namespace GameDevToi.ThirdLib.Extensions
{
    /// <summary>
    /// Example implementation: Log ad events to Firebase Analytics
    /// Dev có thể tạo file này bên ngoài DLL để customize logging
    /// </summary>
    public class FirebaseAnalyticsLogger : MonoBehaviour
    {
        [Header("Event Name Configuration")]
        [SerializeField] private bool useCustomEventNames = false;
        [SerializeField] private string eventPrefix = "ad_";

        [Header("Logging Options")]
        [SerializeField] private bool logToConsole = true;
        [SerializeField] private bool logToFirebase = true;
        [SerializeField] private bool logRevenueEvents = true;

        private void OnEnable()
        {
            // Subscribe to ad events
            AdEvents.OnAdEvent += HandleAdEvent;

            // Hoặc subscribe vào từng event cụ thể
            // AdEvents.OnAdLoadSuccess += HandleAdLoadSuccess;
            // AdEvents.OnAdLoadFailed += HandleAdLoadFailed;
            // AdEvents.OnAdImpression += HandleAdImpression;
            // AdEvents.OnAdPaid += HandleAdPaid;
        }

        private void OnDisable()
        {
            // Unsubscribe
            AdEvents.OnAdEvent -= HandleAdEvent;
        }

        private void HandleAdEvent(AdEventArgs eventArgs)
        {
            if (logToConsole)
            {
                Debug.Log($"[FirebaseLogger] {eventArgs}");
            }

            if (!logToFirebase)
                return;

            // Skip revenue events if disabled
            if (!logRevenueEvents && eventArgs.EventType == AdEventType.AdPaid)
                return;

            // Get event name
            string eventName = GetEventName(eventArgs.EventType);

            // Convert to Firebase parameters
            var parameters = ConvertToFirebaseParameters(eventArgs);

            // Log to Firebase
            LogToFirebase(eventName, parameters);
        }

        private string GetEventName(AdEventType eventType)
        {
            if (useCustomEventNames)
            {
                return eventPrefix + eventType.ToString().ToLower();
            }

            // Mapping to standard Firebase event names
            switch (eventType)
            {
                case AdEventType.AdLoadStarted:
                    return "ad_load_started";
                case AdEventType.AdLoadSuccess:
                    return "ad_load_success";
                case AdEventType.AdLoadFailed:
                    return "ad_load_failed";
                case AdEventType.AdShown:
                    return "ad_show";
                case AdEventType.AdClicked:
                    return "ad_click";
                case AdEventType.AdClosed:
                    return "ad_close";
                case AdEventType.AdImpression:
                    return Firebase.Analytics.FirebaseAnalytics.EventAdImpression; // Standard Firebase event
                case AdEventType.AdPaid:
                    return "ad_revenue"; // Custom revenue tracking
                case AdEventType.AdRewarded:
                    return "ad_rewarded";
                default:
                    return "ad_event";
            }
        }

        private System.Collections.Generic.Dictionary<string, object> ConvertToFirebaseParameters(AdEventArgs eventArgs)
        {
            var parameters = new System.Collections.Generic.Dictionary<string, object>
            {
                { "ad_format", eventArgs.FormatId },
                { "ad_network", eventArgs.NetworkId },
                { "ad_unit_id", eventArgs.AdUnitId }
            };

            if (!string.IsNullOrEmpty(eventArgs.PlacementId))
                parameters["placement_id"] = eventArgs.PlacementId;

            if (!string.IsNullOrEmpty(eventArgs.ErrorMessage))
            {
                parameters["error_message"] = eventArgs.ErrorMessage;
                parameters["error_code"] = eventArgs.ErrorCode;
            }

            if (eventArgs.Revenue > 0)
            {
                parameters["value"] = eventArgs.Revenue; // Firebase standard parameter
                parameters["currency"] = eventArgs.CurrencyCode;
                parameters["revenue"] = eventArgs.Revenue;
            }

            if (!string.IsNullOrEmpty(eventArgs.RewardType))
            {
                parameters["reward_type"] = eventArgs.RewardType;
                parameters["reward_amount"] = eventArgs.RewardAmount;
            }

            // Add timestamp
            parameters["timestamp"] = eventArgs.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

            return parameters;
        }

        private void LogToFirebase(string eventName, System.Collections.Generic.Dictionary<string, object> parameters)
        {
            // Uncomment khi có Firebase SDK
            /*
            var firebaseParams = new Firebase.Analytics.Parameter[parameters.Count];
            int index = 0;
            foreach (var kvp in parameters)
            {
                if (kvp.Value is string strValue)
                    firebaseParams[index] = new Firebase.Analytics.Parameter(kvp.Key, strValue);
                else if (kvp.Value is int intValue)
                    firebaseParams[index] = new Firebase.Analytics.Parameter(kvp.Key, intValue);
                else if (kvp.Value is long longValue)
                    firebaseParams[index] = new Firebase.Analytics.Parameter(kvp.Key, longValue);
                else if (kvp.Value is double doubleValue)
                    firebaseParams[index] = new Firebase.Analytics.Parameter(kvp.Key, doubleValue);
                else
                    firebaseParams[index] = new Firebase.Analytics.Parameter(kvp.Key, kvp.Value.ToString());
                
                index++;
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            */

            // Temporary: Just log to console
            Debug.Log($"[Firebase] Event: {eventName}, Params: {string.Join(", ", parameters)}");
        }

        /// <summary>
        /// Example: Handle specific events with custom logic
        /// </summary>
        private void HandleAdLoadSuccess(AdEventArgs eventArgs)
        {
            // Custom logic for load success
            Debug.Log($"Ad loaded successfully: {eventArgs.FormatId} from {eventArgs.NetworkId}");
        }

        private void HandleAdLoadFailed(AdEventArgs eventArgs)
        {
            // Custom logic for load failed
            Debug.LogWarning($"Ad failed to load: {eventArgs.ErrorMessage}");
        }

        private void HandleAdImpression(AdEventArgs eventArgs)
        {
            // Custom logic for impression
            Debug.Log($"Ad impression recorded: {eventArgs.FormatId}");
        }

        private void HandleAdPaid(AdEventArgs eventArgs)
        {
            // Custom logic for revenue
            Debug.Log($"Ad revenue: {eventArgs.Revenue} {eventArgs.CurrencyCode}");

            // Example: Send to additional analytics platforms
            // Adjust.trackAdRevenue(...)
            // AppsFlyer.trackAdRevenue(...)
        }
    }
}

using UnityEngine;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.Examples
{
    /// <summary>
    /// Example: How to use AdEvents system
    /// Dev có thể copy file này và customize theo nhu cầu mà không cần sửa code trong DLL
    /// </summary>
    public class AdEventsExample : MonoBehaviour
    {
        private void OnEnable()
        {
            // Cách 1: Subscribe vào tất cả events
            AdEvents.OnAdEvent += HandleAllAdEvents;

            // Cách 2: Subscribe vào từng event cụ thể
            AdEvents.OnAdLoadSuccess += HandleAdLoadSuccess;
            AdEvents.OnAdLoadFailed += HandleAdLoadFailed;
            AdEvents.OnAdImpression += HandleAdImpression;
            AdEvents.OnAdPaid += HandleAdPaid;
            AdEvents.OnAdRewarded += HandleAdRewarded;
            AdEvents.OnAdClicked += HandleAdClicked;
            AdEvents.OnAdShown += HandleAdShown;
            AdEvents.OnAdClosed += HandleAdClosed;
        }

        private void OnDisable()
        {
            // Unsubscribe khi disable
            AdEvents.OnAdEvent -= HandleAllAdEvents;
            AdEvents.OnAdLoadSuccess -= HandleAdLoadSuccess;
            AdEvents.OnAdLoadFailed -= HandleAdLoadFailed;
            AdEvents.OnAdImpression -= HandleAdImpression;
            AdEvents.OnAdPaid -= HandleAdPaid;
            AdEvents.OnAdRewarded -= HandleAdRewarded;
            AdEvents.OnAdClicked -= HandleAdClicked;
            AdEvents.OnAdShown -= HandleAdShown;
            AdEvents.OnAdClosed -= HandleAdClosed;
        }

        /// <summary>
        /// Handle tất cả ad events
        /// </summary>
        private void HandleAllAdEvents(AdEventArgs eventArgs)
        {
            Debug.Log($"[AdEventsExample] {eventArgs.EventType}: {eventArgs.FormatId} from {eventArgs.NetworkId}");
        }

        /// <summary>
        /// Handle khi ad load thành công
        /// </summary>
        private void HandleAdLoadSuccess(AdEventArgs eventArgs)
        {
            Debug.Log($"✓ Ad loaded: {eventArgs.FormatId} from {eventArgs.NetworkId}");

            // Example: Track với analytics
            // Analytics.LogEvent("ad_loaded", new Dictionary<string, object>
            // {
            //     { "format", eventArgs.FormatId },
            //     { "network", eventArgs.NetworkId }
            // });
        }

        /// <summary>
        /// Handle khi ad load thất bại
        /// </summary>
        private void HandleAdLoadFailed(AdEventArgs eventArgs)
        {
            Debug.LogWarning($"✗ Ad load failed: {eventArgs.FormatId} - {eventArgs.ErrorMessage}");

            // Example: Track error với analytics
            // Analytics.LogEvent("ad_load_failed", new Dictionary<string, object>
            // {
            //     { "format", eventArgs.FormatId },
            //     { "network", eventArgs.NetworkId },
            //     { "error", eventArgs.ErrorMessage },
            //     { "error_code", eventArgs.ErrorCode }
            // });
        }

        /// <summary>
        /// Handle khi ad impression được ghi nhận
        /// </summary>
        private void HandleAdImpression(AdEventArgs eventArgs)
        {
            Debug.Log($"👁 Ad impression: {eventArgs.FormatId} from {eventArgs.NetworkId}");

            // Example: Log to Firebase Analytics
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAdImpression, 
            //     new Parameter("ad_format", eventArgs.FormatId),
            //     new Parameter("ad_network", eventArgs.NetworkId));
        }

        /// <summary>
        /// Handle khi ad tạo ra revenue (eCPM)
        /// </summary>
        private void HandleAdPaid(AdEventArgs eventArgs)
        {
            Debug.Log($"💰 Ad revenue: {eventArgs.Revenue} {eventArgs.CurrencyCode} " +
                     $"from {eventArgs.FormatId}/{eventArgs.NetworkId}");

            // Example: Log revenue to multiple platforms
            // Firebase
            // FirebaseAnalytics.LogEvent("ad_revenue",
            //     new Parameter("value", eventArgs.Revenue),
            //     new Parameter("currency", eventArgs.CurrencyCode),
            //     new Parameter("ad_format", eventArgs.FormatId),
            //     new Parameter("ad_network", eventArgs.NetworkId));

            // Adjust
            // AdjustAdRevenue adjustAdRevenue = new AdjustAdRevenue(AdjustConfig.AdjustAdRevenueSourceAdMob);
            // adjustAdRevenue.setRevenue(eventArgs.Revenue, eventArgs.CurrencyCode);
            // Adjust.trackAdRevenue(adjustAdRevenue);

            // AppsFlyer
            // Dictionary<string, string> eventValues = new Dictionary<string, string>
            // {
            //     { AFInAppEvents.REVENUE, eventArgs.Revenue.ToString() },
            //     { AFInAppEvents.CURRENCY, eventArgs.CurrencyCode },
            //     { "ad_format", eventArgs.FormatId },
            //     { "ad_network", eventArgs.NetworkId }
            // };
            // AppsFlyer.sendEvent("ad_revenue", eventValues);
        }

        /// <summary>
        /// Handle khi user nhận được reward
        /// </summary>
        private void HandleAdRewarded(AdEventArgs eventArgs)
        {
            Debug.Log($"🎁 Reward granted: {eventArgs.RewardType} x{eventArgs.RewardAmount}");

            // Example: Ghi nhận reward cho user
            // PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + (int)eventArgs.RewardAmount);

            // Example: Show UI notification
            // UIManager.Instance.ShowRewardNotification(eventArgs.RewardType, eventArgs.RewardAmount);

            // Example: Log to analytics
            // Analytics.LogEvent("ad_reward_earned",
            //     new Dictionary<string, object>
            //     {
            //         { "reward_type", eventArgs.RewardType },
            //         { "reward_amount", eventArgs.RewardAmount },
            //         { "ad_network", eventArgs.NetworkId }
            //     });
        }

        /// <summary>
        /// Handle khi user click vào ad
        /// </summary>
        private void HandleAdClicked(AdEventArgs eventArgs)
        {
            Debug.Log($"👆 Ad clicked: {eventArgs.FormatId}");

            // Example: Pause game khi user click ad
            // if (eventArgs.FormatId != "banner")
            // {
            //     Time.timeScale = 0;
            // }
        }

        /// <summary>
        /// Handle khi ad được hiển thị
        /// </summary>
        private void HandleAdShown(AdEventArgs eventArgs)
        {
            Debug.Log($"📺 Ad shown: {eventArgs.FormatId}");

            // Example: Mute game audio khi show interstitial/rewarded
            // if (eventArgs.FormatId == "interstitial" || eventArgs.FormatId == "rewarded")
            // {
            //     AudioListener.volume = 0;
            // }
        }

        /// <summary>
        /// Handle khi ad bị đóng
        /// </summary>
        private void HandleAdClosed(AdEventArgs eventArgs)
        {
            Debug.Log($"❌ Ad closed: {eventArgs.FormatId}");

            // Example: Resume game khi ad đóng
            // Time.timeScale = 1;
            // AudioListener.volume = 1;

            // Example: Continue game flow
            // if (eventArgs.FormatId == "interstitial")
            // {
            //     GameManager.Instance.ContinueGame();
            // }
        }
    }
}

using System;
using UnityEngine;

namespace GameDevToi.ThirdLib.Core
{
    /// <summary>
    /// Enum định nghĩa các loại sự kiện quảng cáo
    /// </summary>
    public enum AdEventType
    {
        AdLoadStarted,      // Bắt đầu load ad
        AdLoadSuccess,      // Load ad thành công
        AdLoadFailed,       // Load ad thất bại
        AdShown,           // Ad được hiển thị
        AdClicked,         // User click vào ad
        AdClosed,          // Ad bị đóng
        AdImpression,      // Ad impression được ghi nhận
        AdPaid,            // Ad tạo ra revenue (eCPM)
        AdRewarded         // User nhận được reward (rewarded ad)
    }

    /// <summary>
    /// Class chứa thông tin chi tiết của ad event
    /// </summary>
    public class AdEventArgs
    {
        /// <summary>
        /// Loại sự kiện
        /// </summary>
        public AdEventType EventType { get; set; }

        /// <summary>
        /// Format ID của ad (banner, interstitial, rewarded, etc.)
        /// </summary>
        public string FormatId { get; set; }

        /// <summary>
        /// Network ID (admob, applovin, ironsource, etc.)
        /// </summary>
        public string NetworkId { get; set; }

        /// <summary>
        /// Ad Unit ID
        /// </summary>
        public string AdUnitId { get; set; }

        /// <summary>
        /// Placement ID (optional)
        /// </summary>
        public string PlacementId { get; set; }

        /// <summary>
        /// Thông báo lỗi (nếu có)
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Error code (nếu có)
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Revenue value (cho AdPaid event)
        /// </summary>
        public double Revenue { get; set; }

        /// <summary>
        /// Currency code (USD, VND, etc.)
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Reward type (cho rewarded ad)
        /// </summary>
        public string RewardType { get; set; }

        /// <summary>
        /// Reward amount (cho rewarded ad)
        /// </summary>
        public double RewardAmount { get; set; }

        /// <summary>
        /// Timestamp của sự kiện
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Additional metadata (optional)
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> Metadata { get; set; }

        public AdEventArgs()
        {
            Timestamp = DateTime.Now;
            Metadata = new System.Collections.Generic.Dictionary<string, object>();
        }

        /// <summary>
        /// Convert sang dictionary để dễ dàng log lên analytics
        /// </summary>
        public System.Collections.Generic.Dictionary<string, object> ToDictionary()
        {
            var dict = new System.Collections.Generic.Dictionary<string, object>
            {
                { "event_type", EventType.ToString() },
                { "format_id", FormatId },
                { "network_id", NetworkId },
                { "ad_unit_id", AdUnitId }
            };

            if (!string.IsNullOrEmpty(PlacementId))
                dict["placement_id"] = PlacementId;

            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                dict["error_message"] = ErrorMessage;
                dict["error_code"] = ErrorCode;
            }

            if (Revenue > 0)
            {
                dict["revenue"] = Revenue;
                dict["currency"] = CurrencyCode;
            }

            if (!string.IsNullOrEmpty(RewardType))
            {
                dict["reward_type"] = RewardType;
                dict["reward_amount"] = RewardAmount;
            }

            // Add metadata
            foreach (var kvp in Metadata)
            {
                dict[$"meta_{kvp.Key}"] = kvp.Value;
            }

            return dict;
        }

        public override string ToString()
        {
            return $"[AdEvent] {EventType} - {NetworkId}/{FormatId} - {AdUnitId}";
        }
    }

    /// <summary>
    /// Delegate cho ad events
    /// </summary>
    public delegate void AdEventHandler(AdEventArgs eventArgs);

    /// <summary>
    /// Static class quản lý tất cả ad events
    /// Dev có thể subscribe vào các events này từ bên ngoài DLL
    /// </summary>
    public static class AdEvents
    {
        /// <summary>
        /// Event được trigger khi bất kỳ ad event nào xảy ra
        /// Subscribe vào đây để handle tất cả events
        /// </summary>
        public static event AdEventHandler OnAdEvent;

        /// <summary>
        /// Event cho từng loại cụ thể
        /// </summary>
        public static event AdEventHandler OnAdLoadStarted;
        public static event AdEventHandler OnAdLoadSuccess;
        public static event AdEventHandler OnAdLoadFailed;
        public static event AdEventHandler OnAdShown;
        public static event AdEventHandler OnAdClicked;
        public static event AdEventHandler OnAdClosed;
        public static event AdEventHandler OnAdImpression;
        public static event AdEventHandler OnAdPaid;
        public static event AdEventHandler OnAdRewarded;

        /// <summary>
        /// Trigger một ad event
        /// </summary>
        public static void TriggerEvent(AdEventArgs eventArgs)
        {
            if (eventArgs == null)
            {
                Debug.LogWarning("[AdEvents] Cannot trigger null event");
                return;
            }

            // Log event
            Debug.Log($"[AdEvents] {eventArgs}");

            // Trigger general event
            OnAdEvent?.Invoke(eventArgs);

            // Trigger specific event based on type
            switch (eventArgs.EventType)
            {
                case AdEventType.AdLoadStarted:
                    OnAdLoadStarted?.Invoke(eventArgs);
                    break;
                case AdEventType.AdLoadSuccess:
                    OnAdLoadSuccess?.Invoke(eventArgs);
                    break;
                case AdEventType.AdLoadFailed:
                    OnAdLoadFailed?.Invoke(eventArgs);
                    break;
                case AdEventType.AdShown:
                    OnAdShown?.Invoke(eventArgs);
                    break;
                case AdEventType.AdClicked:
                    OnAdClicked?.Invoke(eventArgs);
                    break;
                case AdEventType.AdClosed:
                    OnAdClosed?.Invoke(eventArgs);
                    break;
                case AdEventType.AdImpression:
                    OnAdImpression?.Invoke(eventArgs);
                    break;
                case AdEventType.AdPaid:
                    OnAdPaid?.Invoke(eventArgs);
                    break;
                case AdEventType.AdRewarded:
                    OnAdRewarded?.Invoke(eventArgs);
                    break;
            }
        }

        /// <summary>
        /// Helper method để tạo và trigger event nhanh
        /// </summary>
        public static void Trigger(AdEventType eventType, string formatId, string networkId, string adUnitId, string placementId = null)
        {
            var eventArgs = new AdEventArgs
            {
                EventType = eventType,
                FormatId = formatId,
                NetworkId = networkId,
                AdUnitId = adUnitId,
                PlacementId = placementId
            };
            TriggerEvent(eventArgs);
        }

        /// <summary>
        /// Helper method cho failed event
        /// </summary>
        public static void TriggerFailed(string formatId, string networkId, string adUnitId, string errorMessage, int errorCode = 0)
        {
            var eventArgs = new AdEventArgs
            {
                EventType = AdEventType.AdLoadFailed,
                FormatId = formatId,
                NetworkId = networkId,
                AdUnitId = adUnitId,
                ErrorMessage = errorMessage,
                ErrorCode = errorCode
            };
            TriggerEvent(eventArgs);
        }

        /// <summary>
        /// Helper method cho paid event
        /// </summary>
        public static void TriggerPaid(string formatId, string networkId, string adUnitId, double revenue, string currency)
        {
            var eventArgs = new AdEventArgs
            {
                EventType = AdEventType.AdPaid,
                FormatId = formatId,
                NetworkId = networkId,
                AdUnitId = adUnitId,
                Revenue = revenue,
                CurrencyCode = currency
            };
            TriggerEvent(eventArgs);
        }

        /// <summary>
        /// Helper method cho rewarded event
        /// </summary>
        public static void TriggerRewarded(string formatId, string networkId, string adUnitId, string rewardType, double rewardAmount)
        {
            var eventArgs = new AdEventArgs
            {
                EventType = AdEventType.AdRewarded,
                FormatId = formatId,
                NetworkId = networkId,
                AdUnitId = adUnitId,
                RewardType = rewardType,
                RewardAmount = rewardAmount
            };
            TriggerEvent(eventArgs);
        }
    }
}

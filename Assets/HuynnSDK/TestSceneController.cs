using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.Example
{
    /// <summary>
    /// Complete test scene demonstrating AdBridge + FirebaseBridge integration
    /// </summary>
    public class TestSceneController : MonoBehaviour
    {
        private Vector2 scrollPosition;
        private string logText = "";
        private int maxLogLines = 20;

        private void OnEnable()
        {
            // Subscribe to ad events để show trong UI
            AdEvents.OnAdEvent += OnAdEvent;
        }

        private void OnDisable()
        {
            AdEvents.OnAdEvent -= OnAdEvent;
        }

        private void Start()
        {
            AddLog("=== Test Scene Started ===");
            AddLog($"AdBridge initialized: {AdBridge.Instance != null}");
            AddLog($"FirebaseBridge initialized: {FirebaseBridge.Instance != null}");
            AddLog($"Firebase ready: {FirebaseBridge.Instance?.IsInitialized}");
            AddLog("");
            AddLog("Click buttons below to test ad events");
            AddLog("All events will be automatically logged to Firebase!");
        }

        private void OnAdEvent(AdEventArgs e)
        {
            string emoji = GetEventEmoji(e.EventType);
            AddLog($"{emoji} {e.EventType}: {e.FormatId} from {e.NetworkId}");

            if (e.EventType == AdEventType.AdPaid)
            {
                AddLog($"   💰 Revenue: {e.Revenue} {e.CurrencyCode}");
            }
            else if (e.EventType == AdEventType.AdRewarded)
            {
                AddLog($"   🎁 Reward: {e.RewardType} x{e.RewardAmount}");
            }
            else if (e.EventType == AdEventType.AdLoadFailed)
            {
                AddLog($"   ❌ Error: {e.ErrorMessage}");
            }
        }

        private string GetEventEmoji(AdEventType type)
        {
            switch (type)
            {
                case AdEventType.AdLoadStarted: return "⏳";
                case AdEventType.AdLoadSuccess: return "✅";
                case AdEventType.AdLoadFailed: return "❌";
                case AdEventType.AdShown: return "📺";
                case AdEventType.AdClicked: return "👆";
                case AdEventType.AdClosed: return "🚪";
                case AdEventType.AdImpression: return "👁";
                case AdEventType.AdPaid: return "💰";
                case AdEventType.AdRewarded: return "🎁";
                default: return "📋";
            }
        }

        private void AddLog(string message)
        {
            logText = $"[{System.DateTime.Now:HH:mm:ss}] {message}\n" + logText;

            // Limit lines
            var lines = logText.Split('\n');
            if (lines.Length > maxLogLines)
            {
                logText = string.Join("\n", lines, 0, maxLogLines);
            }
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height - 20));

            // Title
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 24;
            titleStyle.fontStyle = FontStyle.Bold;
            GUILayout.Label("AdBridge + FirebaseBridge Test", titleStyle);
            GUILayout.Space(10);

            // Status
            GUILayout.Label($"AdBridge: {(AdBridge.Instance != null ? "✓ Ready" : "✗ Not Ready")}");
            GUILayout.Label($"FirebaseBridge: {(FirebaseBridge.Instance != null ? "✓ Ready" : "✗ Not Ready")}");
            GUILayout.Label($"Firebase Analytics: {(FirebaseBridge.Instance?.IsInitialized == true ? "✓ Initialized" : "⏳ Initializing...")}");
            GUILayout.Space(10);

            // Ad Controls
            GUILayout.Label("=== Ad Controls ===", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show Banner", GUILayout.Height(50)))
            {
                AdBridge.Instance?.ShowAd("banner");
                AddLog("▶ Showing banner ad...");
            }

            if (GUILayout.Button("Hide Banner", GUILayout.Height(50)))
            {
                AdBridge.Instance?.HideBanner();
                AddLog("▶ Hiding banner ad...");
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show Interstitial", GUILayout.Height(50)))
            {
                AdBridge.Instance?.ShowAd("interstitial");
                AddLog("▶ Showing interstitial ad...");
            }

            if (GUILayout.Button("Show Rewarded", GUILayout.Height(50)))
            {
                AdBridge.Instance?.ShowAd("rewarded");
                AddLog("▶ Showing rewarded ad...");
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show App Open", GUILayout.Height(50)))
            {
                AdBridge.Instance?.ShowAd("appopen");
                AddLog("▶ Showing app open ad...");
            }

            if (GUILayout.Button("Check Ad Ready", GUILayout.Height(50)))
            {
                bool bannerReady = AdBridge.Instance?.IsAdReady("banner") ?? false;
                bool interstitialReady = AdBridge.Instance?.IsAdReady("interstitial") ?? false;
                bool rewardedReady = AdBridge.Instance?.IsAdReady("rewarded") ?? false;

                AddLog($"Banner ready: {bannerReady}");
                AddLog($"Interstitial ready: {interstitialReady}");
                AddLog($"Rewarded ready: {rewardedReady}");
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Firebase Controls
            GUILayout.Label("=== Firebase Controls ===", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Log Custom Event", GUILayout.Height(40)))
            {
                FirebaseBridge.Instance?.LogCustomEvent("test_button_clicked", "button_name", "custom_event");
                AddLog("▶ Logged custom event to Firebase");
            }

            if (GUILayout.Button("Set User Property", GUILayout.Height(40)))
            {
                FirebaseBridge.Instance?.SetUserProperty("test_user", "level_5");
                AddLog("▶ Set user property on Firebase");
            }

            if (GUILayout.Button("Set User ID", GUILayout.Height(40)))
            {
                string userId = $"test_user_{Random.Range(1000, 9999)}";
                FirebaseBridge.Instance?.SetUserId(userId);
                AddLog($"▶ Set user ID: {userId}");
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            // Event Log
            GUILayout.Label("=== Event Log ===", EditorStyles.boldLabel);
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(200));
            GUILayout.Label(logText, EditorStyles.wordWrappedLabel);
            GUILayout.EndScrollView();

            GUILayout.Space(10);

            // Info
            GUIStyle infoStyle = new GUIStyle(GUI.skin.label);
            infoStyle.fontSize = 10;
            infoStyle.normal.textColor = Color.gray;
            GUILayout.Label("💡 All ad events are automatically logged to Firebase Analytics", infoStyle);
            GUILayout.Label("   Check Firebase Console → Analytics → Events to see ad_impression, ad_revenue, etc.", infoStyle);

            GUILayout.EndArea();
        }

        private static class EditorStyles
        {
            public static GUIStyle boldLabel
            {
                get
                {
                    var style = new GUIStyle(GUI.skin.label);
                    style.fontStyle = FontStyle.Bold;
                    style.fontSize = 14;
                    return style;
                }
            }

            public static GUIStyle wordWrappedLabel
            {
                get
                {
                    var style = new GUIStyle(GUI.skin.label);
                    style.wordWrap = true;
                    style.alignment = TextAnchor.UpperLeft;
                    return style;
                }
            }
        }
    }
}

using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;
using GameDevToi.ThirdLib.AdModule;

namespace GameDevToi.ThirdLib.Example
{
    /// <summary>
    /// Ví dụ sử dụng AdBridge với string-based IDs
    /// </summary>
    public class AdBridgeExample : MonoBehaviour
    {
        private void Start()
        {
            // AdBridge tự động khởi tạo khi game start
            // Bạn không cần làm gì cả!

            // Có thể lấy instance và kiểm tra status
            if (AdBridge.Instance != null)
            {
                Debug.Log("AdBridge is ready!");

                // In debug info
                Debug.Log(AdBridge.Instance.GetDebugInfo());
            }
        }

        // Ví dụ hiển thị Banner (sử dụng format ID)
        public void ShowBanner()
        {
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.ShowAd("banner");
            }
        }

        // Ví dụ hiển thị Interstitial
        public void ShowInterstitial()
        {
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.ShowAd("interstitial");
            }
        }

        // Ví dụ hiển thị Rewarded
        public void ShowRewarded()
        {
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.ShowAd("rewarded");
            }
        }

        // Ví dụ hiển thị App Open Ad
        public void ShowAppOpen()
        {
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.ShowAd("appopen");
            }
        }

        // Ví dụ kiểm tra ad có sẵn sàng không
        public void CheckAdReady()
        {
            if (AdBridge.Instance != null)
            {
                bool interstitialReady = AdBridge.Instance.IsAdReady("interstitial");
                Debug.Log($"Interstitial ready: {interstitialReady}");

                bool rewardedReady = AdBridge.Instance.IsAdReady("rewarded");
                Debug.Log($"Rewarded ready: {rewardedReady}");

                bool bannerReady = AdBridge.Instance.IsAdReady("banner");
                Debug.Log($"Banner ready: {bannerReady}");
            }
        }

        // Ví dụ kiểm tra ad của network cụ thể (sử dụng network ID)
        public void CheckAdMobInterstitial()
        {
            if (AdBridge.Instance != null)
            {
                bool ready = AdBridge.Instance.IsAdReady("interstitial", "admob");
                Debug.Log($"AdMob Interstitial ready: {ready}");
            }
        }

        // Ví dụ lấy module cụ thể để sử dụng
        public void UseSpecificModule()
        {
            if (AdBridge.Instance != null)
            {
                // Lấy AdMob module (sử dụng network ID)
                var adMobModule = AdBridge.Instance.GetModule("admob");
                if (adMobModule != null && adMobModule.IsInitialized)
                {
                    Debug.Log("AdMob module is initialized and ready");
                    // Có thể gọi methods cụ thể của module nếu cần
                    adMobModule.ShowAd("interstitial");
                }

                // Lấy AppLovin module
                var appLovinModule = AdBridge.Instance.GetModule("applovin");
                if (appLovinModule != null && appLovinModule.IsInitialized)
                {
                    Debug.Log("AppLovin module is initialized and ready");
                }

                // Lấy IronSource module
                var ironSourceModule = AdBridge.Instance.GetModule("ironsource");
                if (ironSourceModule != null && ironSourceModule.IsInitialized)
                {
                    Debug.Log("IronSource module is initialized and ready");
                }
            }
        }

        // Ví dụ reload ad units khi config thay đổi
        public void ReloadConfig()
        {
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.ReloadAdUnits();
                Debug.Log("Ad units reloaded");
            }
        }

        // Ví dụ sử dụng AdRegistry để list formats và networks
        public void ListAvailableFormatsAndNetworks()
        {
            Debug.Log("=== Available Ad Formats ===");
            var formats = AdRegistry.GetAllFormats();
            foreach (var format in formats)
            {
                Debug.Log($"- {format.displayName} (ID: {format.id})");
            }

            Debug.Log("\n=== Available Ad Networks ===");
            var networks = AdRegistry.GetAllNetworks();
            foreach (var network in networks)
            {
                Debug.Log($"- {network.displayName} (ID: {network.id})");
            }
        }

        // Test button trong Inspector
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 400));

            GUILayout.Label("AdBridge Test Panel", GUI.skin.box);

            if (GUILayout.Button("Show Banner", GUILayout.Height(50)))
            {
                ShowBanner();
            }

            if (GUILayout.Button("Show Interstitial", GUILayout.Height(50)))
            {
                ShowInterstitial();
            }

            if (GUILayout.Button("Show Rewarded", GUILayout.Height(50)))
            {
                ShowRewarded();
            }

            if (GUILayout.Button("Check Ad Ready", GUILayout.Height(50)))
            {
                CheckAdReady();
            }

            if (GUILayout.Button("Print Debug Info", GUILayout.Height(50)))
            {
                if (AdBridge.Instance != null)
                {
                    Debug.Log(AdBridge.Instance.GetDebugInfo());
                }
            }

            GUILayout.EndArea();
        }
    }
}

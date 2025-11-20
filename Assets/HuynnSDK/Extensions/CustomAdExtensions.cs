using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;
using GameDevToi.ThirdLib.AdModule;

namespace GameDevToi.ThirdLib.Extensions
{
    /// <summary>
    /// VÍ DỤ: Cách thêm Ad Network mới (Vungle) mà không cần rebuild DLL
    /// </summary>
    public class VungleModule : BaseAdNetworkModule
    {
        public override string NetworkId => "vungle"; // Unique ID

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);

            // TODO: Get Vungle App ID from config (có thể thêm field mới vào config)
            string appId = "your-vungle-app-id";

            LogInfo($"Vungle initializing with App ID: {appId}");

            // TODO: Initialize Vungle SDK
            // Vungle.init(appId);

            isInitialized = true;
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            if (!isInitialized)
            {
                LogError("Vungle not initialized");
                return;
            }

            string adUnitId = adUnit.GetAdUnitId();
            var formatDef = adUnit.GetFormat();
            string formatName = formatDef?.displayName ?? adUnit.formatId;

            LogInfo($"Loading {formatName} ad with ID: {adUnitId}");

            // TODO: Load ad với Vungle SDK
            // Vungle.loadAd(adUnitId);
        }

        public override void ShowAd(string formatId, string placementId = null)
        {
            if (!isInitialized)
            {
                LogError("Vungle not initialized");
                return;
            }

            var formatDef = AdRegistry.GetFormat(formatId);
            string formatName = formatDef?.displayName ?? formatId;
            LogInfo($"Showing {formatName} ad");

            // TODO: Show ad
            // Vungle.playAd(placementId);
        }

        public override bool IsAdReady(string formatId, string placementId = null)
        {
            if (!isInitialized) return false;

            // TODO: Check if ad is ready
            // return Vungle.isAdAvailable(placementId);
            return false;
        }
    }

    /// <summary>
    /// VÍ DỤ: Bootstrapper để đăng ký custom ad networks và formats
    /// File này không nằm trong DLL, developers có thể tự do chỉnh sửa
    /// </summary>
    public class CustomAdExtensions
    {
        /// <summary>
        /// Tự động đăng ký custom networks và formats khi game start
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterCustomExtensions()
        {
            Debug.Log("[CustomAdExtensions] Registering custom ad networks and formats...");

            // 1. Đăng ký custom ad network definition
            var vungleNetwork = new AdNetworkDefinition(
                id: "vungle",
                displayName: "Vungle",
                description: "Vungle ad network",
                color: new Color(0.9f, 0.3f, 0.5f)
            );
            AdRegistry.RegisterNetwork(vungleNetwork);

            // 2. Đăng ký custom ad format (ví dụ: Playable Ads)
            var playableFormat = new AdFormatDefinition(
                id: "playable",
                displayName: "Playable",
                description: "Interactive playable ads"
            );
            AdRegistry.RegisterFormat(playableFormat);

            // 3. Đăng ký module cho Vungle vào AdBridge
            // Cách 1: Đăng ký ngay (nếu AdBridge đã khởi tạo)
            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.RegisterAdModule(new VungleModule());
            }

            // Cách 2: Đăng ký sau khi AdBridge khởi tạo
            // (Khuyến nghị: dùng cách này nếu không chắc timing)
            RegisterModuleDelayed();

            Debug.Log("[CustomAdExtensions] Custom extensions registered successfully!");
        }

        private static async void RegisterModuleDelayed()
        {
            // Đợi AdBridge khởi tạo xong
            await System.Threading.Tasks.Task.Delay(100);

            if (AdBridge.Instance != null)
            {
                AdBridge.Instance.RegisterAdModule(new VungleModule());
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Đăng ký trong Editor mode
        /// </summary>
        [UnityEditor.InitializeOnLoadMethod]
        private static void RegisterInEditor()
        {
            // Đăng ký custom definitions cho Editor
            var vungleNetwork = new AdNetworkDefinition(
                id: "vungle",
                displayName: "Vungle",
                description: "Vungle ad network",
                color: new Color(0.9f, 0.3f, 0.5f)
            );
            AdRegistry.RegisterNetwork(vungleNetwork);

            var playableFormat = new AdFormatDefinition(
                id: "playable",
                displayName: "Playable",
                description: "Interactive playable ads"
            );
            AdRegistry.RegisterFormat(playableFormat);
        }
#endif
    }

    /// <summary>
    /// VÍ DỤ: Cách sử dụng custom ad network trong game
    /// </summary>
    public class CustomAdUsageExample : MonoBehaviour
    {
        private void Start()
        {
            // Sau khi đăng ký, có thể sử dụng như bình thường
            // với format ID và network ID là string
        }

        public void ShowVungleInterstitial()
        {
            // Cách 1: Show ad tự động (AdBridge chọn network theo priority)
            AdBridge.Instance.ShowAd("interstitial");

            // Cách 2: Sử dụng module cụ thể
            var vungleModule = AdBridge.Instance.GetModule("vungle");
            if (vungleModule != null && vungleModule.IsInitialized)
            {
                vungleModule.ShowAd("interstitial");
            }
        }

        public void ShowCustomPlayableAd()
        {
            // Show custom format "playable"
            AdBridge.Instance.ShowAd("playable");
        }

        public void CheckCustomAdReady()
        {
            // Check nếu Vungle interstitial ready
            bool ready = AdBridge.Instance.IsAdReady("interstitial", "vungle");
            Debug.Log($"Vungle Interstitial ready: {ready}");

            // Check nếu playable ad ready từ bất kỳ network nào
            bool playableReady = AdBridge.Instance.IsAdReady("playable");
            Debug.Log($"Playable ad ready: {playableReady}");
        }
    }
}

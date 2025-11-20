# 3rdLib - Unity Ad Management System

## 📋 Tổng quan

**3rdLib** là hệ thống quản lý quảng cáo modular cho Unity với kiến trúc DLL extensible, tích hợp Firebase Analytics tự động.

### Tính năng chính

✅ **Modular Ad Network System** - Hỗ trợ nhiều ad networks (AdMob, AppLovin, IronSource, Unity Ads)  
✅ **Dynamic Ad Format Registration** - String-based IDs, không dùng enum để dễ extend  
✅ **Waterfall Mediation** - Priority-based ad serving  
✅ **Auto-initialization** - Singleton pattern tự động khởi tạo khi game start  
✅ **Unity Editor Integration** - Visual config window với 2 tabs (SDK Config, Ad Units)  
✅ **Event-Driven Architecture** - 9 loại ad events (load, show, impression, revenue, etc.)  
✅ **Firebase Analytics Integration** - Tự động log ad events lên Firebase  
✅ **DLL-Compatible** - Core có thể build thành DLL, extensions ở ngoài  

---

## 🏗️ Kiến trúc hệ thống

### 1. Core Architecture

```
┌────────────────────────────────────────────────────────────┐
│                    Unity Editor Window                     │
│  ┌────────────────────────────────────────────────────┐    │
│  │  ThirdLibWindow (Menu: 3rdLib > Open Window)       │    │
│  │  • Tab 1: SDK Config (App Info + SDK Keys)         │    │
│  │  • Tab 2: Ad Units (CRUD management)               │    │
│  └────────────────────────────────────────────────────┘    │
│                           │                                │
│                           ▼                                │
│  ┌────────────────────────────────────────────────────┐    │
│  │  ThirdLibConfig (ScriptableObject)                 │    │
│  │  • Resources/ThirdLibConfig.asset                  │    │
│  │  • App info + SDK keys + Ad units list             │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌────────────────────────────────────────────────────────────┐
│                      Runtime System                        │
│  ┌────────────────────────────────────────────────────┐    │
│  │  AdRegistry (Dynamic Registration)                 │    │
│  │  • Built-in: banner, interstitial, rewarded, etc.  │    │
│  │  • Custom: RegisterFormat(), RegisterNetwork()     │    │
│  └────────────────────────────────────────────────────┘    │
│                           │                                │
│                           ▼                                │
│  ┌────────────────────────────────────────────────────┐    │
│  │  AdBridge (Singleton)                              │    │
│  │  • Dictionary<networkId, IAdNetworkModule>         │    │
│  │  • ShowAd(formatId), IsAdReady(), etc.             │    │
│  │  • Waterfall mediation logic                       │    │
│  └────────────────────────────────────────────────────┘    │
│                           │                                │
│                           ▼                                │
│  ┌────────────────────────────────────────────────────┐    │
│  │  Ad Network Modules (IAdNetworkModule)             │    │
│  │  • AdMobModule                                     │    │
│  │  • AppLovinModule                                  │    │
│  │  • IronSourceModule                                │    │
│  │  • UnityAdsModule                                  │    │
│  └────────────────────────────────────────────────────┘    │
│                           │                                │
│                           ▼                                │
│  ┌────────────────────────────────────────────────────┐    │
│  │  AdEvents (Event System)                           │    │
│  │  • 9 event types with AdEventArgs                  │    │
│  │  • Static events: OnAdImpression, OnAdPaid, etc.   │    │
│  └────────────────────────────────────────────────────┘    │
│                           │                                │
│                           ▼                                │
│  ┌────────────────────────────────────────────────────┐    │
│  │  FirebaseBridge (Singleton)                        │    │
│  │  • Auto-subscribe to AdEvents                      │    │
│  │  • Log to Firebase Analytics                       │    │
│  │  • Focus: ad_impression, ad_revenue                │    │
│  └────────────────────────────────────────────────────┘    │
└────────────────────────────────────────────────────────────┘
```

### 2. Module Flow

```
User Code
   │
   └─> AdBridge.Instance.ShowAd("interstitial")
          │
          └─> Get active ad units for "interstitial" (priority sorted)
                │
                └─> Loop through networks (waterfall)
                       │
                       ├─> Check IsAdReady("interstitial", "admob")
                       │      │
                       │      └─> AdMobModule.IsAdReady()
                       │             │
                       │             └─> Return true/false
                       │
                       ├─> If ready: AdMobModule.ShowAd("interstitial")
                       │      │
                       │      └─> Trigger AdEvents:
                       │             • AdEvents.Trigger(AdShown)
                       │             • AdEvents.Trigger(AdImpression) ⭐
                       │             • AdEvents.TriggerPaid(revenue, currency)
                       │             • AdEvents.Trigger(AdClosed)
                       │
                       └─> FirebaseBridge auto-handles events
                              │
                              └─> Log to Firebase Analytics:
                                    • ad_show
                                    • ad_impression ⭐
                                    • ad_revenue
                                    • ad_close
```

### 3. Data Flow

```
Editor Window
    │
    └─> Save to ThirdLibConfig.asset (Resources/)
           │
           └─> Runtime: ThirdLibConfig.Instance
                  │
                  ├─> AdBridge.Initialize()
                  │      │
                  │      └─> Register modules
                  │             │
                  │             ├─> AdMobModule
                  │             ├─> AppLovinModule
                  │             ├─> IronSourceModule
                  │             └─> UnityAdsModule
                  │
                  └─> GetActiveAdUnits(formatId)
                         │
                         └─> Return List<AdUnit> (priority sorted)
```

---

## 📦 Cấu trúc thư mục

```
Assets/HuynnSDK/
├── Core/
│   ├── AdDefinitions.cs          # Format & Network definitions
│   ├── AdRegistry.cs              # Dynamic registration system
│   ├── AdUnit.cs                  # Ad unit data model
│   └── AdEvents.cs                # Event system (9 events)
│
├── AdModule/
│   ├── IAdNetworkModule.cs        # Interface
│   ├── BaseAdNetworkModule.cs     # Abstract base
│   ├── AdMobModule.cs             # Google AdMob (full implementation)
│   ├── AppLovinModule.cs          # AppLovin MAX
│   ├── IronSourceModule.cs        # IronSource
│   └── UnityAdsModule.cs          # Unity Ads
│
├── AdBridge.cs                    # Main singleton manager
├── ThirdLibConfig.cs              # ScriptableObject config
├── FirebaseBridge.cs              # Firebase Analytics singleton
│
├── Editor/
│   ├── ThirdLibWindow.cs          # Unity Editor window
│   └── AdEditorHelper.cs          # Helper methods for UI
│
├── Extensions/
│   ├── CustomAdExtensions.cs      # Example: Vungle custom network
│   └── FirebaseAnalyticsLogger.cs # Alternative Firebase logger
│
└── Examples/
    ├── AdBridgeExample.cs         # Usage examples
    ├── ThirdLibConfigExample.cs   # Config access examples
    ├── AdEventsExample.cs         # Event handling examples
    ├── FirebaseBridgeExample.cs   # Firebase integration
    └── TestSceneController.cs     # Complete test scene
```

---

## 🚀 Hướng dẫn sử dụng

### 1. Setup cơ bản (Unity Editor)

#### Bước 1: Mở ThirdLib Window

```
Menu: 3rdLib > Open Window
```

#### Bước 2: Cấu hình SDK (Tab: SDK Config)

```
App Information:
├── Android Package Name: com.yourcompany.yourgame
├── App Version: 1.0.0
└── Version Code: 1

SDK Keys:
├── Google AdMob
│   ├── Android App ID: ca-app-pub-xxxxxxxx~xxxxxxxxxx
│   └── iOS App ID: ca-app-pub-xxxxxxxx~xxxxxxxxxx
│
├── Facebook
│   ├── App ID: xxxxxxxxxxxx
│   └── Client Token: xxxxxxxxxxxxxxxxxxxxxxxx
│
├── AppLovin SDK Key: xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
└── IronSource App Key: xxxxxxxxx
```

#### Bước 3: Tạo Ad Units (Tab: Ad Units)

```
Ad Units Management:
├── Add Ad Unit (+)
│   ├── Select Format: Interstitial
│   └── Select Network: AdMob
│
└── Configure:
    ├── Active: ✓
    ├── Priority: 1 (1 = highest)
    ├── Android Ad Unit ID: ca-app-pub-xxx/xxxxxxxxxx
    └── iOS Ad Unit ID: ca-app-pub-xxx/xxxxxxxxxx
```

### 2. Runtime Usage (Code)

#### Basic - Show Ads

```csharp
using UnityEngine;
using GameDevToi.ThirdLib;

public class GameManager : MonoBehaviour
{
    // Show banner
    public void ShowBanner()
    {
        AdBridge.Instance.ShowAd("banner");
    }

    // Hide banner
    public void HideBanner()
    {
        AdBridge.Instance.HideBanner();
    }

    // Show interstitial
    public void ShowInterstitial()
    {
        if (AdBridge.Instance.IsAdReady("interstitial"))
        {
            AdBridge.Instance.ShowAd("interstitial");
        }
    }

    // Show rewarded
    public void ShowRewarded()
    {
        AdBridge.Instance.ShowAd("rewarded");
    }

    // Show app open
    public void ShowAppOpen()
    {
        AdBridge.Instance.ShowAd("appopen");
    }
}
```

#### Advanced - Custom Logic

```csharp
using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;
using GameDevToi.ThirdLib.AdModule;

public class AdManager : MonoBehaviour
{
    void Start()
    {
        // Check if AdBridge is ready
        if (AdBridge.Instance != null)
        {
            Debug.Log("AdBridge initialized!");
            Debug.Log(AdBridge.Instance.GetDebugInfo());
        }
    }

    // Check ad ready status
    public bool IsInterstitialReady()
    {
        return AdBridge.Instance.IsAdReady("interstitial");
    }

    // Check specific network
    public bool IsAdMobInterstitialReady()
    {
        return AdBridge.Instance.IsAdReady("interstitial", "admob");
    }

    // Get specific module
    public void ConfigureAdMob()
    {
        var adMobModule = AdBridge.Instance.GetModule("admob");
        if (adMobModule != null)
        {
            Debug.Log($"AdMob initialized: {adMobModule.IsInitialized}");
        }
    }

    // Reload ad units (if config changed)
    public void ReloadAds()
    {
        AdBridge.Instance.ReloadAdUnits();
    }
}
```

### 3. Event Handling

#### Option 1: Automatic Firebase Logging (Zero Code)

```csharp
// FirebaseBridge tự động log tất cả ad events!
// Không cần code gì, chỉ cần show ads:

AdBridge.Instance.ShowAd("interstitial");
// --> FirebaseBridge tự động log:
//     • ad_show
//     • ad_impression ⭐
//     • ad_revenue (nếu có)
//     • ad_close
```

#### Option 2: Custom Event Handling

```csharp
using GameDevToi.ThirdLib.Core;

public class MyAdEventHandler : MonoBehaviour
{
    void OnEnable()
    {
        // Subscribe to events
        AdEvents.OnAdImpression += OnAdImpression;
        AdEvents.OnAdPaid += OnAdPaid;
        AdEvents.OnAdRewarded += OnAdRewarded;
        AdEvents.OnAdClosed += OnAdClosed;
    }

    void OnDisable()
    {
        // Unsubscribe
        AdEvents.OnAdImpression -= OnAdImpression;
        AdEvents.OnAdPaid -= OnAdPaid;
        AdEvents.OnAdRewarded -= OnAdRewarded;
        AdEvents.OnAdClosed -= OnAdClosed;
    }

    void OnAdImpression(AdEventArgs e)
    {
        Debug.Log($"👁 Impression: {e.FormatId} from {e.NetworkId}");
        // Track with your analytics
    }

    void OnAdPaid(AdEventArgs e)
    {
        Debug.Log($"💰 Revenue: {e.Revenue} {e.CurrencyCode}");
        // Send to Adjust, AppsFlyer, etc.
    }

    void OnAdRewarded(AdEventArgs e)
    {
        // Grant reward to player
        int coins = (int)e.RewardAmount;
        PlayerData.AddCoins(coins);
        UIManager.ShowNotification($"+{coins} coins!");
    }

    void OnAdClosed(AdEventArgs e)
    {
        // Resume game
        Time.timeScale = 1;
        AudioListener.volume = 1;
    }
}
```

### 4. Firebase Analytics Integration

```csharp
using GameDevToi.ThirdLib;

public class FirebaseSetup : MonoBehaviour
{
    void Start()
    {
        // FirebaseBridge tự động khởi tạo và log ad events
        // Optional: Set user properties
        
        FirebaseBridge.Instance.SetUserId(GetUserId());
        FirebaseBridge.Instance.SetUserProperty("user_level", "5");
        FirebaseBridge.Instance.SetUserProperty("vip_status", "premium");
    }

    // Log custom game events
    public void OnLevelComplete(int level, int score)
    {
        var parameters = new System.Collections.Generic.Dictionary<string, object>
        {
            { "level", level },
            { "score", score },
            { "time", Time.timeSinceLevelLoad }
        };
        
        FirebaseBridge.Instance.LogCustomEvent("level_complete", parameters);
    }
}
```

---

## 🎯 Tính năng chi tiết

### 1. Ad Formats (Built-in)

| Format ID | Display Name | Description |
|-----------|-------------|-------------|
| `banner` | Banner | Small banner at top/bottom |
| `interstitial` | Interstitial | Full-screen ad |
| `rewarded` | Rewarded | Reward-based video ad |
| `appopen` | App Open | App launch ad |
| `rewarded_interstitial` | Rewarded Interstitial | Full-screen rewarded |
| `native` | Native | Custom native ad |

### 2. Ad Networks (Built-in)

| Network ID | Display Name | Module |
|-----------|-------------|---------|
| `admob` | Google AdMob | AdMobModule (Full) |
| `applovin` | AppLovin MAX | AppLovinModule |
| `ironsource` | IronSource | IronSourceModule |
| `unityads` | Unity Ads | UnityAdsModule |

### 3. Ad Events System

| Event | Firebase Event | Trigger Time | Use Case |
|-------|---------------|--------------|----------|
| `AdLoadStarted` | `ad_load_started` | Bắt đầu load | Track load time |
| `AdLoadSuccess` | `ad_load_success` | Load thành công | Success rate |
| `AdLoadFailed` | `ad_load_failed` | Load thất bại | Error tracking |
| `AdShown` | `ad_show` | Ad hiển thị | Show rate |
| `AdClicked` | `ad_click` | User click | CTR calculation |
| `AdClosed` | `ad_close` | Ad đóng | Completion rate |
| **`AdImpression`** ⭐ | **`ad_impression`** | **Impression ghi nhận** | **Main metric** |
| `AdPaid` | `ad_revenue` | Revenue tracked | Monetization |
| `AdRewarded` | `ad_rewarded` | Reward granted | Reward tracking |

### 4. Waterfall Mediation

```csharp
// Ad units được load theo priority (1 = cao nhất)

Ad Units cho "interstitial":
1. Priority 1: AdMob (ca-app-pub-xxx/111111)
2. Priority 2: AppLovin (ad-unit-222)
3. Priority 3: IronSource (ad-unit-333)

// AdBridge tự động thử từ priority cao xuống thấp
AdBridge.Instance.ShowAd("interstitial");
// --> Try AdMob first
// --> If not ready, try AppLovin
// --> If not ready, try IronSource
```

### 5. Firebase Events Generated

```json
{
  "event_name": "ad_impression",
  "parameters": {
    "ad_format": "interstitial",
    "ad_network": "admob",
    "ad_unit_id": "ca-app-pub-xxx/xxxxxxxxxx",
    "ad_platform": "android"
  }
}

{
  "event_name": "ad_revenue",
  "parameters": {
    "ad_format": "interstitial",
    "ad_network": "admob",
    "ad_unit_id": "ca-app-pub-xxx/xxxxxxxxxx",
    "ad_platform": "android",
    "value": 0.05,
    "currency": "USD",
    "revenue": 0.05
  }
}
```

---

## 🔧 Advanced: Custom Extensions

### Thêm custom ad network (Vungle example)

```csharp
using GameDevToi.ThirdLib.Core;
using GameDevToi.ThirdLib.AdModule;
using UnityEngine;

namespace GameDevToi.ThirdLib.Extensions
{
    // Step 1: Define network
    public static class CustomNetworks
    {
        public static AdNetworkDefinition Vungle = new AdNetworkDefinition
        {
            id = "vungle",
            displayName = "Vungle",
            color = new Color(0.2f, 0.6f, 1f)
        };
    }

    // Step 2: Create module
    public class VungleModule : BaseAdNetworkModule
    {
        public override string NetworkId => "vungle";

        public override void Initialize(ThirdLibConfig config)
        {
            base.Initialize(config);
            // Initialize Vungle SDK
            LogInfo("Vungle initialized");
        }

        public override void LoadAdUnit(AdUnit adUnit)
        {
            LogInfo($"Loading Vungle ad: {adUnit.GetAdUnitId()}");
            // Load Vungle ad
        }

        public override void ShowAd(string formatId, string placementId = null)
        {
            LogInfo($"Showing Vungle ad: {formatId}");
            // Show Vungle ad
        }

        public override bool IsAdReady(string formatId, string placementId = null)
        {
            return false; // Check Vungle ad ready
        }
    }

    // Step 3: Register extension
    public static class VungleExtension
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            // Register network
            AdRegistry.RegisterNetwork(CustomNetworks.Vungle);

            // Register module
            AdBridge.Instance.RegisterAdModule(new VungleModule());

            Debug.Log("[Vungle] Extension registered!");
        }
    }
}
```

### Thêm custom ad format

```csharp
// Define custom format
public static class CustomFormats
{
    public static AdFormatDefinition Playable = new AdFormatDefinition
    {
        id = "playable",
        displayName = "Playable",
        description = "Interactive playable ad"
    };
}

// Register
[RuntimeInitializeOnLoadMethod]
static void RegisterCustomFormat()
{
    AdRegistry.RegisterFormat(CustomFormats.Playable);
}

// Use
AdBridge.Instance.ShowAd("playable");
```

---

## 📊 Firebase Analytics Dashboard

### Events to Monitor

```
Key Metrics:
├── ad_impression (Total impressions) ⭐⭐⭐⭐⭐
├── ad_revenue (Total revenue) ⭐⭐⭐⭐⭐
├── ad_show (Show rate)
├── ad_click (CTR calculation)
└── ad_load_failed (Error rate)

Segmentation:
├── By ad_format: banner | interstitial | rewarded
├── By ad_network: admob | applovin | ironsource
└── By ad_platform: android | ios | webgl
```

### Custom Reports Examples

```sql
-- Total impressions by format
SELECT ad_format, COUNT(*) as impressions
FROM ad_impression
GROUP BY ad_format
ORDER BY impressions DESC

-- Revenue by network
SELECT ad_network, SUM(value) as total_revenue
FROM ad_revenue
GROUP BY ad_network
ORDER BY total_revenue DESC

-- CTR (Click-Through Rate)
SELECT 
  (COUNT(ad_click) * 100.0 / COUNT(ad_impression)) as ctr
FROM events

-- Fill rate by network
SELECT 
  ad_network,
  (COUNT(ad_load_success) * 100.0 / COUNT(ad_load_started)) as fill_rate
FROM events
GROUP BY ad_network
```

---

## 📚 API Reference

### AdBridge

```csharp
// Properties
static AdBridge Instance { get; }

// Methods
void ShowAd(string formatId, string placementId = null)
bool IsAdReady(string formatId, string placementId = null)
IAdNetworkModule GetModule(string networkId)
void RegisterAdModule(IAdNetworkModule module)
void ReloadAdUnits()
string GetDebugInfo()
void HideBanner()
void ShowBanner()
```

### AdEvents

```csharp
// Events
static event AdEventHandler OnAdEvent;
static event AdEventHandler OnAdLoadStarted;
static event AdEventHandler OnAdLoadSuccess;
static event AdEventHandler OnAdLoadFailed;
static event AdEventHandler OnAdShown;
static event AdEventHandler OnAdClicked;
static event AdEventHandler OnAdClosed;
static event AdEventHandler OnAdImpression;
static event AdEventHandler OnAdPaid;
static event AdEventHandler OnAdRewarded;

// Methods
static void TriggerEvent(AdEventArgs eventArgs)
static void Trigger(AdEventType type, string formatId, string networkId, string adUnitId)
static void TriggerFailed(string formatId, string networkId, string adUnitId, string error, int code)
static void TriggerPaid(string formatId, string networkId, string adUnitId, double revenue, string currency)
static void TriggerRewarded(string formatId, string networkId, string adUnitId, string type, double amount)
```

### FirebaseBridge

```csharp
// Properties
static FirebaseBridge Instance { get; }
bool IsInitialized { get; }

// Methods
void LogCustomEvent(string eventName, Dictionary<string, object> parameters)
void LogCustomEvent(string eventName, string paramName, string paramValue)
void SetUserProperty(string propertyName, string propertyValue)
void SetUserId(string userId)
```

### AdRegistry

```csharp
// Methods
static void RegisterFormat(AdFormatDefinition format)
static void RegisterNetwork(AdNetworkDefinition network)
static List<AdFormatDefinition> GetAllFormats()
static List<AdNetworkDefinition> GetAllNetworks()
static AdFormatDefinition GetFormat(string id)
static AdNetworkDefinition GetNetwork(string id)
static bool HasFormat(string id)
static bool HasNetwork(string id)
```

### ThirdLibConfig

```csharp
// Properties
static ThirdLibConfig Instance { get; }
string androidPackageName { get; set; }
string appVersion { get; set; }
int versionCode { get; set; }
List<AdUnit> adUnits { get; set; }

// Methods
string GetGoogleAdMobAppId()
AdUnit GetAdUnit(string formatId, string networkId)
List<AdUnit> GetActiveAdUnits(string formatId)
```

---

## 📖 Complete Example

```csharp
using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;

public class GameController : MonoBehaviour
{
    void Start()
    {
        // System tự động initialize
        Debug.Log("Ad system ready!");
        
        // Optional: Setup Firebase user properties
        SetupFirebase();
        
        // Subscribe to events for game logic
        AdEvents.OnAdRewarded += OnRewardGranted;
        AdEvents.OnAdClosed += OnAdClosed;
    }

    void OnDestroy()
    {
        AdEvents.OnAdRewarded -= OnRewardGranted;
        AdEvents.OnAdClosed -= OnAdClosed;
    }

    // Show banner when game starts
    public void OnGameStart()
    {
        AdBridge.Instance.ShowAd("banner");
    }

    // Show interstitial between levels
    public void OnLevelComplete()
    {
        if (AdBridge.Instance.IsAdReady("interstitial"))
        {
            AdBridge.Instance.ShowAd("interstitial");
        }
    }

    // Show rewarded for bonus
    public void OnWatchAdForCoins()
    {
        if (AdBridge.Instance.IsAdReady("rewarded"))
        {
            AdBridge.Instance.ShowAd("rewarded");
        }
        else
        {
            UIManager.ShowMessage("Ad not available");
        }
    }

    // Grant reward
    void OnRewardGranted(AdEventArgs e)
    {
        int coins = (int)e.RewardAmount;
        PlayerData.AddCoins(coins);
        UIManager.ShowRewardNotification($"+{coins} coins!");
    }

    // Resume game after ad closes
    void OnAdClosed(AdEventArgs e)
    {
        Time.timeScale = 1;
        AudioListener.volume = 1;
        
        if (e.FormatId == "interstitial")
        {
            // Continue to next level
            SceneManager.LoadScene("NextLevel");
        }
    }

    void SetupFirebase()
    {
        FirebaseBridge.Instance.SetUserId(GetUserId());
        FirebaseBridge.Instance.SetUserProperty("user_level", GetUserLevel().ToString());
    }

    string GetUserId() => SystemInfo.deviceUniqueIdentifier;
    int GetUserLevel() => PlayerPrefs.GetInt("Level", 1);
}
```

---

## 🎓 Summary

### Core Concepts

- **Modular**: Dễ dàng thêm ad networks mới
- **Dynamic**: String-based IDs thay vì enums
- **Event-Driven**: Decouple ad logic khỏi game logic
- **Auto-Initialization**: Singleton pattern với RuntimeInitializeOnLoadMethod
- **Firebase Ready**: Tự động log ad events

### Key Features

✅ Unity Editor integration  
✅ Multi-network waterfall mediation  
✅ Event system với 9 event types  
✅ Firebase Analytics auto-logging  
✅ DLL-compatible architecture  
✅ Zero-code Firebase integration  
✅ Extensible design  

### Getting Started

1. Open `3rdLib > Open Window`
2. Configure SDK keys
3. Add ad units
4. Call `AdBridge.Instance.ShowAd("interstitial")`
5. Done! Firebase tự động log events

---

**Made with ❤️ for Unity Developers**

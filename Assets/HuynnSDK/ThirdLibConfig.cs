using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib
{
    [CreateAssetMenu(fileName = "ThirdLibConfig", menuName = "GameDevToi/Third Lib Config")]
    public class ThirdLibConfig : ScriptableObject
    {
        private static ThirdLibConfig instance;

        [Header("App Information")]
        [Tooltip("Android Package Name (com.company.game)")]
        public string androidPackageName = "com.company.game";

        [Tooltip("App Version (e.g., 1.0.0)")]
        public string appVersion = "1.0.0";

        [Tooltip("Version Code (integer)")]
        public int versionCode = 1;

        [Header("Google Mobile Ads")]
        [Tooltip("Android App ID for Google Mobile Ads")]
        public string googleAdMobAndroidAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";

        [Tooltip("iOS App ID for Google Mobile Ads")]
        public string googleAdMobIOSAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";

        [Header("Facebook")]
        [Tooltip("Facebook App ID")]
        public string facebookAppId = "";

        [Tooltip("Facebook Client Token")]
        public string facebookClientToken = "";

        [Header("AppLovin")]
        [Tooltip("AppLovin SDK Key")]
        public string appLovinSdkKey = "";

        [Header("IronSource")]
        [Tooltip("IronSource App Key")]
        public string ironSourceAppKey = "";

        [Header("Ad Units")]
        [Tooltip("Danh sách các đơn vị quảng cáo")]
        public List<AdUnit> adUnits = new List<AdUnit>();

        /// <summary>
        /// Lấy instance của ThirdLibConfig (Runtime - dùng Resources.Load)
        /// </summary>
        public static ThirdLibConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<ThirdLibConfig>("ThirdLibConfig");

                    if (instance == null)
                    {
                        Debug.LogError("ThirdLibConfig not found in Resources folder! Please create one using 3rdLib > Open Window");
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Lấy Google AdMob App ID theo platform hiện tại
        /// </summary>
        public string GetGoogleAdMobAppId()
        {
#if UNITY_ANDROID
            return googleAdMobAndroidAppId;
#elif UNITY_IOS
            return googleAdMobIOSAppId;
#else
            return googleAdMobAndroidAppId; // Fallback
#endif
        }

        /// <summary>
        /// Lấy tất cả ad units theo format ID
        /// </summary>
        public List<AdUnit> GetAdUnitsByFormat(string formatId)
        {
            return adUnits.Where(ad => ad.formatId == formatId && ad.isActive && ad.IsValid()).ToList();
        }

        /// <summary>
        /// Lấy ad unit theo format và network ID
        /// </summary>
        public AdUnit GetAdUnit(string formatId, string networkId)
        {
            return adUnits.FirstOrDefault(ad => ad.formatId == formatId && ad.networkId == networkId && ad.isActive && ad.IsValid());
        }

        /// <summary>
        /// Lấy tất cả ad units đang active theo format và sắp xếp theo priority
        /// </summary>
        public List<AdUnit> GetActiveAdUnits(string formatId)
        {
            return adUnits
                .Where(ad => ad.formatId == formatId && ad.isActive && ad.IsValid())
                .OrderByDescending(ad => ad.priority)
                .ToList();
        }
    }
}

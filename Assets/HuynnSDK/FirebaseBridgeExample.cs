using UnityEngine;
using GameDevToi.ThirdLib;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.Example
{
    /// <summary>
    /// Example: Cách sử dụng FirebaseBridge
    /// FirebaseBridge tự động log ad events lên Firebase Analytics
    /// </summary>
    public class FirebaseBridgeExample : MonoBehaviour
    {
        private void Start()
        {
            // FirebaseBridge tự động khởi tạo khi game start
            // Bạn không cần làm gì cả, nó sẽ tự động log ad events!

            if (FirebaseBridge.Instance != null)
            {
                Debug.Log("FirebaseBridge is ready and logging ad events automatically!");
                Debug.Log($"Firebase initialized: {FirebaseBridge.Instance.IsInitialized}");
            }

            // Optional: Set user properties
            SetupUserProperties();
        }

        /// <summary>
        /// Setup user properties cho Firebase Analytics
        /// </summary>
        private void SetupUserProperties()
        {
            if (FirebaseBridge.Instance == null) return;

            // Set user ID
            string userId = SystemInfo.deviceUniqueIdentifier;
            FirebaseBridge.Instance.SetUserId(userId);

            // Set custom properties
            FirebaseBridge.Instance.SetUserProperty("user_level", GetUserLevel().ToString());
            FirebaseBridge.Instance.SetUserProperty("app_version", Application.version);
            FirebaseBridge.Instance.SetUserProperty("device_model", SystemInfo.deviceModel);
        }

        /// <summary>
        /// Example: Log custom game events
        /// </summary>
        public void OnLevelComplete(int level, int score)
        {
            if (FirebaseBridge.Instance == null) return;

            var parameters = new System.Collections.Generic.Dictionary<string, object>
            {
                { "level", level },
                { "score", score },
                { "time_played", Time.timeSinceLevelLoad }
            };

            FirebaseBridge.Instance.LogCustomEvent("level_complete", parameters);
        }

        /// <summary>
        /// Example: Log purchase event
        /// </summary>
        public void OnPurchase(string itemId, double price, string currency)
        {
            if (FirebaseBridge.Instance == null) return;

            var parameters = new System.Collections.Generic.Dictionary<string, object>
            {
                { "item_id", itemId },
                { "value", price },
                { "currency", currency }
            };

            FirebaseBridge.Instance.LogCustomEvent("purchase", parameters);
        }

        /// <summary>
        /// Example: Simple event log
        /// </summary>
        public void OnGameStart()
        {
            if (FirebaseBridge.Instance == null) return;

            FirebaseBridge.Instance.LogCustomEvent("game_start", "scene_name", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        // Helper
        private int GetUserLevel()
        {
            return PlayerPrefs.GetInt("UserLevel", 1);
        }

        #region Test GUI
        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 200, 300, 400));
            GUILayout.Label("=== FirebaseBridge Test ===");

            if (GUILayout.Button("Log Custom Event"))
            {
                FirebaseBridge.Instance?.LogCustomEvent("test_event", "test_param", "test_value");
            }

            if (GUILayout.Button("Set User Property"))
            {
                FirebaseBridge.Instance?.SetUserProperty("test_property", "test_value");
            }

            if (GUILayout.Button("Log Level Complete"))
            {
                OnLevelComplete(1, 1000);
            }

            GUILayout.Space(10);
            GUILayout.Label("Note: Ad events are logged automatically!");
            GUILayout.Label("Just use AdBridge.ShowAd() and FirebaseBridge");
            GUILayout.Label("will capture all ad impressions automatically.");

            GUILayout.EndArea();
        }
        #endregion
    }
}

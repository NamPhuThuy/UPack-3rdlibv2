using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using GameDevToi.ThirdLib.Core;
using GameDevToi.ThirdLib.Editor;

namespace GameDevToi.ThirdLib
{
    public class ThirdLibWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private ThirdLibConfig config;
        private SerializedObject serializedConfig;
        private const string CONFIG_PATH = "Assets/Resources/ThirdLibConfig.asset";
        private const string RESOURCES_FOLDER = "Assets/Resources";

        // Tab management
        private int selectedTab = 0;
        private readonly string[] tabs = { "SDK Config", "Ad Units" };

        // Ad Units management
        private Dictionary<string, bool> adFormatFoldouts = new Dictionary<string, bool>();
        private Vector2 adUnitsScrollPosition;

        [MenuItem("3rdLib/Open Window")]
        public static void ShowWindow()
        {
            ThirdLibWindow window = GetWindow<ThirdLibWindow>("3rd Lib Manager");
            window.minSize = new Vector2(600, 700);
            window.Show();
        }

        private void OnEnable()
        {
            LoadOrCreateConfig();
            InitializeAdFormatFoldouts();
        }

        private void InitializeAdFormatFoldouts()
        {
            var formats = AdRegistry.GetAllFormats();
            foreach (var format in formats)
            {
                if (!adFormatFoldouts.ContainsKey(format.id))
                {
                    adFormatFoldouts[format.id] = true;
                }
            }
        }

        private void LoadOrCreateConfig()
        {
            // Tìm config trong Resources
            config = Resources.Load<ThirdLibConfig>("ThirdLibConfig");

            // Nếu chưa tồn tại, tạo mới
            if (config == null)
            {
                // Tạo thư mục Resources nếu chưa có
                if (!Directory.Exists(RESOURCES_FOLDER))
                {
                    Directory.CreateDirectory(RESOURCES_FOLDER);
                    AssetDatabase.Refresh();
                }

                // Tạo ScriptableObject mới
                config = CreateInstance<ThirdLibConfig>();
                AssetDatabase.CreateAsset(config, CONFIG_PATH);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log($"Created new ThirdLibConfig at {CONFIG_PATH}");
            }

            if (config != null)
            {
                serializedConfig = new SerializedObject(config);
            }
        }

        private void OnGUI()
        {
            if (config == null || serializedConfig == null)
            {
                EditorGUILayout.HelpBox("Config file không tồn tại. Đang tải lại...", MessageType.Warning);
                if (GUILayout.Button("Reload Config"))
                {
                    LoadOrCreateConfig();
                }
                return;
            }

            serializedConfig.Update();

            GUILayout.Label("3rd Library Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Draw tabs
            selectedTab = GUILayout.Toolbar(selectedTab, tabs, GUILayout.Height(30));
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            switch (selectedTab)
            {
                case 0:
                    DrawSDKConfigTab();
                    break;
                case 1:
                    DrawAdUnitsTab();
                    break;
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();
            DrawBottomButtons();

            serializedConfig.ApplyModifiedProperties();
        }

        private void DrawSDKConfigTab()
        {
            EditorGUILayout.HelpBox("Cấu hình các ID cho các SDK bên thứ 3. Dữ liệu này sẽ được sử dụng trong Runtime.", MessageType.Info);
            EditorGUILayout.Space();

            // App Information Section
            DrawSection("App Information", () =>
            {
                SerializedProperty androidPackageName = serializedConfig.FindProperty("androidPackageName");
                SerializedProperty appVersion = serializedConfig.FindProperty("appVersion");
                SerializedProperty versionCode = serializedConfig.FindProperty("versionCode");

                EditorGUILayout.PropertyField(androidPackageName, new GUIContent("Android Package Name"));
                EditorGUILayout.PropertyField(appVersion, new GUIContent("App Version"));
                EditorGUILayout.PropertyField(versionCode, new GUIContent("Version Code"));
            });

            EditorGUILayout.Space();

            // Google Mobile Ads Section
            DrawSection("Google Mobile Ads", () =>
            {
                SerializedProperty androidAppId = serializedConfig.FindProperty("googleAdMobAndroidAppId");
                SerializedProperty iosAppId = serializedConfig.FindProperty("googleAdMobIOSAppId");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(androidAppId, new GUIContent("Android App ID"));
                EditorGUILayout.PropertyField(iosAppId, new GUIContent("iOS App ID"));

                // Khi App ID thay đổi, cập nhật vào GoogleMobileAdsSettings
                if (EditorGUI.EndChangeCheck())
                {
                    serializedConfig.ApplyModifiedProperties();

                    // Cập nhật GoogleMobileAdsSettings (nếu SDK đã import)
                    try
                    {
                        // Tìm type trong tất cả assemblies
                        var googleMobileAdsSettingsType = System.AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.Name == "GoogleMobileAdsSettings");

                        if (googleMobileAdsSettingsType != null)
                        {
                            // LoadInstance() là internal, cần NonPublic flag
                            var loadInstanceMethod = googleMobileAdsSettingsType.GetMethod("LoadInstance",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            if (loadInstanceMethod != null)
                            {
                                var googleMobileAdsSettings = loadInstanceMethod.Invoke(null, null);
                                if (googleMobileAdsSettings != null)
                                {
                                    var androidAppIdProperty = googleMobileAdsSettingsType.GetProperty("GoogleMobileAdsAndroidAppId");
                                    var iosAppIdProperty = googleMobileAdsSettingsType.GetProperty("GoogleMobileAdsIOSAppId");

                                    if (androidAppIdProperty != null && iosAppIdProperty != null)
                                    {
                                        androidAppIdProperty.SetValue(googleMobileAdsSettings, config.googleAdMobAndroidAppId);
                                        iosAppIdProperty.SetValue(googleMobileAdsSettings, config.googleAdMobIOSAppId);

                                        EditorUtility.SetDirty((UnityEngine.Object)googleMobileAdsSettings);
                                        AssetDatabase.SaveAssets();
                                        Debug.Log($"[3rdLib] Updated Google Mobile Ads App IDs");
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[3rdLib] Could not update GoogleMobileAdsSettings: {ex.Message}");
                    }
                }
            });

            EditorGUILayout.Space();

            // Facebook Section
            DrawSection("Facebook", () =>
            {
                SerializedProperty facebookAppId = serializedConfig.FindProperty("facebookAppId");
                SerializedProperty facebookClientToken = serializedConfig.FindProperty("facebookClientToken");

                EditorGUILayout.PropertyField(facebookAppId, new GUIContent("App ID"));
                EditorGUILayout.PropertyField(facebookClientToken, new GUIContent("Client Token"));
            });

            EditorGUILayout.Space();

            // AppLovin Section
            DrawSection("AppLovin", () =>
            {
                SerializedProperty appLovinSdkKey = serializedConfig.FindProperty("appLovinSdkKey");

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(appLovinSdkKey, new GUIContent("SDK Key"));

                // Khi SDK Key thay đổi, cập nhật vào AppLovinSettings
                if (EditorGUI.EndChangeCheck())
                {
                    serializedConfig.ApplyModifiedProperties();

                    // Cập nhật AppLovinSettings ScriptableObject (nếu SDK đã import)
                    try
                    {
                        // Tìm type trong tất cả assemblies
                        var appLovinSettingsType = System.AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .FirstOrDefault(t => t.Name == "AppLovinSettings");

                        if (appLovinSettingsType != null)
                        {
                            var instanceProperty = appLovinSettingsType.GetProperty("Instance", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            if (instanceProperty != null)
                            {
                                var appLovinSettings = instanceProperty.GetValue(null);
                                if (appLovinSettings != null)
                                {
                                    var sdkKeyProperty = appLovinSettingsType.GetProperty("SdkKey");
                                    if (sdkKeyProperty != null)
                                    {
                                        sdkKeyProperty.SetValue(appLovinSettings, config.appLovinSdkKey);

                                        var saveMethod = appLovinSettingsType.GetMethod("SaveAsync");
                                        if (saveMethod != null)
                                        {
                                            saveMethod.Invoke(appLovinSettings, null);
                                            Debug.Log($"[3rdLib] Updated AppLovin SDK Key: {config.appLovinSdkKey}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogWarning($"[3rdLib] Could not update AppLovinSettings: {ex.Message}");
                    }
                }
            });

            EditorGUILayout.Space();

            // IronSource Section
            DrawSection("IronSource", () =>
            {
                SerializedProperty ironSourceAppKey = serializedConfig.FindProperty("ironSourceAppKey");
                EditorGUILayout.PropertyField(ironSourceAppKey, new GUIContent("App Key"));
            });
        }

        private void DrawAdUnitsTab()
        {
            EditorGUILayout.HelpBox("Quản lý các đơn vị quảng cáo (Ad Units). Bạn có thể thêm, sửa, xóa các đơn vị quảng cáo theo format và network.", MessageType.Info);
            EditorGUILayout.Space();

            // Add new ad unit button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("+ Thêm đơn vị quảng cáo", GUILayout.Height(35), GUILayout.Width(200)))
            {
                ShowAddAdUnitMenu();
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (config.adUnits == null || config.adUnits.Count == 0)
            {
                EditorGUILayout.HelpBox("Chưa có đơn vị quảng cáo nào. Nhấn nút 'Thêm đơn vị quảng cáo' để bắt đầu.", MessageType.Info);
                return;
            }

            // Group ad units by format
            var groupedAdUnits = config.adUnits
                .Where(ad => ad.IsValid())
                .GroupBy(ad => ad.formatId)
                .OrderBy(g => g.Key);

            foreach (var group in groupedAdUnits)
            {
                string formatId = group.Key;
                List<AdUnit> units = group.ToList();

                var formatDef = AdRegistry.GetFormat(formatId);
                string formatDisplayName = formatDef?.displayName ?? formatId;

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                // Foldout header with count
                EditorGUILayout.BeginHorizontal();
                if (!adFormatFoldouts.ContainsKey(formatId))
                    adFormatFoldouts[formatId] = true;

                adFormatFoldouts[formatId] = EditorGUILayout.Foldout(
                    adFormatFoldouts[formatId],
                    $"{formatDisplayName} ({units.Count})",
                    true,
                    EditorStyles.foldoutHeader
                );
                EditorGUILayout.EndHorizontal();

                if (adFormatFoldouts[formatId])
                {
                    EditorGUI.indentLevel++;

                    for (int i = 0; i < units.Count; i++)
                    {
                        DrawAdUnit(units[i], i);

                        if (i < units.Count - 1)
                        {
                            EditorGUILayout.Space(5);
                            DrawSeparator();
                            EditorGUILayout.Space(5);
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }
        }

        private void DrawAdUnit(AdUnit adUnit, int index)
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            // Header with network icon and delete button
            EditorGUILayout.BeginHorizontal();

            adUnit.isActive = EditorGUILayout.Toggle(adUnit.isActive, GUILayout.Width(20));

            GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel);
            if (!adUnit.isActive)
            {
                headerStyle.normal.textColor = Color.gray;
            }

            string networkDisplayName = AdEditorHelper.GetNetworkDisplayName(adUnit.networkId);
            EditorGUILayout.LabelField(networkDisplayName, headerStyle, GUILayout.Width(100));

            adUnit.name = EditorGUILayout.TextField(adUnit.name);

            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("✕", GUILayout.Width(25), GUILayout.Height(20)))
            {
                if (EditorUtility.DisplayDialog("Xóa Ad Unit",
                    $"Bạn có chắc muốn xóa '{adUnit.name}'?",
                    "Xóa", "Hủy"))
                {
                    config.adUnits.Remove(adUnit);
                    EditorUtility.SetDirty(config);
                    GUIUtility.ExitGUI();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Network dropdown
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Network:", GUILayout.Width(80));
            adUnit.networkId = AdEditorHelper.DrawNetworkDropdown(adUnit.networkId);
            EditorGUILayout.EndHorizontal();

            // Priority
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Priority:", GUILayout.Width(80));
            adUnit.priority = EditorGUILayout.IntField(adUnit.priority);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // Android Ad Unit ID
            EditorGUILayout.LabelField("Android Ad Unit ID:", EditorStyles.boldLabel);
            adUnit.androidAdUnitId = EditorGUILayout.TextField(adUnit.androidAdUnitId);

            // iOS Ad Unit ID
            EditorGUILayout.LabelField("iOS Ad Unit ID:", EditorStyles.boldLabel);
            adUnit.iosAdUnitId = EditorGUILayout.TextField(adUnit.iosAdUnitId);

            // Notes
            if (!string.IsNullOrEmpty(adUnit.notes))
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.LabelField("Notes:", EditorStyles.miniLabel);
                adUnit.notes = EditorGUILayout.TextArea(adUnit.notes, GUILayout.Height(40));
            }
            else
            {
                if (GUILayout.Button("+ Add Notes", GUILayout.Height(20)))
                {
                    adUnit.notes = "";
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void ShowAddAdUnitMenu()
        {
            AdEditorHelper.ShowAddAdUnitMenu((formatId, networkId) => AddNewAdUnit(formatId, networkId));
        }

        private void AddNewAdUnit(string formatId, string networkId)
        {
            if (config.adUnits == null)
            {
                config.adUnits = new List<AdUnit>();
            }

            AdUnit newAdUnit = new AdUnit(formatId, networkId);
            config.adUnits.Add(newAdUnit);

            EditorUtility.SetDirty(config);

            // Ensure foldout is open for the new ad unit's format
            if (!adFormatFoldouts.ContainsKey(formatId))
            {
                adFormatFoldouts[formatId] = true;
            }
            else
            {
                adFormatFoldouts[formatId] = true;
            }
        }

        private void DrawSeparator()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));
        }

        private void DrawBottomButtons()
        {
            // Save button
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Save Changes", GUILayout.Width(150), GUILayout.Height(30)))
            {
                SaveConfig();
            }

            if (selectedTab == 0 && GUILayout.Button("Reset to Default", GUILayout.Width(150), GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Reset Config",
                    "Bạn có chắc muốn reset tất cả về giá trị mặc định?",
                    "Yes", "No"))
                {
                    ResetConfig();
                }
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // File location info
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"Config file: {CONFIG_PATH}", EditorStyles.miniLabel);
            if (GUILayout.Button("Select in Project", GUILayout.Width(120)))
            {
                Selection.activeObject = config;
                EditorGUIUtility.PingObject(config);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSection(string title, System.Action drawContent)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label(title, EditorStyles.boldLabel);
            EditorGUILayout.Space(5);
            drawContent?.Invoke();
            EditorGUILayout.EndVertical();
        }

        private void SaveConfig()
        {
            if (serializedConfig != null)
            {
                serializedConfig.ApplyModifiedProperties();
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("ThirdLibConfig saved successfully!");
                ShowNotification(new GUIContent("✓ Đã lưu thành công!"));
            }
        }

        private void ResetConfig()
        {
            if (config != null)
            {
                config.androidPackageName = "com.company.game";
                config.appVersion = "1.0.0";
                config.versionCode = 1;
                config.googleAdMobAndroidAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";
                config.googleAdMobIOSAppId = "ca-app-pub-xxxxxxxxxxxxxxxx~yyyyyyyyyy";
                config.facebookAppId = "";
                config.facebookClientToken = "";
                config.appLovinSdkKey = "";
                config.ironSourceAppKey = "";

                serializedConfig.Update();
                SaveConfig();
            }
        }
    }
}

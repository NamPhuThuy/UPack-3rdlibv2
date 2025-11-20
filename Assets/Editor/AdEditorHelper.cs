using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using GameDevToi.ThirdLib.Core;

namespace GameDevToi.ThirdLib.Editor
{
    /// <summary>
    /// Helper methods cho ThirdLibWindow Editor
    /// </summary>
    public static class AdEditorHelper
    {
        /// <summary>
        /// Tạo menu để chọn Ad Format và Network
        /// </summary>
        public static void ShowAddAdUnitMenu(System.Action<string, string> onSelected)
        {
            GenericMenu menu = new GenericMenu();

            var formats = AdRegistry.GetAllFormats();
            var networks = AdRegistry.GetAllNetworks();

            foreach (var format in formats)
            {
                foreach (var network in networks)
                {
                    string menuPath = $"{format.displayName}/{network.displayName}";
                    menu.AddItem(
                        new GUIContent(menuPath),
                        false,
                        () => onSelected?.Invoke(format.id, network.id)
                    );
                }
            }

            menu.ShowAsContext();
        }

        /// <summary>
        /// Vẽ dropdown cho Format selection
        /// </summary>
        public static string DrawFormatDropdown(string currentFormatId, GUIContent label = null)
        {
            var formats = AdRegistry.GetAllFormats();
            var currentFormat = AdRegistry.GetFormat(currentFormatId);

            if (label == null)
                label = new GUIContent("Format");

            int currentIndex = 0;
            string[] displayNames = new string[formats.Count];

            for (int i = 0; i < formats.Count; i++)
            {
                displayNames[i] = formats[i].displayName;
                if (formats[i].id == currentFormatId)
                    currentIndex = i;
            }

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup(label, currentIndex, displayNames);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < formats.Count)
            {
                return formats[newIndex].id;
            }

            return currentFormatId;
        }

        /// <summary>
        /// Vẽ dropdown cho Network selection
        /// </summary>
        public static string DrawNetworkDropdown(string currentNetworkId, GUIContent label = null)
        {
            var networks = AdRegistry.GetAllNetworks();
            var currentNetwork = AdRegistry.GetNetwork(currentNetworkId);

            if (label == null)
                label = new GUIContent("Network");

            int currentIndex = 0;
            string[] displayNames = new string[networks.Count];

            for (int i = 0; i < networks.Count; i++)
            {
                displayNames[i] = networks[i].displayName;
                if (networks[i].id == currentNetworkId)
                    currentIndex = i;
            }

            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUILayout.Popup(label, currentIndex, displayNames);
            if (EditorGUI.EndChangeCheck() && newIndex >= 0 && newIndex < networks.Count)
            {
                return networks[newIndex].id;
            }

            return currentNetworkId;
        }

        /// <summary>
        /// Lấy màu cho network
        /// </summary>
        public static Color GetNetworkColor(string networkId)
        {
            var network = AdRegistry.GetNetwork(networkId);
            return network?.editorColor ?? Color.white;
        }

        /// <summary>
        /// Lấy display name cho format
        /// </summary>
        public static string GetFormatDisplayName(string formatId)
        {
            var format = AdRegistry.GetFormat(formatId);
            return format?.displayName ?? formatId;
        }

        /// <summary>
        /// Lấy display name cho network
        /// </summary>
        public static string GetNetworkDisplayName(string networkId)
        {
            var network = AdRegistry.GetNetwork(networkId);
            return network?.displayName ?? networkId;
        }
    }
}

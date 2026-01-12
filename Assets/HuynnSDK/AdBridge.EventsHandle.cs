using System;
using System.Collections;
using System.Collections.Generic;
using GameDevToi.ThirdLib.AdModule;
using MoreMountains.Tools;
using NamPhuThuy;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameDevToi.ThirdLib
{
    
    public partial class AdBridge : MMEventListener<ETimePassed>
    {
        #region Private Serializable Fields
        
        [Header("Flags")]
        [SerializeField] private bool isReadyToShowNextInter = true;
        public bool IsReadyToShowNextInter => isReadyToShowNextInter; 
        
        [Header("Stats")]
        [SerializeField] private float remainTimeForNextInter = AdModuleConst.INTER_TIME_BETWEEN_ADS;
        

        #endregion

        #region Private Fields

        #endregion


        #region Private Methods
        #endregion

        #region Public Methods
        
        public void ResetRemainTimeForNextInter()
        {
            isReadyToShowNextInter = false;
            remainTimeForNextInter = AdModuleConst.INTER_TIME_BETWEEN_ADS;
        }
        
        #endregion

        #region Editor Methods

        public void ResetValues()
        {
            
        }

        #endregion

        #region Events Handle

        public void OnMMEvent(ETimePassed eventType)
        {
            if (remainTimeForNextInter <= 0)
            {
                isReadyToShowNextInter = true;
                remainTimeForNextInter = 0f;
            }
            else
            {
                remainTimeForNextInter -= eventType.deltaTime;
            }
        }
        

        #endregion
    }

    /*#if UNITY_EDITOR
    [CustomEditor(typeof(AdBridge.EventsHandle))]
    [CanEditMultipleObjects]
    public class AdBridge.EventsHandleEditor : Editor
    {
        private AdBridge.EventsHandle script;
        private Texture2D frogIcon;
        
        private void OnEnable()
        {
            frogIcon = Resources.Load<Texture2D>("frog"); // no extension needed
        }
        
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            script = (AdBridge.EventsHandle)target;

            ButtonResetValues();
        }

        private void ButtonResetValues()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Reset Values", frogIcon), GUILayout.Width(InspectorConst.BUTTON_WIDTH_MEDIUM)))
            {
                script.ResetValues();
                EditorUtility.SetDirty(script); // Mark the object as dirty
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
    #endif*/
}
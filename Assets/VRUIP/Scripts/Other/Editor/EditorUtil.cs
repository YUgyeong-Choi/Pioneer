using UnityEditor;
using UnityEngine;

namespace VRUIP
{
    public static class EditorUtil
    {
        // CONSTANTS
        private const string OculusIntegration = "Oculus";
        private const string XRInteractionToolkit = "XRITK";
        private const string UnityEditor = "UnityEditor";

        public static VRUIPManager.TargetFramework GetTargetFramework()
        {
            return StringToTargetFramework(EditorPrefs.GetString(VRUIPMenu.SettingPrefKey));
        }
        
        public static VRUIPManager.TargetFramework StringToTargetFramework(string targetFramework)
        {
            switch (targetFramework)
            {
                case OculusIntegration:
                    return VRUIPManager.TargetFramework.OculusIntegration;
                case XRInteractionToolkit:
                    return VRUIPManager.TargetFramework.XRInteractionToolkit;
                case UnityEditor:
                    return VRUIPManager.TargetFramework.UnityEditorTesting;
                default:
                    return VRUIPManager.TargetFramework.UnityEditorTesting;
            }
        }
        
        public static string TargetFrameworkToString(VRUIPManager.TargetFramework targetFramework)
        {
            switch (targetFramework)
            {
                case VRUIPManager.TargetFramework.OculusIntegration:
                    return OculusIntegration;
                case VRUIPManager.TargetFramework.XRInteractionToolkit:
                    return XRInteractionToolkit;
                case VRUIPManager.TargetFramework.UnityEditorTesting:
                    return UnityEditor;
                default:
                    return UnityEditor;
            }
        }
    }
}

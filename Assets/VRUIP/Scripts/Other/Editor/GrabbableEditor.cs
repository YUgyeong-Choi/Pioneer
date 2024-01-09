using UnityEditor;
using UnityEngine;
#if XR_ITK
using UnityEngine.XR.Interaction.Toolkit;
#endif

namespace VRUIP
{
    [CustomEditor(typeof(A_Grabbable))]
    public class GrabbableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var grabbable = (A_Grabbable) target;
            
            #if OCULUS_INTEGRATION
            var components = grabbable.GetComponents<OVRGrabbable>();
            if (components.Length > 1)
            {
                for (var i = components.Length - 1; i > 0; i--)
                {
                    DestroyImmediate(components[i]);
                }
            }
            #elif XR_ITK
            var components = grabbable.GetComponents<XRGrabInteractable>();
            if (components.Length > 1)
            {
                for (var i = components.Length - 1; i > 0; i--)
                {
                    DestroyImmediate(components[i]);
                }
            }
            #endif
                
        }
    }
}

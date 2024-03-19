using Fusion;
using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class LightBulb : NetworkBehaviour
    {
        [SerializeField] MeshRenderer lightBulbRenderer;
        [SerializeField] Material lightOffMaterial;
        [SerializeField] Material lightOnMaterial;
        [SerializeField] Light pointLight;

        [Networked(OnChanged = nameof(OnLightStateChanged))]
        public bool LightOn { get; set;}

        public override void Spawned()
        {
            base.Spawned();
            if (LightOn) lightBulbRenderer.material = lightOnMaterial;
            else lightBulbRenderer.material = lightOffMaterial;
        }

        //This is called from the button press
        public void ToggleLight()
        {
            RPC_ToggleLight();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_ToggleLight()
        {
            LightOn = !LightOn;
        }

        public static void OnLightStateChanged(Changed<LightBulb> changed)
        {
            //Called when the light state changes
            if (changed.Behaviour.LightOn)
            {
                changed.Behaviour.lightBulbRenderer.material = changed.Behaviour.lightOnMaterial;
                changed.Behaviour.pointLight.enabled = true;
            }
            else
            {
                changed.Behaviour.lightBulbRenderer.material = changed.Behaviour.lightOffMaterial;
                changed.Behaviour.pointLight.enabled = false;
            }
        }
    }
}

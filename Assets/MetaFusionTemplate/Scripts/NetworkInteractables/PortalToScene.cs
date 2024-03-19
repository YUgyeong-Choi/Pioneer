using UnityEngine;

namespace Chiligames.MetaFusionTemplate
{
    public class PortalToScene : MonoBehaviour
    {
        [Tooltip("The unity scene name to load")]
        [SerializeField] private string sceneName;
        private NetworkRunnerHandler networkRunnerHandler;

        private void Start()
        {
            networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        }
        
        [ContextMenu("GoToScene")]
        public void GoToScene()
        {
            StartCoroutine(networkRunnerHandler.LoadNewScene(sceneName));
        }
    }
}

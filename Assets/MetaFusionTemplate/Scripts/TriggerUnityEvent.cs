using UnityEngine;
using UnityEngine.Events;

namespace Chiligames.MetaFusionTemplate
{
    public class TriggerUnityEvent : MonoBehaviour
    {
        [SerializeField] bool triggerOnce;
        private bool alreadyTriggered;
        public UnityEvent OnTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (alreadyTriggered && triggerOnce) return;
            alreadyTriggered = true;
            OnTriggered.Invoke();
        }
    }
}
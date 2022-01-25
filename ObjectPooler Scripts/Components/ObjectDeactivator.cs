using UnityEngine;

namespace JoaoMilone.Pooler.Components
{
    public class ObjectDeactivator : MonoBehaviour
    {
        [Tooltip("The amount of time that the object will be activated in scene!")]
        [SerializeField] private float deactivateTime = 1.5f;

        private void OnEnable() => Invoke(nameof(HideObject), deactivateTime);
        
        private void OnDisable() => CancelInvoke(nameof(HideObject));
        
        private void HideObject()
        {
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }
    }
}
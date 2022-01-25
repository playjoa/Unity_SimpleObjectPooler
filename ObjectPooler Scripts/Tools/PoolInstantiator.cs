using JoaoMilone.Pooler.Components;
using UnityEngine;

namespace JoaoMilone.Pooler.Tools
{
    public static class PoolInstantiator
    {
        private static Transform targetTransform;

        public static void Initiate(Transform transform) => targetTransform = transform;
        
        public static GameObject InstantiateLocalPosition(PoolHolderObject holderObject, Vector3 position)
        {
            var aux = InstantiateAndSetUp(holderObject);
            ObjectPreparer.PrepareObjectWithLocalPosition(aux, position);
            return aux;
        }

        public static GameObject Instantiate(PoolHolderObject holderObject, Vector3 position)
        {
            var aux = InstantiateAndSetUp(holderObject);
            ObjectPreparer.PrepareObject(aux, position);
            return aux;
        }

        public static GameObject Instantiate(PoolHolderObject holderObject, Transform tf)
        {
            var aux = InstantiateAndSetUp(holderObject);
            ObjectPreparer.PrepareObject(aux, tf);
            return aux;
        }

        public static GameObject Instantiate(PoolHolderObject holderObject, Vector3 position, Quaternion idt)
        {
            var aux = InstantiateAndSetUp(holderObject);
            ObjectPreparer.PrepareObject(aux, position, idt);
            return aux;
        }

        private static GameObject InstantiateAndSetUp(PoolHolderObject holderObject)
        {
            var aux = Object.Instantiate(holderObject.GameObjectPrefab);
            holderObject.ReceiveObject(aux);
            SetParent(aux);
            return aux;
        }
        
        public static void SetParent(GameObject objectToSetParent)
        {
            if (targetTransform != null)
                objectToSetParent.transform.SetParent(targetTransform);
        }
    }
}
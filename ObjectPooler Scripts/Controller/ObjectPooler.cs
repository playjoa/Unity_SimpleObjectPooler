using System.Collections.Generic;
using System.Linq;
using JoaoMilone.Pooler.Components;
using JoaoMilone.Pooler.Tools;
using UnityEngine;

namespace JoaoMilone.Pooler.Controller
{
    public class ObjectPooler : MonoBehaviour
    {
        [Header("-----Pool Configuration!-----")]
        [Tooltip("If you wish to keep objects separated in a parent, reference it here!")]
        [SerializeField] private Transform pool_Parent;
        
        [Header("-----Object List-----")] 
        [Tooltip("Configure your PoolObjects")] 
        [SerializeField] private List<PoolObject> objectsList;

        private Dictionary<string, PoolHolderObject> poolHolderDictionary;

        public static ObjectPooler ME;

        private void Awake()
        {
            ME = this;
            Initiate();
        }

        private void Initiate()
        {
            PoolInstantiator.Initiate(pool_Parent);
            poolHolderDictionary = new Dictionary<string, PoolHolderObject>();

            foreach (var poolObjectToHold in objectsList)
            {
                var poolHolderObject = new PoolHolderObject(poolObjectToHold);
                poolHolderDictionary.Add(poolHolderObject.Id, poolHolderObject);
                PreMakeObjects(poolHolderObject);
            }
        }

        private static void PreMakeObjects(PoolHolderObject objectToPremake)
        {
            if (objectToPremake.PreMakeQty == 0) return;

            for (var i = 0; i < objectToPremake.PreMakeQty; i++)
            {
                if (objectToPremake.PoolCount < objectToPremake.PreMakeQty || objectToPremake.MaxQtyAllowed == 0)
                {
                    //Getting the reference to gameobject
                    var aux = Instantiate(objectToPremake.GameObjectPrefab);

                    //Setting object to parent if it exists
                    PoolInstantiator.SetParent(aux);

                    //Adding Object to Pool
                    objectToPremake.ReceiveObject(aux);

                    //Inactivating Objects if not deactivated already
                    if (aux.activeSelf)
                        aux.SetActive(false);
                }
                else
                    break;
            }
        }

        ///<summary>
        ///Requests an object from the pool and assigns the transform reference to object and returns a GameObject reference.
        ///</summary>
        public GameObject RequestObject(string id, Transform tf)
        {
            if (!PoolExists(id)) return null;

            //Try to get the first on active in pool
            var currentHolder = poolHolderDictionary[id];
            var objectRequested = currentHolder.FirstActive;
            
            if (objectRequested != null)
            {
                ObjectPreparer.PrepareObject(objectRequested, tf);
                return objectRequested;
            }
            
            //Could not find available PoolObject to use! Instantiating new one if max qty not reached
            if (currentHolder.CanInstantiateMore)
                objectRequested = PoolInstantiator.Instantiate(currentHolder, tf);
            else
            {
                //Getting farthest Object in list to use in request
                objectRequested = currentHolder.GetObjectAtCounter();
                ObjectPreparer.PrepareObject(objectRequested, tf);
            }
            return objectRequested;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a position to be activated! The rotation of the object will be the same as prefab.
        ///</summary>
        public GameObject RequestObject(string id, Vector3 position)
        {
            if (!PoolExists(id)) return null;

            //Try to get the first on active in pool
            var currentHolder = poolHolderDictionary[id];
            var objectRequested = currentHolder.FirstActive;
            
            if (objectRequested != null)
            {
                ObjectPreparer.PrepareObject(objectRequested, position);
                return objectRequested;
            }
            
            //Could not find available PoolObject to use! Instantiating new one if max qty not reached
            if (currentHolder.CanInstantiateMore)
                objectRequested = PoolInstantiator.Instantiate(currentHolder, position);
            else
            {
                //Getting farthest Object in list to use in request
                objectRequested = currentHolder.GetObjectAtCounter();
                ObjectPreparer.PrepareObject(objectRequested, position);
            }
            return objectRequested;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a position and rotation.
        ///</summary>
        public GameObject RequestObject(string id, Vector3 position, Quaternion idt)
        {
            if (!PoolExists(id)) return null;

            //Try to get the first on active in pool
            var currentHolder = poolHolderDictionary[id];
            var objectRequested = currentHolder.FirstActive;

            if (objectRequested != null)
            {
                ObjectPreparer.PrepareObject(objectRequested, position, idt);
                return objectRequested;
            }
            
            //Could not find available PoolObject to use! Instantiating new one if max qty not reached
            if (currentHolder.CanInstantiateMore)
                objectRequested = PoolInstantiator.Instantiate(currentHolder, position, idt);
            else
            {
                //Getting farthest Object in list to use in request
                objectRequested = currentHolder.GetObjectAtCounter();
                ObjectPreparer.PrepareObject(objectRequested, position, idt);
            }
            return objectRequested;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a local position to be activated! The rotation of the object will be the same as prefab.
        ///</summary>
        public GameObject RequestObjectWithLocalPosition(string id, Vector3 localPosition)
        {
            if (!PoolExists(id)) return null;

            //Try to get the first on active in pool
            var currentHolder = poolHolderDictionary[id];
            var objectRequested = currentHolder.FirstActive;
            
            if (objectRequested != null)
            {
                ObjectPreparer.PrepareObjectWithLocalPosition(objectRequested, localPosition);
                return objectRequested;
            }
            
            //Could not find available PoolObject to use! Instantiating new one if max qty not reached
            if (currentHolder.CanInstantiateMore)
                objectRequested = PoolInstantiator.InstantiateLocalPosition(currentHolder, localPosition);
            else
            {
                //Getting farthest Object in list to use in request
                objectRequested = currentHolder.GetObjectAtCounter();
                ObjectPreparer.PrepareObjectWithLocalPosition(objectRequested, localPosition);
            }
            return objectRequested;
        }

        private bool PoolExists(string id)
        {
            if (poolHolderDictionary.ContainsKey(id)) return true;
            Debug.LogError("Can't find object in pool list with tag: " + id);
            return false;
        }

        ///<summary>
        ///This counts how many objects are in the pool. (Activated or not).
        ///</summary>
        public int TotalObjectsInPool()
        {
            return poolHolderDictionary.Sum(poolHolderObjectPair => poolHolderObjectPair.Value.PoolCount);
        }

        ///<summary>
        ///This can be expensive to the CPU with large pools. Be careful in using this inside Updates().
        ///</summary>
        public int TotalActiveObjects()
        {
            return poolHolderDictionary.Sum(poolHolderObjectPair => poolHolderObjectPair.Value.PoolActiveCount);
        }

        ///<summary>
        ///This counts how many objects exist with a certain id.
        ///</summary>
        public int CountObjectWithID(string id)
        {
            if (poolHolderDictionary.TryGetValue(id, out var holderObject)) return holderObject.PoolCount;
            Debug.LogError("Can't find object in pool list with tag: " + id);
            return 0;
        }

        ///<summary>
        ///This counts how many objects of certain id are active.
        ///</summary>
        public int CountActivatedObjectWithID(string id)
        {
            if (poolHolderDictionary.TryGetValue(id, out var holderObject)) return holderObject.PoolActiveCount;
            Debug.LogError("Can't find object in pool list with tag: " + id);
            return 0;
        }

        ///<summary>
        ///This deletes every object with a certain id with an intent to clear some memory. Objects with this id can still be spawned again if you want.
        ///</summary>
        public void ClearPoolWithID(string id)
        {
            if (poolHolderDictionary.TryGetValue(id, out var holderObject))
            {
                holderObject.Reset();
                return;
            }
            
            Debug.LogError("Can't find object in pool list with tag: " + id);
        }

        ///<summary>
        ///This will DELETE every object in the pool. Objects can still be spawned again if you want.
        ///</summary>
        public void ClearEntirePool()
        {
            foreach (var poolHolderPair in poolHolderDictionary)
                poolHolderPair.Value.Reset();
            
            Initiate();
        }
    }
}

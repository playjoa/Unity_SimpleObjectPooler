using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JoaoMilone
{
    public class ObjectPooler : MonoBehaviour
    {
        [Header("Pool Configuration!")]

        [Tooltip("If you wish to keep objects separeted in a parent, reference it here!")]
        [SerializeField]
        private Transform pool_Parent;

        [System.Serializable]
        private class PoolObject
        {
            [Tooltip("If you want to call object by name of object it self, leave idObject blank!")]
            public string idObject = "";

            [Tooltip("The prefab reference you want to create a pool to!")]
            public GameObject prefabModel;

            [Tooltip("The max ammount of each objects in pool! If you wish to make it adaptive leave if with value 0")]
            [Range(0, 100)]
            public int max_Qty_Object = 0;

            [Tooltip("The ammount objects that you want to pre create in pool! If you wish to have these objects available from Awake!")]
            [Range(0, 20)]
            public int preMake_Qty = 0;
        }

        [Header("Object List")]
        [Tooltip("Configure your PoolObjects")]
        [SerializeField]
        private List<PoolObject> objectsList;

        protected Dictionary<string, List<GameObject>> poolDictionary;
        protected Dictionary<string, int> poolCounter;
        protected Dictionary<string, int> max_QtyOfObject;

        protected Dictionary<string, GameObject> refGameObjects;

        public static ObjectPooler Instance;

        private void Awake()
        {
            Instance = this;
            InitializeReferences();
        }

        void InitializeReferences()
        {
            poolDictionary = new Dictionary<string, List<GameObject>>();
            refGameObjects = new Dictionary<string, GameObject>();
            poolCounter = new Dictionary<string, int>();
            max_QtyOfObject = new Dictionary<string, int>();

            foreach (PoolObject p in objectsList)
            {
                List<GameObject> objectPool = new List<GameObject>();

                string auxObjectName;

                //Choosing id for the object in pool
                if (p.idObject == string.Empty)
                    auxObjectName = p.prefabModel.name;
                else
                    auxObjectName = p.idObject;

                //Adding Object to pool
                poolDictionary.Add(auxObjectName, objectPool);

                //Giving reference for game object model
                refGameObjects.Add(auxObjectName, p.prefabModel);

                //Setting max qty value for the object
                max_QtyOfObject.Add(auxObjectName, p.max_Qty_Object);

                //Setting counter to 0
                poolCounter.Add(auxObjectName, 0);
                PreMakeObjects(auxObjectName, p.preMake_Qty);              
            }
        }

        void PreMakeObjects(string id, int Qty) 
        {
            if (Qty == 0)
                return;

            for (int i = 0; i < Qty; i++)
            {
                if (poolDictionary[id].Count < max_QtyOfObject[id] || max_QtyOfObject[id] == 0)
                {
                    //Getting the reference to gameobject
                    GameObject aux = Instantiate(refGameObjects[id]);

                    //Setting object to parent if it exists
                    SetParent(aux);

                    //Adding Object to Pool
                    poolDictionary[id].Add(aux);

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
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return null;
            }

            GameObject auxGo = null;

            for (int i = 0; i < poolDictionary[id].Count; i++)
            {
      
                if (!poolDictionary[id][i].activeSelf)
                {
                    auxGo = poolDictionary[id][i];
                    PrepareObject(auxGo, tf);
                    break;
                }
            }

            if (auxGo == null)
            {
                //Could not find available PoolObject to use! Instantiating new one if max qty not reached
                if (poolDictionary[id].Count < max_QtyOfObject[id] || max_QtyOfObject[id] == 0)
                    auxGo = InstatiateInDictionary(id, tf);
                else
                {
                    //Getting farthest Object in list to use in request
                    int currentCount = poolCounter[id];

                    auxGo = poolDictionary[id][currentCount];
                    PrepareObject(auxGo, tf);
                    currentCount++;

                    if (currentCount >= poolDictionary[id].Count)
                        currentCount = 0;

                    poolCounter[id] = currentCount;
                }
            }
            return auxGo;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a position to be activated! The rotation of the object will be the same as prefab.
        ///</summary>
        public GameObject RequestObject(string id, Vector3 position)
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return null;
            }

            GameObject auxGo = null;

            for (int i = 0; i < poolDictionary[id].Count; i++)
            {

                if (poolDictionary[id][i] == null)
                {
                    Debug.LogError("Object missing in pool of id: " + id + ". Instantiating new one to replace missing object");
                    poolDictionary[id].RemoveAt(i);
                    auxGo = null;
                    break;
                }

                if (!poolDictionary[id][i].activeSelf)
                {
                    auxGo = poolDictionary[id][i];
                    PrepareObject(auxGo, position);
                    break;
                }
            }

            if (auxGo == null)
            {
                //Could not find available PoolObject to use! Instantiating new one if max qty not reached
                if (poolDictionary[id].Count < max_QtyOfObject[id] || max_QtyOfObject[id] == 0)
                    auxGo = InstatiateInDictionary(id, position);
                else
                {
                    //Getting farthest Object in list to use in request
                    int currentCount = poolCounter[id];

                    auxGo = poolDictionary[id][currentCount];
                    PrepareObject(auxGo, position);
                    currentCount++;

                    if (currentCount >= poolDictionary[id].Count)
                        currentCount = 0;

                    poolCounter[id] = currentCount;
                }
            }
            return auxGo;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a position and rotation.
        ///</summary>
        public GameObject RequestObject(string id, Vector3 position, Quaternion idt)
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return null;
            }

            GameObject auxGo = null;

            for (int i = 0; i < poolDictionary[id].Count; i++)
            {
                if (!poolDictionary[id][i].activeSelf)
                {
                    auxGo = poolDictionary[id][i];
                    PrepareObject(auxGo, position, idt);
                    break;
                }
            }

            if (auxGo == null)
            {
                //Could not find available PoolObject to use! Instantiating new one if max qty not reached
                if (poolDictionary[id].Count < max_QtyOfObject[id] || max_QtyOfObject[id] == 0)
                    auxGo = InstatiateInDictionary(id, position, idt);
                else
                {
                    //Getting farthest Object in list to use in request
                    int currentCount = poolCounter[id];

                    auxGo = poolDictionary[id][currentCount];
                    PrepareObject(auxGo, position, idt);
                    currentCount++;

                    if (currentCount >= poolDictionary[id].Count)
                        currentCount = 0;

                    poolCounter[id] = currentCount;
                }
            }
            return auxGo;
        }

        ///<summary>
        ///Requests an object from the pool and assigns a local position to be activated! The rotation of the object will be the same as prefab.
        ///</summary>
        public GameObject RequestObjectWithLocalPosition(string id, Vector3 position)
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return null;
            }

            GameObject auxGo = null;

            for (int i = 0; i < poolDictionary[id].Count; i++)
            {

                if (poolDictionary[id][i] == null)
                {
                    Debug.LogError("Object missing in pool of id: " + id + ". Instantiating new one to replace missing object");
                    poolDictionary[id].RemoveAt(i);
                    auxGo = null;
                    break;
                }

                if (!poolDictionary[id][i].activeSelf)
                {
                    auxGo = poolDictionary[id][i];
                    PrepareObjectWithLocalPosition(auxGo, position);
                    break;
                }
            }

            if (auxGo == null)
            {
                //Could not find available PoolObject to use! Instantiating new one if max qty not reached
                if (poolDictionary[id].Count < max_QtyOfObject[id] || max_QtyOfObject[id] == 0)
                    auxGo = InstatiateInDictionaryWithLocalPosition(id, position);
                else
                {
                    //Getting farthest Object in list to use in request
                    int currentCount = poolCounter[id];

                    auxGo = poolDictionary[id][currentCount];
                    PrepareObjectWithLocalPosition(auxGo, position);
                    currentCount++;

                    if (currentCount >= poolDictionary[id].Count)
                        currentCount = 0;

                    poolCounter[id] = currentCount;
                }
            }
            return auxGo;
        }

        void PrepareObject(GameObject auxObject, Transform tf)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = tf.position;
            auxObject.transform.rotation = tf.rotation;
            auxObject.SetActive(true);
        }
        
        void PrepareObject(GameObject auxObject, Vector3 position)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = position;
            auxObject.SetActive(true);
        }

        void PrepareObject(GameObject auxObject, Vector3 position, Quaternion idt)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = position;
            auxObject.transform.rotation = idt;
            auxObject.SetActive(true);
        }

        void PrepareObjectWithLocalPosition(GameObject auxObject, Vector3 localPosition)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.localPosition = localPosition;
            auxObject.SetActive(true);
        }

        //Setting object to parent if it exists
        void SetParent(GameObject objectToSetParent)
        {
            if (pool_Parent != null)
                objectToSetParent.transform.SetParent(pool_Parent);
        }

        GameObject InstatiateInDictionaryWithLocalPosition(string idSpawn, Vector3 position)
        {
            //Instantiating Object on Demand
            GameObject aux = Instantiate(refGameObjects[idSpawn]);

            PrepareObjectWithLocalPosition(aux, position);
            poolDictionary[idSpawn].Add(aux);
            SetParent(aux);
            return aux;
        }

        GameObject InstatiateInDictionary(string idSpawn, Vector3 position)
        {
            //Instantiating Object on Demand
            GameObject aux = Instantiate(refGameObjects[idSpawn]);

            PrepareObject(aux, position);
            poolDictionary[idSpawn].Add(aux);
            SetParent(aux);
            return aux;
        }

        GameObject InstatiateInDictionary(string idSpawn, Transform tf)
        {
            //Instanciando Objeto
            GameObject aux = Instantiate(refGameObjects[idSpawn]);

            PrepareObject(aux, tf);
            poolDictionary[idSpawn].Add(aux);
            SetParent(aux);
            return aux;
        }

        GameObject InstatiateInDictionary(string idSpawn, Vector3 position, Quaternion idt)
        {
            //Instanciando Objeto
            GameObject aux = Instantiate(refGameObjects[idSpawn]);

            PrepareObject(aux, position, idt);
            poolDictionary[idSpawn].Add(aux);
            SetParent(aux);
            return aux;
        }

        ///<summary>
        ///This counts how many objects are in the pool. (Activated or not).
        ///</summary>
        public int TotalObjectsInPool() 
        {
            int count = 0;

            for (int i = 0; i < poolDictionary.Count; i++)
            {
                string curID;

                if (objectsList[i].idObject == string.Empty)
                    curID = objectsList[i].prefabModel.name;
                else
                    curID = objectsList[i].idObject;

                count += CountObjectWithID(curID);
            }

            return count;
        }

        ///<summary>
        ///This can be expensive to the CPU with large pools. Be careful in using this inside Updates().
        ///</summary>
        public int TotalActiveObjects()
        {            
            int count = 0;

            for (int i = 0; i < poolDictionary.Count; i++)
            {
                string curID;

                if (objectsList[i].idObject == string.Empty)
                    curID = objectsList[i].prefabModel.name;
                else
                    curID = objectsList[i].idObject;

                count += CountActivatedObjectWithID(curID);
            }

            return count;
        }

        ///<summary>
        ///This counts how many objects exist with a certain id.
        ///</summary>
        public int CountObjectWithID(string id)
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return 0;
            }

            if (poolDictionary[id].Count == 0)
                return 0;

            return poolDictionary[id].Count;
        }

        ///<summary>
        ///This counts how many objects of certain id are active.
        ///</summary>
        public int CountActivatedObjectWithID(string id)
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return 0;
            }

            if (poolDictionary[id].Count == 0)
                return 0;

            int count = 0;

            for (int i = 0; i < poolDictionary[id].Count; i++)
            {
                if (poolDictionary[id][i] != null)
                {
                    if (poolDictionary[id][i].activeSelf)
                        count++;
                }
            }

            return count;
        }

        ///<summary>
        ///This deletes every object with a certain id with an intent to clear some memory. Objects with this id can still be spawned again if you want.
        ///</summary>
        public void ClearPoolWithID(string id) 
        {
            if (!poolDictionary.ContainsKey(id))
            {
                Debug.LogError("Can't find object in pool list with tag: " + id);
                return;
            }

            if(poolDictionary[id].Count == 0)
            {
                Debug.LogWarning("This pool is already clean: " + id);
                return;
            }

            for (int i = 0; i < poolDictionary[id].Count; i++)
                Destroy(poolDictionary[id][i]);  

            poolDictionary[id].Clear();
            poolCounter[id] = 0;
        }

        ///<summary>
        ///This will DELETE every object in the pool. Objects can still be spawned again if you want.
        ///</summary>
        public void ClearEntirePool()
        {
            for (int i = 0; i < poolDictionary.Count; i++)
            {
                string curID;

                if (objectsList[i].idObject == string.Empty)
                    curID = objectsList[i].prefabModel.name;
                else
                    curID = objectsList[i].idObject;

                for (int y = 0; y < poolDictionary[curID].Count; y++)
                {
                    if (poolDictionary[curID][y] != null)
                    {
                        Destroy(poolDictionary[curID][y]);
                    }
                }
            }
            InitializeReferences();
        }
    }
}

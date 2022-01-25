using UnityEngine;

namespace JoaoMilone.Pooler.Components
{
    [System.Serializable]
    public class PoolObject
    {
        [Tooltip("If you want to call object by name of object it self, leave idObject blank!")]
        [SerializeField] private string idObject = "";

        [Tooltip("The prefab reference you want to create a pool to!")]
        [SerializeField] private GameObject prefabModel;

        [Tooltip("The max amount of each objects in pool! If you wish to make it adaptive leave if with value 0")]
        [Range(0, 100)]
        [SerializeField] private int max_Qty_Object = 0;

        [Tooltip("The amount objects that you want to pre create in pool! If you wish to have these objects available from Awake!")]
        [Range(0, 20)]
        [SerializeField] private int preMake_Qty = 0;

        public string Id => idObject == string.Empty ? prefabModel.name : idObject;
        public GameObject Model => prefabModel;
        public int MaxQty => max_Qty_Object;
        public int PreMakeQty => preMake_Qty;
    }
}
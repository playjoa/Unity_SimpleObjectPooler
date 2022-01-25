using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JoaoMilone.Pooler.Components
{
    public class PoolHolderObject
    {
        public string Id { get; private set; }
        public int Counter { get; private set; }
        public int MaxQtyAllowed { get; private set; }
        public int PreMakeQty { get; private set; }
        public GameObject GameObjectPrefab { get; private set; }
        public List<GameObject> PoolList { get; private set; }
        public int PoolCount => PoolList.Count;
        public int PoolActiveCount => PoolList.Count(g => g.activeSelf);
        public GameObject FirstActive => PoolList.FirstOrDefault(o => !o.activeSelf);
        public bool CanInstantiateMore => PoolCount < MaxQtyAllowed || MaxQtyAllowed == 0;

        public PoolHolderObject(PoolObject poolObjectToHold)
        {
            Id = poolObjectToHold.Id;
            MaxQtyAllowed = poolObjectToHold.MaxQty;
            PreMakeQty = poolObjectToHold.PreMakeQty;
            GameObjectPrefab = poolObjectToHold.Model;
            Counter = 0;
            PoolList = new List<GameObject>();
        }

        public void ReceiveObject(GameObject gameObject) => PoolList.Add(gameObject);

        public GameObject GetObjectAtCounter()
        {
            var objectRequested = PoolList[Counter];
            Count();
            return objectRequested;
        }

        private void Count()
        {
            Counter++;
            if (Counter >= PoolCount) Counter = 0;
        }

        public void Reset()
        {
            foreach (var poolObject in PoolList.Where(poolObject => poolObject != null))
                Object.Destroy(poolObject);
            
            Counter = 0;
            PoolList.Clear();
        }
    }
}
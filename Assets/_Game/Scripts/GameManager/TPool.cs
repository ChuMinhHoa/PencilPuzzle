using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Game.Scripts.GameManager
{
    [Serializable]
    public class TPool<T> where T : Component
    {
        public T TPrefab;
        public Transform parentTrs;
        public List<T> Pool =new();
        public List<T> DisableT = new();
        
        [BoxGroup("TPool")]
        [Button]
        public void Spawn(int countSpawn)
        {
            for (var i = 0; i < countSpawn; i++)
            {
                GetTObj();
            }
        }
        
        [BoxGroup("TPool")]
        [Button]
        public T Spawn()
        {
            return GetTObj();
        }

        private T GetTObj()
        {
            if (DisableT.Count > 0)
            {
                var temp = DisableT[0];
                Pool.Add(temp);
                temp.gameObject.SetActive(true);
                DisableT.RemoveAt(0);
                return temp;
            }

            var newT = Object.Instantiate(TPrefab, parentTrs);
            Pool.Add(newT);
            return newT;
        }
        
        [BoxGroup("TPool")]
        [Button]
        public void Despawn(T t)
        {
            if (Pool.Contains(t))
            {
                Pool.Remove(t);
                DisableT.Add(t);
                t.gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("Trying to despawn an object that is not in the pool.");
            }
        }
    }
}

using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj
{
    [System.Serializable]
    public class PointHaveObj<T>
    {
        public T objOnPoint;
        public Transform trsPoint;

        public virtual bool isFree => objOnPoint == null;

        public void SetObjOnPoint(T obj) => objOnPoint = obj;
        
        public virtual void ResetObjOnPoint() => objOnPoint = default(T);
    }
}

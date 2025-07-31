using System.Collections.Generic;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class PencilUnderOther : PencilBase
    {
        [Title("====Unit Under Other==")]
        public List<int> unitIdLockThat;
        public List<int> unitResolved;
        
        public override void TryMoveOut()
        {
            if (unitResolved.Count == unitIdLockThat.Count)
                base.TryMoveOut();
            else
            {
                Debug.Log("Need animation warning!");
                GameManager.Instance.CheckCanTouch();
            }
        }
        
        public void AddUnitIdLock(int unitIdAdd)
        {
            if (!unitIdLockThat.Contains(unitIdAdd))
            {
                unitIdLockThat.Add(unitIdAdd);
            }
        }
        
        public void RemoveUnitIdLock(int unitIdRemove)
        {
            if (unitIdLockThat.Contains(unitIdRemove))
            {
                unitIdLockThat.Remove(unitIdRemove);
            }
        }
        
        public void UnitResolve(int id)
        {
            Debug.Log("resolve unit: " + id);
            if (unitIdLockThat.Contains(id) && !unitResolved.Contains(id))
                unitResolved.Add(id);
        }

        public override void Reset()
        {
            base.Reset();
            unitResolved.Clear();
        }
    }
}

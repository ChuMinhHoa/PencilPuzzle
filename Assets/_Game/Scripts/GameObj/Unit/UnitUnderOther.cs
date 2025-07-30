using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class UnitUnderOther : UnitBase
    {
        [Title("====Unit Under Other==")]
        public List<int> unitIdLockThat;
        
        public override void TryMoveOut()
        {
            if (unitIdLockThat.Count == 0)
                base.TryMoveOut();
            else Debug.Log("Need animation warning!");
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
    }
}

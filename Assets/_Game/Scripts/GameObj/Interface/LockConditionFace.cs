using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public class LockConditionFace : ConditionFace
    {
        public List<int> unitIdLockThat = new();
        public List<int> unitIdResolve = new();

        public override bool IsConditionSatisfied()
        {
            return true;
        }

        public void AddUnitIdResolve(int unitId)
        {
            if (unitIdLockThat.Contains(unitId) && !unitIdResolve.Contains(unitId))
            {
                unitIdResolve.Add(unitId);
            }
        }
    }
}

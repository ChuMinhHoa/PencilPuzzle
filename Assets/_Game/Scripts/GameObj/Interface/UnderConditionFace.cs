using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public class UnderConditionFace : ConditionFace
    {
        public List<int> unitIdLockThat = new();
        public List<int> unitIdResolve = new();

        public override bool IsConditionSatisfied()
        {
            return unitIdLockThat.Count == unitIdResolve.Count;
        }

        public override void ResetCondition()
        {
            unitIdResolve.Clear();
        }


        protected override void AddProperty(Memory<object> args)
        {
            var unitId = (int)args.Span[0];
            if (unitIdLockThat.Contains(unitId) && !unitIdResolve.Contains(unitId))
            {
                unitIdResolve.Add(unitId);
            }
        }
    }
}

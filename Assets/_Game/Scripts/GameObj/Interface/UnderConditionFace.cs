using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager.Etc;
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

        public override void InitCondition()
        {
            ResetCondition();
            GameGlobalEvent.OnPencilResolve += ResolvePencil;
        }

        private void ResolvePencil(PencilBase pencilBase)
        {
            var unitId = pencilBase.unitId;
            if (!unitIdResolve.Contains(unitId) && unitIdLockThat.Contains(unitId))
            {
                unitIdResolve.Add(unitId);
            }
        }

        private void OnDisable()
        {
            GameGlobalEvent.OnPencilResolve -= ResolvePencil;
        }
    }
}

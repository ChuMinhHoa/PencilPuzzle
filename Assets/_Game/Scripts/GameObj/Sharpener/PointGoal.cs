using System;
using _Game.Scripts.GameObj.Unit;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class PointGoal : MonoBehaviour
    {
        public UnitBase currentUnit;
        private Action _actionCallBack;

        public void SetActionCallBack(Action action) => _actionCallBack = action;
        [field: SerializeField] public Transform pointGoal { get; set; }

        public void SetUnit(UnitBase unit)
        {
            currentUnit = unit;
        }
        
        public bool IsFree()
        {
            return currentUnit == null;
        }

        public void ClearPointGoal()
        {
            currentUnit = null;
        }

        public void OnCheckClearSharpener()
        {
            _actionCallBack?.Invoke();
        }
    }
}

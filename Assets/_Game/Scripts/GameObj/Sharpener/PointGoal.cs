using _Game.Scripts.GameObj.Unit;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class PointGoal : MonoBehaviour
    {
        public UnitBase currentUnit;

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
    }
}

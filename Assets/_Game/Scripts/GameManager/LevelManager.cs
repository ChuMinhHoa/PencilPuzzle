using _Game.Scripts.GameManager.Controller;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.GameManager
{
    public class LevelManager : Singleton<LevelManager>
    {
        public SharpenerController sharpenerController;
        public UnitController unitController;

        public bool TryResolveUnit(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetSharpener(unitBase.colorType);
            if (sharpener == null)
            {
                Debug.Log("null sharpener");
                return false;
            }

            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                Debug.Log("null point goal");
                return TryResolveToTemp(unitBase);
            }
            
            ResolveDone(pointGoal, unitBase);
            return true;
        }

        private bool TryResolveToTemp(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetTempSharpener();
            if (sharpener == null)
                return false;
            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                Debug.Log("null point goal");
                return false;
            }

            ResolveDone(pointGoal, unitBase);
            return true;
        }

        private void ResolveDone(PointGoal pointGoal, UnitBase unitBase)
        {
            pointGoal.SetUnit(unitBase);
            Debug.Log("point goal set for unit: " + unitBase.name);
#if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = unitBase.gameObject;
#endif
        }
    }
}

using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.Manager.Controller
{
    public class UnitController : MonoBehaviour
    {
        public List<UnitBase> units = new();
        public List<int> unitTemps = new();
        public async UniTask InitData()
        {
            unitTemps.Clear();
            for (var i = 0; i < units.Count; i++)
            {
                units[i].gameObject.SetActive(true);
                units[i].InitData();
                await UniTask.WaitForEndOfFrame();
            }
        }

        public UnitBase GetUnitById(int id)
        {
            for (var i = 0; i < units.Count; i++)
            {
                if(units[i].unitId == id)
                    return units[i];
            }

            return null;
        }

        public UnitBase GetUnitByCollider(Collider colliderCheck)
        {
            for (var i = 0; i < units.Count; i++)
            {
                if (units[i].IsHaveThatCollider(colliderCheck))
                    return units[i];
            }
            return null;
        }

        public void AddUnitToTemp(int unitId)
        {
            unitTemps.Add(unitId);
        }

        private void CheckColorUnitTemps(Sharpener sharpener)
        {
            for (var i = unitTemps.Count - 1; i >= 0; i--)
            {
                var unitBase = GetUnitById(unitTemps[i]);
                if(!unitBase) continue;
                if (unitBase.colorType == sharpener.sharpenerColorType)
                {
                    var pointGoal = sharpener.TryGetPointGoal();
                    if(!pointGoal) continue;
                    pointGoal.SetUnit(unitBase.unitId);
                    unitBase.AnimForUnitTemp(pointGoal, sharpener.id);
                    unitTemps.RemoveAt(i);
                }
            }
           
        }

        public void CheckUnitTemps(List<Sharpener> currentSharpeners)
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                CheckColorUnitTemps(currentSharpeners[i]);
            }
        }

        public void ResetAllUnits()
        {
            for (var i = 0; i < units.Count; i++)
            {
                units[i].ResetUnit();
            }
        }

        public bool CheckCanTouch()
        {
            for (var i = 0; i < units.Count; i++)
            {
                if (!units[i].CheckCanTouch())
                    return false;
            }

            return true;
        }

        public void AddUnit(UnitBase unit)
        {
            units.Add(unit);
        }

        public void RemoveUnit(UnitBase unit)
        {
            units.Remove(unit);
        }
    }
}

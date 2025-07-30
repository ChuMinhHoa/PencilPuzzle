using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Manager.Controller
{
    public class PencilController : MonoBehaviour
    {
        public List<PencilBase> pencils = new();
        public List<int> pencilTemps = new();
        public async UniTask InitData()
        {
            pencilTemps.Clear();
            for (var i = 0; i < pencils.Count; i++)
            {
                pencils[i].gameObject.SetActive(true);
                pencils[i].InitData();
                await UniTask.WaitForEndOfFrame();
            }
        }

        public PencilBase GetUnitById(int id)
        {
            for (var i = 0; i < pencils.Count; i++)
            {
                if(pencils[i].unitId == id)
                    return pencils[i];
            }

            return null;
        }

        public PencilBase GetUnitByCollider(Collider colliderCheck)
        {
            for (var i = 0; i < pencils.Count; i++)
            {
                if (pencils[i].IsHaveThatCollider(colliderCheck))
                    return pencils[i];
            }
            return null;
        }

        public void AddUnitToTemp(int unitId)
        {
            pencilTemps.Add(unitId);
        }

        private void CheckColorUnitTemps(Sharpener sharpener)
        {
            for (var i = pencilTemps.Count - 1; i >= 0; i--)
            {
                var unitBase = GetUnitById(pencilTemps[i]);
                if(!unitBase) continue;
                if (unitBase.colorType == sharpener.sharpenerColorType)
                {
                    var pointGoal = sharpener.TryGetPointGoal();
                    if(!pointGoal) continue;
                    pointGoal.SetUnit(unitBase.unitId);
                    unitBase.AnimForUnitTemp(pointGoal, sharpener.id);
                    pencilTemps.RemoveAt(i);
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
            for (var i = 0; i < pencils.Count; i++)
            {
                pencils[i].Reset();
            }
        }

        public bool CheckCanTouch()
        {
            for (var i = 0; i < pencils.Count; i++)
            {
                if (!pencils[i].CheckCanTouch())
                    return false;
            }

            return true;
        }

        public void AddUnit(PencilBase unit)
        {
            pencils.Add(unit);
        }

        public void RemoveUnit(PencilBase unit)
        {
            pencils.Remove(unit);
        }
    }
}

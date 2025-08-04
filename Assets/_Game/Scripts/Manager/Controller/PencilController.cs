using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Manager.Controller
{
    public class PencilController : MonoBehaviour
    {
        public List<PencilBase> pencils = new();
        public List<int> pencilTemps = new();
        public async UniTask InitData(Action<float> onProgress = null)
        {
            pencilTemps.Clear();
            for (var i = 0; i < pencils.Count; i++)
            {
                pencils[i].gameObject.SetActive(true);
                pencils[i].InitData();
                await UniTask.WaitForEndOfFrame();
                onProgress?.Invoke((i + 1) / (float)pencils.Count);
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
                    if(pointGoal == null) continue;
                    pointGoal.SetObjOnPoint(unitBase.unitId);
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

        public void SaveConfig(LevelConfig currentLevelConfig)
        {
            for (var i = 0; i < pencils.Count; i++)
            {
                Debug.Log($"Saving data for Unit ID: {pencils[i].unitId}, Color: {pencils[i].colorType}");

                var pMesh = GetPathPoint(pencils[i]);
                var pOut = GetPathPointOut(pencils[i]);
            
                currentLevelConfig.SaveUnitData(
                    pencils[i].unitId,
                    pencils[i].colorType,
                    pMesh,
                    pOut
                );
            }
        }
        
        #region path point

        private List<float3> GetPathPoint(PencilBase pencilBase)
        {
            var path = new List<float3>();
            for (var i = 0; i < pencilBase.splineController.spline.nodes.Count; i++)
            {
                var point = pencilBase.splineController.spline.nodes[i].Position;
                path.Add(point);
            }

            return path;
        }
        
        private List<float3> GetPathPointFollowCurrentPencilTransform(PencilBase pencilBase)
        {
            var path = new List<float3>();
            for (var i = 0; i < pencilBase.splineController.spline.nodes.Count; i++)
            {
                var point = pencilBase.transform.TransformPoint(pencilBase.splineController.spline.nodes[i].Position);
                path.Add(point);
            }

            return path;
        }

        private List<float3> GetPathPointOut(PencilBase pencilBase)
        {
            var path = new List<float3>();
            for (var i = 0; i < pencilBase.splineController.splineOut.splineOut.nodes.Count; i++)
            {
                var point = pencilBase.splineController.splineOut.splineOut.nodes[i].Position;
                path.Add(point);
            }
            return path;
        }

        #endregion

        public void AddPropertyCondition(ConditionType conditionType, params object[] args)
        {
            for (var i = 0; i < pencils.Count; i++)
            {
                pencils[i].unitConditionController.AddPropertyCondition(conditionType, args);
            }
        }
    }
}

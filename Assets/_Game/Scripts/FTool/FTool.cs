using System.Collections.Generic;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.FTool
{
    public class FTool : MonoBehaviour
    {
        [BoxGroup("Create Material")]
        public List<Texture> spritesMaterialCreate = new();

        [BoxGroup("Create Material")] 
        public string materialPath = "Assets/Materials/";
        [BoxGroup("Create Material")]
        [Button("Create Material", 50)]
        private void CreateMaterialFollowTexture()
        {
            foreach (var sprite in spritesMaterialCreate)
            {
                if (sprite == null) continue;
                var material = new Material(Shader.Find("Standard"));
                material.mainTexture = sprite;
                material.name = sprite.name;
                AssetDatabase.CreateAsset(material, $"{materialPath}{sprite.name}.mat");
            }
        }

        [BoxGroup("Unit")] public LevelManager levelManager;
        [BoxGroup("Unit")] public List<PencilBase> pencilPrefab;
        [BoxGroup("Unit")] public Transform pencilParents;
        [BoxGroup("Unit")] public PencilBase pencilClone;
        [BoxGroup("Unit")] public PencilBase currentPencilUnit;
        [BoxGroup("Unit")] public PencilType pencilType;
        [BoxGroup("Unit")]
        [ShowIf("@this.currentPencilUnit != null")]
        public List<float3> currentPathMesh;
        [BoxGroup("Unit")]
        [Button("Create Unit", 50)]
        private void CreateNewUnit()
        {
            if (levelManager == null)
            {
                Debug.Log("Level Manager is not assigned.");
                return;
            }
            var newUnit =  PrefabUtility.InstantiatePrefab(pencilPrefab[(int)pencilType], pencilParents) as PencilBase;
            if (newUnit == null)
            {
                Debug.Log("New Unit is null.");
                return;
            }
            levelManager.pencilController.pencils.Add(newUnit);
            
            newUnit.unitId = levelManager.pencilController.pencils.Count;
            newUnit.name = "UnitBase_" + (newUnit.unitId -1);
            currentPencilUnit = newUnit;
            
        }

        [BoxGroup("Unit")]
        [Button("Clone Unit", 50)]
        private void CloneUnit()
        {
            currentPencilUnit.unitId = pencilClone.unitId;
            currentPencilUnit.InitData();
            currentPencilUnit.unitId = levelManager.pencilController.pencils.Count;
        }
        [BoxGroup("Unit")]
        [Button("Edit Path Unit", 50)]
        private void EditPathUnit()
        {
            currentPathMesh = levelManager.GetUnitPositionConfig(currentPencilUnit.unitId).pathMesh;
        }
        [BoxGroup("Unit")]
        [Button("Apply Path Unit", 50)]
        private void ApplyPathUnit()
        {
            levelManager.currentLevelConfig.SaveData();
        }
        
        [BoxGroup("Unit")]
        [Button("Preset Way Out", 50)]
        private void PresetWayOut()
        {
            currentPencilUnit.trsWayOut.position = currentPencilUnit.splineController.splineOut.splineOut.nodes[0].Position;
        }
        
        [BoxGroup("Unit")]
        [Button("Save Path", 50)]
        private void SavePath()
        {
            Debug.Log($"Saving data for Unit ID: {currentPencilUnit.unitId}, Color: {currentPencilUnit.colorType}");
            var levelConfig = GameManager.Instance.currentLevelManager.currentLevelConfig;
            if (levelConfig == null)
            {
                Debug.LogError("Current level config is null. Cannot save unit data.");
                return;
            }

            var pMesh = GetPathPointFollowCurrentPencilTransform();
            var pOut = GetPathPointOut();
            levelConfig.SaveUnitData(
                currentPencilUnit.unitId,
                currentPencilUnit.colorType,
                pMesh,
                pOut
            );
        }

        [BoxGroup("Unit")]
        [Button("Save Unit", 50)]
        private void SaveUnit()
        {Debug.Log($"Saving data for Unit ID: {currentPencilUnit.unitId}, Color: {currentPencilUnit.colorType}");
            var levelConfig = GameManager.Instance.currentLevelManager.currentLevelConfig;
            if (levelConfig == null)
            {
                Debug.LogError("Current level config is null. Cannot save unit data.");
                return;
            }

            var pMesh = GetPathPoint();
            var pOut = GetPathPointOut();
          
            
            levelConfig.SaveUnitData(
                currentPencilUnit.unitId,
                currentPencilUnit.colorType,
                pMesh,
                pOut
            );
        }

        #region path point

        private List<float3> GetPathPoint()
        {
            var path = new List<float3>();
            for (var i = 0; i < currentPencilUnit.splineController.spline.nodes.Count; i++)
            {
                var point = currentPencilUnit.splineController.spline.nodes[i].Position;
                path.Add(point);
            }

            return path;
        }
        
        private List<float3> GetPathPointFollowCurrentPencilTransform()
        {
            var path = new List<float3>();
            for (var i = 0; i < currentPencilUnit.splineController.spline.nodes.Count; i++)
            {
                var point = currentPencilUnit.transform.TransformPoint(currentPencilUnit.splineController.spline.nodes[i].Position);
                path.Add(point);
            }

            return path;
        }

        private List<float3> GetPathPointOut()
        {
            var path = new List<float3>();
            for (var i = 0; i < currentPencilUnit.splineController.splineOut.splineOut.nodes.Count; i++)
            {
                var point = currentPencilUnit.splineController.splineOut.splineOut.nodes[i].Position;
                path.Add(point);
            }
            return path;
        }

        #endregion
        
    }
}

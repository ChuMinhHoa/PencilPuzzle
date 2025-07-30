using System.Collections.Generic;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

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
        [BoxGroup("Unit")] public List<UnitBase> unitPrefab;
        [BoxGroup("Unit")] public Transform unitParent;
        [BoxGroup("Unit")] public UnitBase unitClone;
        [BoxGroup("Unit")] public UnitBase currentUnit;
        [BoxGroup("Unit")] public UnitType unitType;
        [BoxGroup("Unit")]
        [ShowIf("@this.currentUnit != null")]
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
            var newUnit =  PrefabUtility.InstantiatePrefab(unitPrefab[(int)unitType], unitParent) as UnitBase;
            if (newUnit == null)
            {
                Debug.Log("New Unit is null.");
                return;
            }
            levelManager.unitController.units.Add(newUnit);
            
            newUnit.unitId = levelManager.unitController.units.Count;
            newUnit.name = "UnitBase_" + (newUnit.unitId -1);
            currentUnit = newUnit;
            
        }

        [BoxGroup("Unit")]
        [Button("Clone Unit", 50)]
        private void CloneUnit()
        {
            currentUnit.unitId = unitClone.unitId;
            currentUnit.InitDataEditor();
            currentUnit.unitId = levelManager.unitController.units.Count;
        }
        [BoxGroup("Unit")]
        [Button("Edit Path Unit", 50)]
        private void EditPathUnit()
        {
            currentPathMesh = levelManager.GetUnitPositionConfig(currentUnit.unitId).pathMesh;
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
            currentUnit.trsWayOut.position = currentUnit.splineOut.splineOut.nodes[0].Position;
        }
        
        [BoxGroup("Unit")]
        [Button("Save Path", 50)]
        private void SavePath()
        {
            Debug.Log($"Saving data for Unit ID: {currentUnit.unitId}, Color: {currentUnit.colorType}");
            var levelConfig = GameManager.Instance.currentLevelManager.currentLevelConfig;
            if (levelConfig == null)
            {
                Debug.LogError("Current level config is null. Cannot save unit data.");
                return;
            }
            var pMesh = new List<float3>();
            var pOut = new List<float3>();
            for (var i = 0; i < currentUnit.spline.nodes.Count; i++)
            {
                var point = currentUnit.transform.TransformPoint(currentUnit.spline.nodes[i].Position);
                pMesh.Add(point);
            }
            
            for (var i = 0; i < currentUnit.splineOut.splineOut.nodes.Count; i++)
            {
                var point = currentUnit.splineOut.splineOut.nodes[i].Position;
                pOut.Add(point);
            }
            
            levelConfig.SaveUnitData(
                currentUnit.unitId,
                currentUnit.colorType,
                pMesh,
                pOut
            );
        }

        [BoxGroup("Unit")]
        [Button("Save Unit", 50)]
        private void SaveUnit()
        {Debug.Log($"Saving data for Unit ID: {currentUnit.unitId}, Color: {currentUnit.colorType}");
            var levelConfig = GameManager.Instance.currentLevelManager.currentLevelConfig;
            if (levelConfig == null)
            {
                Debug.LogError("Current level config is null. Cannot save unit data.");
                return;
            }
            var pMesh = new List<float3>();
            var pOut = new List<float3>();
            for (var i = 0; i < currentUnit.spline.nodes.Count; i++)
            {
                var point = currentUnit.spline.nodes[i].Position;
                pMesh.Add(point);
            }
            
            for (var i = 0; i < currentUnit.splineOut.splineOut.nodes.Count; i++)
            {
                var point = currentUnit.splineOut.splineOut.nodes[i].Position;
                pOut.Add(point);
            }
            
            levelConfig.SaveUnitData(
                currentUnit.unitId,
                currentUnit.colorType,
                pMesh,
                pOut
            );
        }
    }
}

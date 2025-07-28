using System.Collections.Generic;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Unit;
using Sirenix.OdinInspector;
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
        [BoxGroup("Unit")] public UnitBase unitPrefab;
        [BoxGroup("Unit")] public Transform unitParent;
        [BoxGroup("Unit")] public UnitBase unitClone;
        [BoxGroup("Unit")] public UnitBase currentUnit;
        [BoxGroup("Unit")]
        [Button("Create Unit", 50)]
        private void CreateNewUnit()
        {
            if (levelManager == null)
            {
                Debug.Log("Level Manager is not assigned.");
                return;
            }
            var newUnit =  PrefabUtility.InstantiatePrefab(unitPrefab, unitParent) as UnitBase;
            if (newUnit == null)
            {
                Debug.Log("New Unit is null.");
                return;
            }
            newUnit.unitId = levelManager.unitController.units.Count;
            levelManager.unitController.units.Add(newUnit);
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
    }
}

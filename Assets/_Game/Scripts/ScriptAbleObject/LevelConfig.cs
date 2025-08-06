using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager;
using Sirenix.OdinInspector;
using SplineMesh;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Game.Scripts.ScriptAbleObject
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int level;
        public List<UnitPositionConfig> unitPositionConfig = new();
        public List<SharpenerColorType> sharpenerColors = new();
        public int startSharpenerCount;
        public AssetReference levelPrefabReference;
        public float timeDuration = 60f;
        public Vector3 cameraPosition;
        public float cameraSize = 5f;
        public UnitPositionConfig GetLevelUnitPositionConfig(int unitId)
        {
            return unitPositionConfig.Find(config => config.unitId == unitId);
        }

        public void SaveUnitData(
            int unitId,
            SharpenerColorType colorType,
            List<float3> splineNodes,
            List<float3> wayOutNodes,
            Vector3 wayOutPosition,
            float wayOutRotationInit
            )
        {
            for (var i = 0; i < unitPositionConfig.Count; i++)
            {
                if (unitPositionConfig[i].unitId == unitId)
                {
                    unitPositionConfig[i].SaveData(
                        colorType, 
                        splineNodes,
                        wayOutNodes,
                        wayOutPosition,
                        wayOutRotationInit
                    );
                    
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssetIfDirty(this);
#endif
                    return;
                }
            }
            
            var newUnitConfig = new UnitPositionConfig();
            newUnitConfig.unitId = unitId;
            newUnitConfig.SaveData(
                colorType, 
                splineNodes,
                wayOutNodes,
                wayOutPosition,
                wayOutRotationInit
            );
            unitPositionConfig.Add(newUnitConfig);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }  
        
#if UNITY_EDITOR
        [Button]
        public void InitData(int levelChange)
        {
            level = levelChange;
            unitPositionConfig.Clear();
            var path = $"Assets/_Game/Prefab/Level/Level_{levelChange}.prefab";
            levelPrefabReference = new AssetReference(AssetDatabase.AssetPathToGUID(path));
        }
#endif
        public void SaveData()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public SharpenerColorType GetColorNext(int currentWaveIndex)
        {
            if (currentWaveIndex < 0 || currentWaveIndex >= sharpenerColors.Count)
            {
                return SharpenerColorType.None;
            }
            return sharpenerColors[currentWaveIndex];
        }
    }

    [System.Serializable]
    public class UnitPositionConfig
    {
        public int unitId;
        public SharpenerColorType unitColor;
        public List<float3> pathMesh = new();
        public List<float3> wayOut = new();
        public Vector3 wayOutPosition;
        public float wayOutRotation;
        public void SaveData(SharpenerColorType colorType,
            List<float3> splineNodes,
            List<float3> wayOutNodes,
            Vector3 wayOutPositionInit, 
            float wayOutRotationInit)  
        {
            unitColor = colorType;

            pathMesh.Clear();
            wayOut.Clear();
            
            for (var i = 0; i < splineNodes.Count; i++)
            {
                pathMesh.Add(splineNodes[i]);
            }
            
            for (var i = 0; i < wayOutNodes.Count; i++)
            {
                wayOut.Add(wayOutNodes[i]);
            }
            wayOutPosition = wayOutPositionInit;
            wayOutRotation = wayOutRotationInit;
        }
    }
}
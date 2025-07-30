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
        public List<UnitPositionConfig> unitPositionConfig;
        public List<WaveConfig> waveConfig;
        public AssetReference levelPrefabReference;
        public UnitPositionConfig GetLevelUnitPositionConfig(int unitId)
        {
            return unitPositionConfig.Find(config => config.unitId == unitId);
        }

        public void SaveUnitData(
            int unitId,
            SharpenerColorType colorType,
            List<float3> splineNodes,
            List<float3> wayOutNodes
            )
        {
            for (var i = 0; i < unitPositionConfig.Count; i++)
            {
                if (unitPositionConfig[i].unitId == unitId)
                {
                    unitPositionConfig[i].SaveData(
                        colorType, 
                        splineNodes,
                        wayOutNodes
                    );
                    return;
                }
            }
            
            var newUnitConfig = new UnitPositionConfig();
            newUnitConfig.unitId = unitId;
            newUnitConfig.SaveData(
                colorType, 
                splineNodes,
                wayOutNodes
            );
            unitPositionConfig.Add(newUnitConfig);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public WaveConfig GetWaveConfig(int currentWaveIndex)
        {
            if (currentWaveIndex >= waveConfig.Count)
                return null;
            return waveConfig[currentWaveIndex];
        }
        [Button]
        private void InitData()
        {
            unitPositionConfig.Clear();
            waveConfig.Clear();
            var path = $"Assets/_Game/Prefab/Level/Level_{level}.prefab";
            levelPrefabReference = new AssetReference(AssetDatabase.AssetPathToGUID(path));
        }

        public void SaveData()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }
    }

    [System.Serializable]
    public class UnitPositionConfig
    {
        public int unitId;
        public SharpenerColorType unitColor;
        public List<float3> pathMesh = new();
        public List<float3> wayOut = new();
        public void SaveData(SharpenerColorType colorType,
            List<float3> splineNodes,
            List<float3> wayOutNodes)
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
        }
    }
    
    [System.Serializable]
    public class WaveConfig
    {
        public int waveId;
        public List<SharpenerColorType> sharpenerColors;
    }
}
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using SplineMesh;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.ScriptAbleObject
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int level;
        public List<UnitPositionConfig> unitPositionConfig;
        public List<WaveConfig> waveConfig;
        public GameObject levelPrefab;
        
        public UnitPositionConfig GetLevelUnitPositionConfig(int unitId)
        {
            return unitPositionConfig.Find(config => config.unitId == unitId);
        }

        public void SaveUnitData(
            int unitId,
            UnitLengthType unitLength,
            SharpenerColorType colorType,
            List<SplineNode> splineNodes
            )
        {
            for (var i = 0; i < unitPositionConfig.Count; i++)
            {
                if (unitPositionConfig[i].unitId == unitId)
                {
                    unitPositionConfig[i].SaveData(
                        colorType, 
                        unitLength, 
                        splineNodes
                    );
                    return;
                }
            }
            
            var newUnitConfig = new UnitPositionConfig();
            newUnitConfig.unitId = unitId;
            newUnitConfig.SaveData(
                colorType, 
                unitLength, 
                splineNodes
            );
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
#endif
        }

        public WaveConfig GetWaveConfig(int currentWaveIndex)
        {
            return waveConfig[currentWaveIndex];
        }
    }

    [System.Serializable]
    public class UnitPositionConfig
    {
        public int unitId;
        public SharpenerColorType unitColor;
        public UnitLengthType unitLength;
        public List<float3> pathMesh = new();

        public void SaveData(SharpenerColorType colorType,
            UnitLengthType lengthType,
            List<SplineNode> splineNodes)
        {
            unitColor = colorType;
            unitLength = lengthType;

            pathMesh.Clear();
            
            for (var i = 0; i < splineNodes.Count; i++)
            {
                pathMesh.Add(splineNodes[i].Position);
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
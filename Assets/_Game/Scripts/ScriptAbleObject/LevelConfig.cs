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
        public GameObject levelPrefab;

        public UnitPositionConfig GetLevelUnitPositionConfig(int unitId)
        {
            return unitPositionConfig.Find(config => config.unitId == unitId);
        }

        public void SaveUnitData(
            int unitId,
            UnitLengthType unitLength,
            SharpenerColorType colorType,
            List<SplineNode> splineNodes,
            float3 transformEulerAngles, 
            float3 position)
        {
            for (var i = 0; i < unitPositionConfig.Count; i++)
            {
                if (unitPositionConfig[i].unitId == unitId)
                {
                    unitPositionConfig[i].SaveData(
                        colorType, 
                        unitLength, 
                        splineNodes, 
                        transformEulerAngles, 
                        position
                    );
                    return;
                }
            }
            
            var newUnitConfig = new UnitPositionConfig();
            newUnitConfig.unitId = unitId;
            newUnitConfig.SaveData(
                colorType, 
                unitLength, 
                splineNodes, 
                transformEulerAngles, 
                position
            );
            
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
        public UnitLengthType unitLength;
        public float3 position;
        public float3 eulerAngles;
        public List<float3> pathMesh = new();

        public void SaveData(SharpenerColorType colorType,
            UnitLengthType lengthType,
            List<SplineNode> splineNodes,
            float3 newEulerAngles,
            float3 unitPosition)
        {
            unitColor = colorType;
            unitLength = lengthType;
            position = splineNodes[0].Position;
            eulerAngles = newEulerAngles;
            position = unitPosition;

            pathMesh.Clear();
            
            for (var i = 0; i < splineNodes.Count; i++)
            {
                pathMesh.Add(splineNodes[i].Position);
            }
        }
    }
}
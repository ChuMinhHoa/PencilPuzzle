using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.GlobalConfig
{
    [CreateAssetMenu(fileName = "UnitGlobalConfig", menuName = "GlobalConfigs/UnitGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class UnitGlobalConfig : GlobalConfig<UnitGlobalConfig>
    {
        public float unitScaleHit = 1.5f;
        public float distanceMoveToNearHit = 0.15f;
        public float unitScaleHitDuration = 0.2f;
        public float sizeUnitHead = 0.3f;
        public List<UnitConfig> unitConfigs = new();
        public List<SharpenerColor> unitMaterial = new();
        public List<SharpenerColor> tipMaterial = new();
        
        public UnitConfig GetUnitConfig(UnitLengthType unitLengthType)
        {
            return unitConfigs.Find(config => config.unitLength == unitLengthType);
        }
        
        public Material GetUnitMaterial(SharpenerColorType colorType)
        {
            return unitMaterial.Find(color => color.colorType == colorType)?.colorMat;
        }
        
        public Material GetTipMaterial(SharpenerColorType colorType)
        {
            return tipMaterial.Find(color => color.colorType == colorType)?.colorMat;
        }

#if UNITY_EDITOR
        [Button]
        private void CreateTipMaterial()
        {
            tipMaterial.Clear();
            foreach (var color in Enum.GetNames(typeof(SharpenerColorType)))
            {
                var newTipColor = new SharpenerColor();
                newTipColor.colorType = Enum.Parse<SharpenerColorType>(color);
                var path = $"Assets/_Game/Materials/TipMaterial/{newTipColor.colorType.ToString()}.mat";
                newTipColor.colorMat =
                    AssetDatabase.LoadAssetAtPath<Material>(path);
                tipMaterial.Add(newTipColor);
            }
        }
        
        [Button]
        private void CreateUnitMaterial()
        {
            unitMaterial.Clear();
            foreach (var color in Enum.GetNames(typeof(SharpenerColorType)))
            {
                var newUnitMaterial = new SharpenerColor();
                newUnitMaterial.colorType = Enum.Parse<SharpenerColorType>(color);
                var path = $"Assets/_Game/Materials/UnitMaterial/{newUnitMaterial.colorType.ToString()}.mat";
                newUnitMaterial.colorMat =
                    AssetDatabase.LoadAssetAtPath<Material>(path);
                unitMaterial.Add(newUnitMaterial);
            }
        }
#endif
    }

    [System.Serializable]
    public class UnitConfig
    {
        public UnitLengthType unitLength;
        public float size;
    }
    
    [System.Serializable]
    public class SharpenerColor
    {
        public SharpenerColorType colorType;
        public Material colorMat;
    }
}
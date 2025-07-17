using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using Sirenix.Utilities;
using UnityEngine;

namespace _Game.Scripts.GlobalConfig
{
    [CreateAssetMenu(fileName = "UnitGlobalConfig", menuName = "GlobalConfigs/UnitGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class UnitGlobalConfig : GlobalConfig<UnitGlobalConfig>
    {
        public List<UnitConfig> unitConfigs = new();
        public List<SharpenerColor> unitMaterial = new();
        
        public UnitConfig GetUnitConfig(UnitLengthType unitLengthType)
        {
            return unitConfigs.Find(config => config.unitLength == unitLengthType);
        }
        
        public Material GetUnitMaterial(SharpenerColorType colorType)
        {
            return unitMaterial.Find(color => color.colorType == colorType)?.colorMat;
        }
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
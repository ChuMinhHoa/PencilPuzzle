using System.Collections.Generic;
using _Game.Scripts.ScriptAbleObject;
using Sirenix.Utilities;
using UnityEngine;

namespace _Game.Scripts.GlobalConfig
{
    [CreateAssetMenu(fileName = "LevelGlobalConfig", menuName = "GlobalConfigs/LevelGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class LevelGlobalConfig : GlobalConfig<LevelGlobalConfig>
    {
        public List<LevelConfig> levelConfigs = new();

        public LevelConfig GetLevelConfig(int level)
        {
            return levelConfigs.Find(x => x.level == level);
        }
    }
}
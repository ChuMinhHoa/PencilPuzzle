using System.Collections.Generic;
using _Game.Scripts.GameEnum;
using _Game.Scripts.Manager;
using R3;
using UnityEngine;

namespace BaseGame.Scripts.Data
{
    [System.Serializable]
    public class SettingData
    {
        public static SettingData Instance => InGameDataManager.Instance.InGameData.SettingData;
        [field: SerializeField] public List<SettingSubData> SettingSubDataList { get; set; } = new();
        [field: SerializeField] public SerializableReactiveProperty<string> LanguageCode { get; private set; }
        public SettingSubData GetSettingSubData(SettingType type)
        {
            for (int i = 0; i < SettingSubDataList.Count; i++)
            {
                if (SettingSubDataList[i].Type == type)
                {
                    return SettingSubDataList[i];
                }
            }
            SettingSubData newSetting = new(type, true);
            SettingSubDataList.Add(newSetting);
            return newSetting;
        }
        public void SetSettingValue(SettingType type, bool value)
        {
            GetSettingSubData(type).Value.Value = value;
            InGameDataManager.Instance.SaveData();
        }
        
        public void SetLanguage(string code)
        {
            LanguageCode.Value = code;
            InGameDataManager.Instance.SaveData();
        }
    }
    
    [System.Serializable]
    public class SettingSubData
    {
        [field: SerializeField] public SettingType Type  { get; private set; }
        [field: SerializeField] public SerializableReactiveProperty<bool> Value { get; private set; }

        public SettingSubData(SettingType type, bool value)
        {
            Type = type;
            Value = new(value);
        }
    }
}
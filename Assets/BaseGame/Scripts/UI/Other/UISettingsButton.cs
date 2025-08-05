using System;
using _Game.Scripts.GameEnum;
using _Game.Scripts.Manager;
using BaseGame.Scripts.Data;
using CoreData;
using Manager;
using R3;
using UnityEngine;

namespace Core.UI.Other
{
    public class UISettingsButton : MonoBehaviour
    {
        [field: SerializeField] private SettingType SettingType { get; set; }
        [field: SerializeField] private UIToggleButton UIToggleButton {get; set;}
        [field: SerializeField] private SerializableReactiveProperty<bool> CurrentValue { get; set; }
        private void Start()
        {
            Initialize();
        }

        private void Initialize()
        {
            UIToggleButton.InitValue(SettingData.Instance.GetSettingSubData(SettingType).Value);
            UIToggleButton.OnClickButton = OnClickButton;
        }
        private void OnClickButton(bool value)
        {
            InGameDataManager.Instance.SaveData();
        }
    }
}
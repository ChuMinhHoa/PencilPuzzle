using _Game.Scripts.GameEnum;
using BaseGame.Scripts.Data;
using CoreData;
using Lofelt.NiceVibrations;
using R3;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace Manager
{
    public class VibrationManager : Singleton<VibrationManager>
    {
        [field: SerializeField] public bool IsActive { get; set; }

        private void Start()
        {
            SettingData.Instance.GetSettingSubData(SettingType.Vibration).Value.Subscribe(SetVibration).AddTo(this);
        }

        private void SetVibration(bool value)
        {
            IsActive = value;
        }

        public void CallHaptic(HapticPatterns.PresetType presetType)
        {
            if (!IsActive) return;
            HapticPatterns.PlayPreset(presetType);
        }
    }
}
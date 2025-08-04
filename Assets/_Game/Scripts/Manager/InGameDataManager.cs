using _Game.Scripts.Manager;
using CodeStage.AntiCheat.Storage;
using CoreData;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class InGameDataManager : Singleton<InGameDataManager>
    {
        [field: SerializeField] public InGameData InGameData { get; private set; } = new();

        protected override void Awake()
        {
            base.Awake();
            LoadData();
        }
        [Button]
        public void SaveData()
        {
            PlayerPrefs.SetString(DataSerializer.Encrypt(GameStaticData.KeyInGameData), DataSerializer.Serialize(InGameData));
        }
        [Button]
        public void LoadData()
        {
            InGameData = DataSerializer.Deserialize<InGameData>(
                PlayerPrefs.GetString(DataSerializer.Encrypt(GameStaticData.KeyInGameData),
                    DataSerializer.Serialize(new InGameData())));
        }

        public void ResetData()
        {
            InGameData = new InGameData();
            SaveData();
        }
    }
}

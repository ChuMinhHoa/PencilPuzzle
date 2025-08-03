using _Game.Scripts.Manager;
using CodeStage.AntiCheat.Storage;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class InGameDataManager : Singleton<InGameDataManager>
    {
        [field: SerializeField] public InGameData InGameData { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            LoadData();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        [Button]
        public void SaveData()
        {
            ObscuredPrefs.Set(GameStaticData.KeyInGameData, MemoryPackSerializer.Serialize(InGameData));
        }
        [Button]
        public void LoadData()
        {
            InGameData = MemoryPackSerializer.Deserialize<InGameData>(
                ObscuredPrefs.Get<byte[]>(GameStaticData.KeyInGameData, 
                    MemoryPackSerializer.Serialize(new InGameData())));
        }
        [Button]
        public void ResetData()
        {
            InGameData = new InGameData();  
            SaveData();
        }
    }
}

[System.Serializable]
[MemoryPackable(GenerateType.VersionTolerant)]
public partial class InGameData
{
    [MemoryPackOnSerializing]
    public void OnSerializing()
    {
        playerDataSave ??= new PlayerDataSave(); //0
    }
    [MemoryPackOnDeserialized]
    public void OnDeserialized()
    {
        playerDataSave ??= new PlayerDataSave();
    }
}

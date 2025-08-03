using System.Collections.Generic;
using _Game.Scripts.Core;
using _Game.Scripts.Manager;
using MemoryPack;
using NUnit.Framework;
using TW.Reactive.CustomComponent;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    [System.Serializable]
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class PlayerDataSave
    {
        public static PlayerDataSave Instance => InGameDataManager.Instance.InGameData.playerDataSave;
        [MemoryPackOrder(0)]
        [field: SerializeField] public ReactiveValue<int> PlayerLevel { get; set; } = new(0);
        [MemoryPackOrder(1)]
        [field: SerializeField] public List<GameResourceSave> GameResources { get; set; } = new();
    }
}


public partial class InGameData
{
    [MemoryPackOrder(0)]
    public PlayerDataSave playerDataSave = new PlayerDataSave();
}
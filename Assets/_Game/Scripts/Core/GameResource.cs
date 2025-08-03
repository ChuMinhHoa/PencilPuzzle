using MemoryPack;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;

namespace _Game.Scripts.Core
{
    public enum ResourceType
    {
        None = 0,
        Gold = 1
    }
    [System.Serializable]
    public class GameResource
    {
        [field: HideLabel, HorizontalGroup(nameof(GameResource), 100)]
        public ResourceType type;
       
        private BigNumber value;
        
        public BigNumber Value
        {
            get => value;
            set
            {
                this.value = value;
                amount.Value = value;
            }
        }
        [field: SerializeField, HideLabel, HorizontalGroup(nameof(GameResource))]
        public ReactiveValue<BigNumber> amount = new();
    
        public GameResource()
        {
            amount.ReactiveProperty.Subscribe(OnAmountChanged);
        }

        public void OnAmountChanged(BigNumber valueChange)
        {
            value = valueChange;
        }
        
        public void Add(BigNumber valueToAdd)
        {
            Value += valueToAdd;
        }
        
        public void Consume(BigNumber valueToAdd)
        {
            Value -= valueToAdd;
        }
    }

    [System.Serializable]
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GameResourceSave
    {
        [MemoryPackOrder(0)]
        public ResourceType type;
        [MemoryPackOrder(1)]
        public double value;
        [MemoryPackOrder(2)]
        public int exp;
      
    }
}
using System;
using BaseGame.Scripts.Helper;
using R3;
using UnityEngine;

namespace CoreData
{
    /// <summary>
    /// Represents a game resource with type, value and expiration information
    /// </summary>
    [Serializable]
    public class GameResource
    {
        [field: SerializeField] public ResourceType Type { get; private set; }
        
        [field: ResourceSpecificTypeEditor("Type")]
        [field: SerializeField] public int SpecificType { get; private set; }
        
        [field: SerializeField] public SerializableReactiveProperty<float> Value { get; private set; }
        
        [field: SerializeField] public string ExpireTime { get; private set; }
        
        public int IntValue => Mathf.RoundToInt(Value.Value);
        /// <summary>
        /// Base constructor for creating a game resource
        /// </summary>
        public GameResource(ResourceType type, int specificType, float value, string expireTime = "")
        {
            InitializeResource(type, specificType, value, expireTime);
        }

        /// <summary>
        /// Creates a currency resource
        /// </summary>
        public GameResource(CurrencyType currencyType, float value, string expireTime = "") 
            : this(ResourceType.Currency, (int)currencyType, value, expireTime)
        {
        }

        /// <summary>
        /// Creates a booster resource
        /// </summary>
        public GameResource(BoosterType boosterType, float value, string expireTime = "") 
            : this(ResourceType.Booster, (int)boosterType, value, expireTime)
        {
        }

        /// <summary>
        /// Creates a special resource
        /// </summary>
        public GameResource(SpecialResourceType specialResourceType, float value, string expireTime = "") 
            : this(ResourceType.Special, (int)specialResourceType, value, expireTime)
        {
        }

        private void InitializeResource(ResourceType type, int specificType, float value, string expireTime)
        {
            if (value < 0)
                throw new ArgumentException("Resource value cannot be negative", nameof(value));
                
            Type = type;
            SpecificType = specificType;
            Value = new SerializableReactiveProperty<float>(value);
            ExpireTime = expireTime ?? string.Empty;
        }
    }
}
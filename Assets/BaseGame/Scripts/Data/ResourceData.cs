using System;
using System.Collections.Generic;
using System.Globalization;
using _Game.Scripts.Manager;
using UnityEngine;

namespace CoreData
{
    [Serializable]
    public class ResourceData
    {
        public static ResourceData Instance => InGameDataManager.Instance.InGameData.ResourceData;
        [field: SerializeField] public List<GameResource> Resources { get; set; }
        [field: SerializeField] public List<BoosterType> UsedBoosterOnTut { get; set; }
        [field: SerializeField] public List<string> LifeSave { get; set; }
        [field: SerializeField] public List<string> PurchasedPacks { get; set; }
        
        public ResourceData()
        {
            Resources = new List<GameResource>()
            {
                new GameResource(CurrencyType.Life, 5),
                new GameResource(BoosterType.FreezeClock, 1),
                new GameResource(BoosterType.Propeller, 1),
                new GameResource(BoosterType.Magnet, 1),
            };
        }
        
    }
}
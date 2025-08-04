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
        public GameResource GetResource(ResourceType type, int specificType)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                GameResource resource = Resources[i];
                if (resource.Type == type && resource.SpecificType == specificType)
                {
                    return resource;
                }
            }
            GameResource newResource = new GameResource(type, specificType, 0);
            Resources.Add(newResource);
            return newResource;
        }
        public GameResource GetResource(CurrencyType type)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                GameResource resource = Resources[i];
                if (resource.Type == ResourceType.Currency && resource.SpecificType == (int)type)
                {
                    return resource;
                }
            }
            GameResource newResource = new GameResource(ResourceType.Currency, (int)type, 0);
            Resources.Add(newResource);
            return newResource;
        }

        public GameResource GetResource(BoosterType type)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                GameResource resource = Resources[i];
                if (resource.Type == ResourceType.Booster && resource.SpecificType == (int)type)
                {
                    return resource;
                }
            }
            GameResource newResource = new GameResource(ResourceType.Booster, (int)type, 0);
            Resources.Add(newResource);
            return newResource;
        }

        public GameResource GetResource(SpecialResourceType type)
        {
            for (int i = 0; i < Resources.Count; i++)
            {
                GameResource resource = Resources[i];
                if (resource.Type == ResourceType.Special && resource.SpecificType == (int)type)
                {
                    return resource;
                }
            }

            GameResource newResource = new GameResource(ResourceType.Special, (int)type, 0);
            Resources.Add(newResource);
            return newResource;
        }

        public bool IsEnoughResourceValue(ResourceType type, int specificType, float value)
        {
            if(type == ResourceType.Currency && specificType == (int)CurrencyType.Life)
            {
                if(GetResource(ResourceType.Special, (int)SpecialResourceType.InfiniteLife).Value.Value > 0)
                {
                    return true;
                }
            }
            GameResource resource = GetResource(type, specificType);
            return resource.Value.Value >= value;
        }
        
        public void SubResourceValue(ResourceType type, int specificType, float value)
        {
            if(type == ResourceType.Currency && specificType == (int)CurrencyType.Life)
            {
                if(GetResource(ResourceType.Special, (int)SpecialResourceType.InfiniteLife).Value.Value > 0)
                {
                    return;
                }
            }
            GameResource resource = GetResource(type, specificType);
            if (resource.Value.Value >= value)
            {
                resource.Value.Value -= value;
            }
            UpdateResourceValue(type, specificType);
        }
        public void AddResourceValue(ResourceType type, int specificType, float value)
        {
            GameResource resource = GetResource(type, specificType);
            resource.Value.Value += value;
            UpdateResourceValue(type, specificType);
        }
        
        void UpdateResourceValue(ResourceType type, int specificType)
        {
            GameResource resource = GetResource(type, specificType);
            switch (type)
            {
                case ResourceType.None:
                    break;
                case ResourceType.Currency:
                    switch ((CurrencyType)specificType)
                    {
                        case CurrencyType.Money:
                            // Handle money resource addition logic if needed
                            break;
                        case CurrencyType.Life:
                            resource.Value.Value = Mathf.Clamp(resource.Value.Value, 0, DefaultGlobalConfig.Instance.DefaultLife);
                            UpdateLifeSave();
                            break;
                        case CurrencyType.Gem:
                            // Handle gem resource addition logic if needed
                            break;
                        default:
                            break;
                    }
                    break;
                case ResourceType.Booster:
                    break;
                case ResourceType.Special:
                    break;
                default:
                    break;
            }
        }

        void UpdateLifeSave()
        {
            if(LifeSave == null)
            {
                LifeSave = new List<string>();
            }
            DateTime firstLife = DateTime.Now;
            if(LifeSave.Count > 0)
            {
                try
                {
                    firstLife = DateTime.Parse(LifeSave[0], CultureInfo.InvariantCulture);
                }
                catch (FormatException)
                {
                    Debug.LogError("Invalid date format in life save data.");
                    LifeSave.Clear();
                    firstLife = DateTime.Now.AddMinutes(DefaultGlobalConfig.Instance.DefaultFillHeartTime);
                }
            }
            else
            {
                firstLife = DateTime.Now.AddMinutes(DefaultGlobalConfig.Instance.DefaultFillHeartTime);
            }
            LifeSave.Clear();
            int lifeCount = (int)(GetResource(ResourceType.Currency, (int)CurrencyType.Life).Value.Value);
            int needToAdd = DefaultGlobalConfig.Instance.DefaultLife - lifeCount;
            if (needToAdd > 0)
            {
                for (int i = 0; i < needToAdd; i++)
                {
                    DateTime nextLife = firstLife.AddMinutes(DefaultGlobalConfig.Instance.DefaultFillHeartTime * i);
                    LifeSave.Add(nextLife.ToString(CultureInfo.InvariantCulture));
                }
            }
            InGameDataManager.Instance.SaveData();
            RewardManager.Instance.UpdateLifeInfo();
        }
        
        public bool IsBoosterUsedOnTut(BoosterType boosterType)
        {
            if(UsedBoosterOnTut == null)
            {
                UsedBoosterOnTut = new List<BoosterType>();
            }
            return UsedBoosterOnTut.Contains(boosterType);
        }

        public void AddBoosterUsedOnTut(BoosterType boosterType)
        {
            if (UsedBoosterOnTut == null)
            {
                UsedBoosterOnTut = new List<BoosterType>();
            }

            if (!UsedBoosterOnTut.Contains(boosterType))
            {
                UsedBoosterOnTut.Add(boosterType);
            }
        }
        
        public void AddPurchasedPack(string packName)
        {
            if (PurchasedPacks == null)
            {
                PurchasedPacks = new List<string>();
            }
            if (!PurchasedPacks.Contains(packName))
            {
                PurchasedPacks.Add(packName);
            }
        }
        public bool IsPackPurchased(string packName)
        {
            if (PurchasedPacks == null)
            {
                PurchasedPacks = new List<string>();
            }
            return PurchasedPacks.Contains(packName);
        }
    }
}
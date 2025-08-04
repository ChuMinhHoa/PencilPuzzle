using System;
using System.Collections.Generic;
using _Game.Scripts.Core;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomType;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Manager
{
    public class PlayerResourceManager : Singleton<PlayerResourceManager>
    {
        public List<GameResourceSave> gameResourcesSave = new();
        public List<GameResource> gameResources = new();
        
        private void Start()
        {
            LoadData();
        }

        private void LoadData()
        {
            gameResourcesSave = PlayerDataSave.Instance.GameResources;
            
            InitResources();
        }

        private void InitResources()
        {
            for (var i = 0; i < gameResourcesSave.Count; i++)
            {
                var newGameResource = new GameResource
                {
                    type = gameResourcesSave[i].type,
                    Value = new BigNumber(gameResourcesSave[i].value, gameResourcesSave[i].exp)
                };
                gameResources.Add(newGameResource);
            }
        }
        
        [Button]
        public void AddResource(ResourceType type, BigNumber value)
        {
            for (var i = 0; i < gameResources.Count; i++)
            {
                if (gameResources[i].type == type)
                {
                    gameResources[i].Value += value;
                    InGameDataManager.Instance.SaveData();
                    return;
                }
            }

            var newGameResource = new GameResource
            {
                type = type,
                Value = value
            };
            gameResources.Add(newGameResource);
            CreateNewResourceSave(type, value);
            InGameDataManager.Instance.SaveData();
        }
        
        private void CreateNewResourceSave(ResourceType type, BigNumber value)
        {
            var newResourceSave = new GameResourceSave
            {
                type = type,
                value = value.coefficient,
                exp = value.exponent
            };
            gameResourcesSave.Add(newResourceSave);
        }

        public void ConsumeResource(ResourceType type, BigNumber value)
        {
            for (var i = 0; i < gameResources.Count; i++)
            {
                if (gameResources[i].type == type)
                {
                    gameResources[i].Value -= value;
                    InGameDataManager.Instance.SaveData();
                    return;
                }
            }

            Debug.LogWarning($"Resource not found: {type}");
        }

        public ReactiveValue<BigNumber> GetGameResource(ResourceType type)
        {
            for (var i = 0; i < gameResources.Count; i++)
            {
                if (gameResources[i].type == type)
                {
                    return gameResources[i].amount;
                }
            }
            var newGameResource = new GameResource
            {
                type = type,
                Value = new BigNumber(0)
            };
            gameResources.Add(newGameResource);
            return newGameResource.amount;
        }
    }
}

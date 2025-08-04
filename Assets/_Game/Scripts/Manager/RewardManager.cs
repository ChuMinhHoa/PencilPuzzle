using System;
using System.Collections.Generic;
using System.Globalization;
using _Game.Scripts.UI.Core;
using Core.UI.Modals;
using CoreData;
using Cysharp.Threading.Tasks;
using R3;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class RewardManager : Singleton<RewardManager>
    {
        public List<GameResource> rewardResources = new List<GameResource>();
        public SerializableReactiveProperty<float> fillHeartTime = new(0);
        public SerializableReactiveProperty<float> infinityLifeTime = new(0);
        public List<string> lifeSave;
        public bool needFillHeart;

        public void LoadData()
        {
            LoadLifeInfo();
            UpdateLifeInfo();
            LoadInfinityLifeTime();
        }
        public void AddReward(List<GameResource> GameResources)
        {
            rewardResources.Clear();
            rewardResources.AddRange(GameResources);
            ClaimReward();
        }

        public void ShowReward()
        {
            if(rewardResources != null && rewardResources.Count > 0)
            {
                ShowRewardAsync().Forget();
            }
        }
    
        async UniTask ShowRewardAsync()
        {
            ViewOptions viewOptions = new ViewOptions(nameof(ModalReward));
            await ModalContainer.Find(ContainerKey.Modals).PushAsync(viewOptions);
        }
    
        public void ClaimReward()
        {
            for (int i = 0; i < rewardResources.Count; i++)
            {
                ClaimReward(rewardResources[i]);
            }
        }
    
        public void ClaimReward(GameResource gameResource)
        {
            InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(gameResource.Type, gameResource.SpecificType, gameResource.Value.Value);
        }

        void LoadLifeInfo()
        {
            lifeSave = InGameDataManager.Instance.InGameData.ResourceData.LifeSave;
            if (lifeSave == null)
            {
                fillHeartTime.Value = 0;
                needFillHeart = false;
                return;
            }
            if (lifeSave.Count == 0)
            {
                fillHeartTime.Value = 0;
                needFillHeart = false;
                return;
            }

            int toAddLifeCount = 0;
            for (int i = lifeSave.Count - 1; i >= 0; i--)
            {
                DateTime firstLifeDateTime;
                try
                {
                    firstLifeDateTime = DateTime.Parse(lifeSave[i], CultureInfo.InvariantCulture);
                    TimeSpan timeSinceFirstLife = DateTime.Now - firstLifeDateTime;
                    if (timeSinceFirstLife.TotalSeconds > 0)
                    {
                        toAddLifeCount++;
                        lifeSave.RemoveAt(i);
                    }
                }
                catch (FormatException)
                {
                    Debug.LogError("Invalid date format in life save data.");
                    fillHeartTime.Value = 0;
                    needFillHeart = false;
                    return;
                }
            }
            InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Life, toAddLifeCount);
        }

        public void UpdateLifeInfo()
        {
            if(lifeSave == null)
            {
                fillHeartTime.Value = 0;
                needFillHeart = false;
                return;
            }

            if (lifeSave.Count == 0)
            {
                fillHeartTime.Value = 0;
                needFillHeart = false;
                return;
            }

            if (lifeSave.Count > 0)
            {
                string firstLife = lifeSave[0];
                DateTime firstLifeDateTime;
                try
                {
                    firstLifeDateTime = DateTime.Parse(firstLife, CultureInfo.InvariantCulture);
                    TimeSpan timeSinceFirstLife = DateTime.Now - firstLifeDateTime;
                    fillHeartTime.Value = -(float)(timeSinceFirstLife.TotalSeconds);   
                    needFillHeart = true;
                }
                catch (FormatException)
                {
                    Debug.LogError("Invalid date format in life save data.");
                    fillHeartTime.Value = 0;
                    needFillHeart = false;
                    return;
                }
            }
        }

        void Update()
        {
            if(needFillHeart) 
            {
                fillHeartTime.Value -= Time.deltaTime;
                if (fillHeartTime.Value <= 0)
                {
                    if(lifeSave.Count > 0)
                    {
                        lifeSave.RemoveAt(0);
                        InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Life, 1);
                    }
                }
            }
            if(infinityLifeTime.Value > 0)
            {
                infinityLifeTime.Value -= Time.deltaTime;
            }
        }
    
        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused)
            {
                Debug.Log("OnApplicationPause");
            }
            else
            {
                Debug.Log("OnApplicationResume");
                LoadData();
            }
        }

        void LoadInfinityLifeTime()
        {
            infinityLifeTime = InGameDataManager.Instance.InGameData.ResourceData.GetResource(ResourceType.Special, (int)SpecialResourceType.InfiniteLife).Value;
        }
    }
}
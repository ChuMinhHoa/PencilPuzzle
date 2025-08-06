using System;
using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using CoreData;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalBooster : Modal
    {
        [field: SerializeField] public ModalBoosterContext.UIPresenter UIPresenter { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            AddLifecycleEvent(UIPresenter, 1);
        }

        public override async UniTask Initialize(Memory<object> args)
        {
            await base.Initialize(args);
        }
    }


    [Serializable]
    public class ModalBoosterContext
    {
        public static class Events
        {
            public static Action SampleEvent { get; set; }
        }

        [HideLabel]
        [Serializable]
        public class UIModel : IAModel
        {
            [field: Title(nameof(UIModel))]
            [field: SerializeField]
            public SerializableReactiveProperty<int> SampleValue { get; private set; }
            [field: SerializeField] public BoosterSelector BoosterSelector { get; private set; }
            [field: SerializeField] public BoosterData BoosterData { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                if (args.Length > 0)
                {
                    BoosterSelector = (BoosterSelector)args.Span[0];
                    BoosterData = ItemGlobalConfig.Instance.GetBoosterData(BoosterSelector.BoosterType);
                }
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIView : IAView
        {
            [field: Title(nameof(UIView))]
            [field: SerializeField]
            public CanvasGroup MainView { get; private set; }
            [field: SerializeField] public TextMeshProUGUI BoosterNameTxt { get; private set; }
            [field: SerializeField] public TextMeshProUGUI BoosterDesTxt { get; private set; }
            [field: SerializeField] public TextMeshProUGUI BoosterPriceTxt { get; private set; }
            [field: SerializeField] public Image BoosterIcon { get; private set; }
            [field: SerializeField] public Button CoinBuyBoosterBtn { get; private set; }
            [field: SerializeField] public Button AdsBuyBoosterBtn { get; private set; }
            [field: SerializeField] public Button ExitBtn { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                if (args.Length > 0)
                {
                    BoosterSelector boosterSelector = (BoosterSelector)args.Span[0];
                    BoosterData boosterData = ItemGlobalConfig.Instance.GetBoosterData(boosterSelector.BoosterType);
                    BoosterNameTxt.text = boosterData.BoosterName;
                    BoosterDesTxt.text = boosterData.Description;
                    BoosterPriceTxt.text = $"{boosterData.Price}";
                    BoosterIcon.sprite = boosterData.Sprite;
                    //CoinBuyBoosterBtn.interactable = InGameDataManager.Instance.InGameData.ResourceData.IsEnoughResourceValue(ResourceType.Currency, (int)(CurrencyType.Money), boosterData.Price);
                }
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                // View.ExitBtn.SetOnClickDestination(() => OnButtonCloseClick().Forget());
                // View.CoinBuyBoosterBtn.SetOnClickDestination(() => OnButtonBuyBoosterClick().Forget());
                // View.AdsBuyBoosterBtn.SetOnClickDestination(() => OnAdsButtonBuyBoosterClick().Forget());
            }
            
            private async UniTask OnButtonBuyBoosterClick()
            {
                PlayerResourceManager.Instance.AddResourceValue(ResourceType.Booster, (int)(Model.BoosterSelector.BoosterType), 1);
                PlayerResourceManager.Instance.SubResourceValue(ResourceType.Currency, (int)(CurrencyType.Money), Model.BoosterData.Price);
                await OnButtonCloseClick();
                Model.BoosterSelector.SelectBooster();
            }
            
            private async UniTask OnAdsButtonBuyBoosterClick()
            {
                View.MainView.interactable = false;
                //InGameAdsController.EventShowAdsReward?.Invoke($"AdsRw_GetBooste{Model.BoosterData.BoosterType}", () => OnAdsButtonBuyBoosterSuccess().Forget(), OnAdsButtonBuyBoosterFail);
            }
            
            private async UniTask OnAdsButtonBuyBoosterSuccess()
            {
                PlayerResourceManager.Instance.AddResourceValue(ResourceType.Booster, (int)(Model.BoosterSelector.BoosterType), 1);
                await OnButtonCloseClick();
                Model.BoosterSelector.SelectBooster();
            }
            
            private void OnAdsButtonBuyBoosterFail()
            {
                View.MainView.interactable = true;
            }
            
            private async UniTask OnButtonCloseClick()
            {
                await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            }
            
            public void DidPushEnter(Memory<object> args)
            {
                //LevelManager.Instance.SetPause(true);
            }
            public void DidPopEnter(Memory<object> args)
            {
                //LevelManager.Instance.SetPause(true);
            }

            public void DidPushExit(Memory<object> args)
            {
                //LevelManager.Instance.SetPause(false);
            }
            public void DidPopExit(Memory<object> args)
            {
                //LevelManager.Instance.SetPause(false);
            }
        }
    }
}
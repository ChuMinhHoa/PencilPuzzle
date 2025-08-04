using System;
using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using CoreData;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.MVPPattern;
using UnityEngine;

namespace BaseGame.Scripts.UI.Modals
{
    public class ModalFillHeart : Modal
    {
        [field: SerializeField] public ModalFillHeartContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalFillHeartContext
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

            public UniTask Initialize(Memory<object> args)
            {
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
            [field: SerializeField] public CustomButton BuyHeartWithAdsBtn {get; set;}
            [field: SerializeField] public CustomButton BuyHeartWithCoinBtn {get; set;}
            [field: SerializeField] public CustomButton BuyClose {get; set;}
            [field: SerializeField] public TextMeshProUGUI CoinPriceTxt {get; set;}
            [field: SerializeField] public GameObject NotEnoughLifeObj {get; set;}

            public UniTask Initialize(Memory<object> args)
            {
                BuyHeartWithCoinBtn.SetInteractable(InGameDataManager.Instance.InGameData.ResourceData.IsEnoughResourceValue(ResourceType.Currency, (int)CurrencyType.Money, DefaultGlobalConfig.Instance.DefaultFullHeartCoin));
                CoinPriceTxt.text = $"{DefaultGlobalConfig.Instance.DefaultFullHeartCoin}";
                NotEnoughLifeObj.SetActive(GameManager.Instance._inGame);
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
                //View.BuyHeartWithAdsBtn.button.SetOnClickDestination(BuyHeartWithAds);
                //View.BuyHeartWithCoinBtn.button.SetOnClickDestination(BuyHeartWithCoin);
                View.BuyClose.button.SetOnClickDestination(OnButtonCloseClick);
            }

            void BuyHeartWithAds()
            {
                //InGameAdsController.EventShowAdsReward?.Invoke("AdsRw_BuyHeart", BuyHeartWithAdsSuccess, BuyHeartWithAdsFail);
            }
            void BuyHeartWithAdsSuccess()
            {
                InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Life, 1);
            }
            void BuyHeartWithAdsFail()
            {
                
            }
            void BuyHeartWithCoin()
            {
                InGameDataManager.Instance.InGameData.ResourceData.SubResourceValue(ResourceType.Currency, (int)CurrencyType.Money, DefaultGlobalConfig.Instance.DefaultFullHeartCoin);
                InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Life, DefaultGlobalConfig.Instance.DefaultLife);
                OnButtonCloseClick().Forget();
            }
            private async UniTask OnButtonCloseClick()
            {
                await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            }
        }
    }
}
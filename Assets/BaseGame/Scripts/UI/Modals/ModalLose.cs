using System;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.OnGameManager;
using _Game.Scripts.UI.Core;
using Core.UI.Activities;
using Core.UI.Screens;
using CoreData;
using Cysharp.Threading.Tasks;
using Manager;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalLose : Modal
    {
        [field: SerializeField] public ModalLoseContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalLoseContext
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
            public float ReviveCoin { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                ReviveCoin = DefaultGlobalConfig.Instance.ReviveCoin;
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
            [field: SerializeField] public Button ButtonKeepPlaying {get; private set;}
            [field: SerializeField] public Button ButtonClose {get; private set;}
            [field: SerializeField] public TextMeshProUGUI reviveCoinTxt {get; private set;}
            [field: SerializeField] public TextMeshProUGUI reviveTime1Txt {get; private set;}
            [field: SerializeField] public TextMeshProUGUI reviveTime2Txt {get; private set;}
            [field: SerializeField] public List<UIShopPack> UiShopPacks {get; private set;}
            [field: SerializeField] public List<TextMeshProUGUI> UiShopReviveTime {get; private set;}
            public UniTask Initialize(Memory<object> args)
            {
                reviveCoinTxt.text = $"{DefaultGlobalConfig.Instance.ReviveCoin}";
                reviveTime1Txt.text = $"Get {DefaultGlobalConfig.Instance.ReviveValue.reviveTime}s to keep playing";
                reviveTime2Txt.text = $"+{DefaultGlobalConfig.Instance.ReviveValue.reviveTime}s";
                ButtonKeepPlaying.interactable = PlayerResourceManager.Instance.IsEnoughResourceValue(ResourceType.Currency, (int)CurrencyType.Money, DefaultGlobalConfig.Instance.ReviveCoin);
                for (var i = 0; i < UiShopReviveTime.Count; i++)
                {
                    UiShopReviveTime[i].text = $"Instant<br>+ {DefaultGlobalConfig.Instance.ReviveValue.reviveTime}s";
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
                AudioManager.Instance.PlaySoundFx(AudioKey.SfxUILoseGame);
                await Model.Initialize(args);
                await View.Initialize(args);
                
                View.ButtonKeepPlaying.SetOnClickDestination(OnClickButtonKeepPlaying);
                View.ButtonClose.SetOnClickDestination(OnClickButtonClose);
            }
            private async UniTask OnClickButtonKeepPlaying()
            {
                PlayerResourceManager.Instance.SubResourceValue(ResourceType.Currency, (int)CurrencyType.Money, Model.ReviveCoin);
                await ContinueSuccess();
            }
            
            private async UniTask ContinueSuccess()
            {
                await ModalContainer.Find(ContainerKey.Modals).PopAsync(false);
                //LevelManager.Instance.TryAddTime(DefaultGlobalConfig.Instance.ReviveValue.reviveTime);
                Debug.LogError("add time game play");
                GameManager.Instance.SetPause(false);
            }
            
            private async UniTask OnClickButtonClose()
            {
                ViewOptions activityLoading = new ViewOptions(nameof(ActivityLoading));
                Memory<object> args = new Memory<object>(new object[]
                {
                    (Func<UniTask>)(async () =>
                    {
                        ScreenOptions screenOptions = new ScreenOptions(nameof(ScreenMainMenu), stack: false);
                        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(screenOptions);
                        await ModalContainer.Find(ContainerKey.Modals).PopAsync(false);
                        GameManager.Instance.ClearLevel();
                    }),
                    null
                });
                await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(activityLoading, args);
            }

            void InitShopPack()
            {
                for (var i = 0; i < View.UiShopPacks.Count; i++)
                {
                    View.UiShopPacks[i].Init(() => ContinueSuccess().Forget(), false);
                }
            }
        }
    }
}
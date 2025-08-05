using System;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using BaseGame.Scripts.UI.Other;
using Core.UI.Modals;
using Core.UI.Other;
using CoreData;
using Cysharp.Threading.Tasks;
using Manager;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using Screen = TW.UGUI.Core.Screens.Screen;

namespace Core.UI.Screens
{
    public class ScreenInGame : Screen
    {
        [field: SerializeField] public ScreenInGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ScreenInGameContext
    {
        public static class Events
        {
            public static Action OnReloadCurrentLevel { get; set; }
            public static Action<BoosterType> ClaimBooster { get; set; }
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
            [field: SerializeField] public TimeBar TimeBar {get; private set;}
            [field: SerializeField] public TextMeshProUGUI TextLevel {get; private set;}
            [field: SerializeField] public Button ButtonPause {get; private set;}
            [field: SerializeField] public List<BoosterSelector> BoosterSelectors {get; private set;}
            [field: SerializeField] public CanvasGroup BoosterWrap {get; private set;}
            [field: SerializeField] public Transform BoosterClaimRoot {get; private set;}
            public UniTask Initialize(Memory<object> args)
            {
                //BoosterWrap.alpha = GameManager.Instance.Level.Value > 1 ? 1f : 0f;
                return UniTask.CompletedTask;
            }
            
            public void ClaimBooster(BoosterType boosterType)
            {
                foreach (var selector in BoosterSelectors)
                {
                    if (selector.BoosterType == boosterType)
                    {
                        selector.GetBoosterOnTutorial(BoosterClaimRoot.position);
                        break;
                    }
                }
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                LoadLevelInfo();
                View.ButtonPause.SetOnClickDestination(OnClickButtonPause);
                Events.OnReloadCurrentLevel = OnReloadCurrentLevel;
                Events.ClaimBooster = View.ClaimBooster;
            }

            public UniTask Cleanup(Memory<object> args)
            {
                Events.OnReloadCurrentLevel = null;
                Events.ClaimBooster = null;
                return UniTask.CompletedTask;
            }

            public void LoadLevelInfo()
            {
                //Debug.LogError("init time bar");
                View.TimeBar.InitTimeBar(GameManager.Instance.currentLevelManager.currentLevelConfig.timeDuration);
                View.TextLevel.SetText(GameManager.Instance.currentLevel.Value.ToString());

            }
            private async UniTask OnClickButtonPause()
            {
                ViewOptions viewOptions = new ViewOptions(nameof(ModalPause));
                await ModalContainer.Find(ContainerKey.Modals).PushAsync(viewOptions);
            }

            private void OnReloadCurrentLevel()
            {
                LoadLevelInfo();
            }
        }
    }
}
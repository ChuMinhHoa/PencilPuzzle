using System;
using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;

namespace Core.UI.Modals
{
    public class ModalLeaveGame : Modal
    {
        [field: SerializeField] public ModalLeaveGameContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalLeaveGameContext
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
            [field: SerializeField] public CustomButton ButtonLeave {get; set;}
            [field: SerializeField] public CustomButton ButtonClose {get; set;}

            public UniTask Initialize(Memory<object> args)
            {
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
                View.ButtonLeave.button.SetOnClickDestination(OnButtonLeaveClick);
                View.ButtonClose.button.SetOnClickDestination(OnButtonCloseClick);
            }
            
            private async UniTask OnButtonLeaveClick()
            {
                View.MainView.interactable = false;
                await UIAnimationBase.ButtonBasic(View.ButtonLeave.transform);
                await BackToMainMenu();
            }
            
            public async UniTask BackToMainMenu()
            {
                ViewOptions activityLoading = new ViewOptions(nameof(ActivityLoading));
                Memory<object> args = new Memory<object>(new object[]
                {
                    (Func<UniTask>)(async () =>
                    {
                        GameManager.Instance.currentLevelManager.ClearLevel();
                        ScreenOptions screenOptions = new ScreenOptions(nameof(ScreenMainMenu), false);
                        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(screenOptions);
                        await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
                    }),
                    null
                });
                await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(activityLoading, args);
            }
            
            private async UniTask OnButtonCloseClick()
            {
                await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            }
            
            public void DidPushEnter(Memory<object> args)
            {
                GameManager.Instance.SetPause(true);// LevelManager.Instance.SetPause(true);
            }
            public void DidPopEnter(Memory<object> args)
            {
                GameManager.Instance.SetPause(true);
            }

            public void DidPushExit(Memory<object> args)
            {
                GameManager.Instance.SetPause(false);
            }
            public void DidPopExit(Memory<object> args)
            {
                GameManager.Instance.SetPause(false);
            }
        }
    }
}
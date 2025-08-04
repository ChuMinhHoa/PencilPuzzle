using System;
using _Game.Scripts.UI.Core;
using CoreData;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;

namespace Core.UI.Activities
{
    public class ActivityUseBoosterFreezeClock : Activity
    {
        [field: SerializeField] public ActivityUseBoosterFreezeClockContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityUseBoosterFreezeClockContext
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
            [field: SerializeField] public CanvasGroup FreezeCG { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
                FreezeCG.alpha = 0f;
                LMotion.Create(0f, 1f, 0.25f).Bind(x => FreezeCG.alpha = x)
                    .ToValueTask(MainView.GetCancellationTokenOnDestroy());
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
        {
            [field: SerializeField] public UIModel Model { get; private set; } = new();
            [field: SerializeField] public UIView View { get; set; } = new();

            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
            }

            public void DidEnter(Memory<object> args)
            {
                AddTime().Forget();
            }

            public async UniTask AddTime()
            {
                Debug.Log("Play freeze clock animation");
                await UniTask.Delay(1000, cancellationToken: View.MainView.GetCancellationTokenOnDestroy());
                var freezeTime = ItemGlobalConfig.Instance.GetBoosterData(BoosterType.FreezeClock).Effect;
                //LevelManager.Instance.AddFreezeDuration(freezeTime);
                await ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityUseBoosterFreezeClock));
                //LevelManager.Instance.SetPause(false);
            }
        }
    }
}
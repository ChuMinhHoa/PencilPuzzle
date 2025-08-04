using System;
using System.Threading.Tasks;
using _Game.Scripts.UI.Core;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Activities;
using UnityEngine.UI;

namespace Core.UI.Activities
{
    public class ActivityLoading : Activity
    {
        [field: SerializeField] public ActivityLoadingContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityLoadingContext
    {
        public static class Events
        {
            public static Action SampleEvent { get; set; }

            public static Action<float> changeProgress { get; set; }
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
            
            [field: SerializeField] public Slider loadingBar { get; private set; }
            [field: SerializeField] public TextMeshProUGUI txtLoading { get; private set; }

            public UniTask Initialize(Memory<object> args)
            {
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

                Events.changeProgress += (value)=>_ = ChangeProgress(value);
            }

            private async UniTask ChangeProgress(float progress)
            {
                Debug.Log(progress);

                LMotion.Create(View.loadingBar.value, progress, 0.25f).Bind(x =>
                {
                    View.loadingBar.value = x;
                    View.txtLoading.text = $"Loading... {(int)(x * 100f)}%";
                }).AddTo(View.MainView);
                
                if (progress == 1f)
                {
                    await UniTask.Delay(1000);
                    CloseActivity();
                }
            }
            
            private void CloseActivity()
            {
                ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityLoading)).Forget();
            }
            
            UniTask IActivityLifecycleEvent.Cleanup(Memory<object> args)
            {
                Debug.Log("Clear activity loading context");
                Events.changeProgress = null;
                return UniTask.CompletedTask;
            }
        }
    }
}
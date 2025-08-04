using System;
using System.Collections.Generic;
using Core.UI.Other;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.MVPPattern;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BaseGame.Scripts.UI.Activities
{
    public class ActivityUseBoosterPropeller : Activity
    {
        [field: SerializeField] public ActivityUseBoosterPropellerContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityUseBoosterPropellerContext
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
            [field: SerializeField] public Transform ButtonSelectContainer { get; private set; }
            [field: SerializeField]
            public UIMaskButton ButtonSelectPrefab { get; private set; }
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
            // private List<People> PeopleCanUsePropeller { get; set; }
            // private List<UIMaskButton> PropellerButtons { get; set; }
            // private LinearFunction Scale { get; set; } = new LinearFunction(new Vector3(36.5f,1), new Vector3(50, 0.8f));
            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                // PeopleCanUsePropeller = LevelManager.Instance.GetPeopleCanUsePropeller();
                // PropellerButtons = new List<UIMaskButton>();
                // float scale = Scale.GetValue(CameraManager.Instance.GetCameraHeightZ());
                // for (int i = 0; i < PeopleCanUsePropeller.Count; i++)
                // {
                //     UIMaskButton button = Object.Instantiate(View.ButtonSelectPrefab, View.ButtonSelectPrefab.transform.parent);
                //     button.gameObject.SetActive(true);
                //     button.transform.position = CameraManager.Instance.WorldToScreenPoint(PeopleCanUsePropeller[i].transform.position);
                //     button.transform.localScale = Vector3.one * scale;
                //     PropellerButtons.Add(button);
                //     int capturedIndex = i;
                //     button.SetMainButtonParent(View.ButtonSelectContainer);
                //     button.SetOnClickDestination(OnClickAction);
                //     continue;
                //     UniTask OnClickAction() => OnClickButtonSelectPrefab(capturedIndex);
                // }
            }
            public async UniTask OnClickButtonSelectPrefab(int index)
            {
                // People people = PeopleCanUsePropeller[index];
                // if (people != null)
                // {
                //     people.UsePropeller();
                //     AudioManager.Instance.PlaySoundFx(AudioKey.SfxHelicopter);
                // }
                // await ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityUseBoosterPropeller));
                // await UniTask.Delay(2000, cancellationToken: people.GetCancellationTokenOnDestroy());
                // LevelManager.Instance.SetPause(false);
            }
        }
    }
}
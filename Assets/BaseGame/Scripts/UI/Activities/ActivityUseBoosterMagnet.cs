using System;
using Core.UI.Activities;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using TW.UGUI.MVPPattern;
using UnityEngine;

namespace BaseGame.Scripts.UI.Activities
{
    public class ActivityUseBoosterMagnet : Activity
    {
        [field: SerializeField] public ActivityUseBoosterMagnetContext.UIPresenter UIPresenter { get; private set; }

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
    public class ActivityUseBoosterMagnetContext
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

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }
        }

        [HideLabel]
        [Serializable]
        public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
        {
            [field: SerializeField] public ActivityUseBoosterPropellerContext.UIModel Model { get; private set; } = new();
            [field: SerializeField] public ActivityUseBoosterPropellerContext.UIView View { get; set; } = new();
            //private List<Hole> HoleCanMagnet { get; set; }
            //private List<UIMaskButton> MagnetButtons { get; set; }
            //private LinearFunction Scale { get; set; } = new LinearFunction(new Vector3(36.5f, 1.7f), new Vector3(50, 1.3f));
            public async UniTask Initialize(Memory<object> args)
            {
                await Model.Initialize(args);
                await View.Initialize(args);
                // HoleCanMagnet = LevelManager.Instance.GetHoleCanMagnet();
                // MagnetButtons = new List<UIMaskButton>();
                // float scale = Scale.GetValue(CameraManager.Instance.GetCameraHeightZ());
                // for (int i = 0; i < HoleCanMagnet.Count; i++)
                // {
                //     Hole hole = HoleCanMagnet[i];
                //     int capturedIndex = i;
                //     for (int j = 0; j < hole.Size.Length; j++)
                //     {
                //         UIMaskButton button = Object.Instantiate(View.ButtonSelectPrefab, View.ButtonSelectPrefab.transform.parent);
                //         button.gameObject.SetActive(true);
                //         button.transform.position = CameraManager.Instance.WorldToScreenPoint(HoleCanMagnet[i].transform.position + hole.Size[j]);
                //         button.transform.localScale = Vector3.one * scale;
                //         MagnetButtons.Add(button);
                //         button.SetMainButtonParent(View.ButtonSelectContainer);
                //         button.SetOnClickDestination(OnClickAction);
                //     }
                //     continue;
                //     UniTask OnClickAction() => OnClickButtonSelect(capturedIndex);
                // }
            }
            public async UniTask OnClickButtonSelect(int index)
            {
                // HoleCanMagnet[index].UseMagnet();
                // AudioManager.Instance.PlaySoundFx(AudioKey.SfxMagnet);
                // await ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivityUseBoosterMagnet));
                // await UniTask.Delay(2000, cancellationToken: HoleCanMagnet[index].GetCancellationTokenOnDestroy());
                // LevelManager.Instance.SetPause(false);
            }
        }
    }
}
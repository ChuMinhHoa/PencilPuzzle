using System;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.OnGameManager;
using _Game.Scripts.UI.Core;
using Core.UI;
using Core.UI.Screens;
using CoreData;
using Cysharp.Threading.Tasks;
using LitMotion;
using R3;
using Sirenix.OdinInspector;
using TMPro;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using TW.UGUI.MVPPattern;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGame.Scripts.UI.Modals
{
    public class ModalWin : Modal
    {
        [field: SerializeField] public ModalWinContext.UIPresenter UIPresenter { get; private set; }

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
    public class ModalWinContext
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
            [field: SerializeField] public Button ButtonClaimWithAds {get; private set;}
            [field: SerializeField] public Button ButtonClaim {get; private set;}
            [field: SerializeField] public List<StarGroup> StarGroups {get; private set;}
            [field: SerializeField] public CanvasGroup TitleCG {get; private set;}
            [field: SerializeField] public TextMeshProUGUI DefaultRewardTxt {get; private set;}
            [field: SerializeField] public TextMeshProUGUI AdsRewardTxt {get; private set;}
            public UniTask Initialize(Memory<object> args)
            {
                DefaultRewardTxt.text = $"{DefaultGlobalConfig.Instance.WinGameReward}";
                AdsRewardTxt.text = $"{DefaultGlobalConfig.Instance.WinGameReward * 2}";
                RunAnim().Forget();
                return UniTask.CompletedTask;
            }
            
            [Button]
            public async UniTask RunAnim()
            {
                ButtonClaimWithAds.transform.localScale = Vector3.zero;
                ButtonClaim.transform.localScale = Vector3.zero;
                TitleCG.alpha = 0f;
                Vector3 titlePos = TitleCG.transform.position;
                Vector3 titlePosStart = TitleCG.transform.position - Vector3.up * 250;
                TitleCG.transform.position = titlePosStart;
                LMotion.Create(0f, 1f, 0.35f).Bind(x => TitleCG.alpha = x).AddTo(MainView);
                LMotion.Create(titlePosStart, titlePos, 0.35f).Bind(x => TitleCG.transform.position = x).AddTo(MainView);
                StarGroups[0].RunAnim().Forget();
                StarGroups[1].RunAnim().Forget();
                StarGroups[2].RunAnim().Forget();
                await UniTask.Delay(350);
                if (GameManager.Instance.currentLevel.Value > DefaultGlobalConfig.Instance.DefaultBackToMenuLevel)
                {
                    await LMotion.Create(0f, 1f, 0.2f).WithEase(Ease.OutBack).Bind(x => ButtonClaimWithAds.transform.localScale = x * Vector3.one).AddTo(MainView);
                    await UniTask.Delay(1000, cancellationToken: MainView.GetCancellationTokenOnDestroy());
                }
                else
                {
                    ButtonClaim.transform.position = ButtonClaimWithAds.transform.position;
                }
                await LMotion.Create(0f, 1f, 0.2f).WithEase(Ease.OutBack).Bind(x => ButtonClaim.transform.localScale = x * Vector3.one).AddTo(MainView);
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
                AudioManager.Instance.PlaySoundFx(AudioKey.SfxUIWinGame);
                await Model.Initialize(args);
                await View.Initialize(args);
                View.ButtonClaim.SetOnClickDestination(OnClickButtonClaim);
                View.ButtonClaimWithAds.SetOnClickDestination(OnClickButtonClaimWithAds);
            }
            private async UniTask OnClickButtonClaimWithAds()
            {
                View.MainView.interactable = false;
                await UIAnimationBase.ButtonBasic(View.ButtonClaimWithAds.transform);
                //InGameAdsController.EventShowAdsReward?.Invoke("AdsRw_GetWinReward", OnClaimWithAdsSuccess, OnClaimWithAdsFail);
                #if Unity_Editor
                OnClaimWithAdsSuccess();
                #endif
                
            }
            void OnClaimWithAdsSuccess()
            {
                //InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Money, DefaultGlobalConfig.Instance.WinGameReward * 2);
                OnClaimWithAdsSuccessAsync().Forget();
            }
            
            async UniTask OnClaimWithAdsSuccessAsync()
            {
                await DoneGetReward();
            }
            
            private void OnClaimWithAdsFail()
            {
                View.MainView.interactable = true;
            }
            
            private async UniTask OnClickButtonClaim()
            {
                PlayerResourceManager.Instance.AddResourceValue(ResourceType.Currency, (int)CurrencyType.Money, DefaultGlobalConfig.Instance.WinGameReward);
                await UIAnimationBase.ButtonBasic(View.ButtonClaim.transform);
                //InGameAdsController.EventShowAdsInter?.Invoke("AdsInter_WinLevel", null);
                Debug.LogError("AdsInter_WinLevel");
                await DoneGetReward();
            }

            async UniTask DoneGetReward()
            {
                if (GameManager.Instance.currentLevel.Value <= DefaultGlobalConfig.Instance.DefaultBackToMenuLevel)
                {
                    await LoadNextLevel();
                }
                else
                {
                    await BackToMainMenu();
                }
            }
            
            public async UniTask BackToMainMenu()
            {
                ViewOptions activityLoading = new ViewOptions(nameof(ActivityLoading));
                Memory<object> args = new Memory<object>(new object[]
                {
                    (Func<UniTask>)(async () =>
                    {
                        ScreenOptions screenOptions = new ScreenOptions(nameof(ScreenMainMenu), stack: false);
                        await ScreenContainer.Find(ContainerKey.Screens).PushAsync(screenOptions);
                        await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
                        GameManager.Instance.ClearLevel();
                        //LevelManager.Instance.ClearLevel();
                    }),
                    null
                });
                await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(activityLoading, args);
            }
            
            public async UniTask LoadNextLevel()
            {
                ViewOptions activityLoading = new ViewOptions(nameof(ActivityLoading));
                Memory<object> args = new Memory<object>(new object[]
                {
                    (Func<UniTask>)(async () =>
                    {
                        await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
                        GameManager.Instance.ClearLevel();
                        //LevelManager.Instance.ClearLevel();
                        await GameManager.Instance.LoadCurrentLevel();
                      
                    }),
                    null
                });
                await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(activityLoading, args);
            }
        }
    }
}
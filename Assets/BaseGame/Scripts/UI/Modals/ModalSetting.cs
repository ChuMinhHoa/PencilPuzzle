using System;
using _Game.Scripts.Manager;
using BaseGame.Scripts.UI.Other.MainMenuTab;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using UnityEngine.UI;

namespace Core.UI.Modals
{
    public class ModalSetting : Modal, IUITabLifecycleEvent
    {
        [field: SerializeField] public ModalSettingContext.UIPresenter UIPresenter { get; private set; }
        
        public override async UniTask Initialize(Memory<object> args)
        {
            await UIPresenter.Initialize(args);
        }

        public void OnTabEnter(Memory<object> args)
        {
            UIPresenter.DidPushEnter(args);
        }

        public void OnTabExit(Memory<object> args)
        {
            UIPresenter.DidPushExit(args);
        }
    }


    [Serializable]
    public class ModalSettingContext
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
            [field: SerializeField] public bool ActionSelected { get; set; }

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
            [field: SerializeField] public CanvasGroup LanguagePopup {get; private set;}  
            //[field: SerializeField] public ReactiveToggleButton MusicToggleButton {get; private set;} 
            //[field: SerializeField] public ReactiveToggleButton SoundToggleButton {get; private set;} 
            //[field: SerializeField] public ReactiveToggleButton VibrationToggleButton {get; private set;}
            [field: SerializeField] public CustomButton RestoreBtn {get; set;}
            //[field: SerializeField] public CustomButton LanguageBtn {get; set;}
            [field: SerializeField] public Button ShowInterBtn {get; private set;}
            [field: SerializeField] public Button ShowBannerBtn {get; private set;}
            [field: SerializeField] public Button HideBannerBtn {get; private set;}

            public UniTask Initialize(Memory<object> args)
            {
                return UniTask.CompletedTask;
            }
            
            void SetupLanguageDropdown()
            {
                // if(languageInited)
                //     return;
                // languageInited = true;
                // languageBtnPool.OnInit(LanguageBtnPrefab, 1, languageBtnParrent);
                // listLanguage = LanguageGlobalConfig.Instance.listLanguage;
                // for (var i = 0; i < listLanguage.Count; i++)
                // {
                //     LanguageBtn languageBtn = languageBtnPool.Spawn();
                //     languageBtn.Init(listLanguage[i].ToString(), HideLanguagePopup);
                // }
            }

            void HideLanguagePopup()
            {
                LanguagePopup.alpha = 0f;
                LanguagePopup.interactable = false;
                LanguagePopup.blocksRaycasts = false;
            }

            public void ShowLanguagePopup()
            {
                SetupLanguageDropdown();
                LanguagePopup.alpha = 1f;
                LanguagePopup.interactable = true;
                LanguagePopup.blocksRaycasts = true;
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
                //View.RestoreBtn.AddListener(RestorePurchase);
                // View.LanguageBtn.AddListener(View.ShowLanguagePopup);
                // View.MusicToggleButton.AddListener(InGameDataManager.Instance.InGameData.SettingData.GetSettingSubData(SettingType.Music).Value, SetSettingValue, SettingType.Music);
                // View.SoundToggleButton.AddListener(InGameDataManager.Instance.InGameData.SettingData.GetSettingSubData(SettingType.Sound).Value, SetSettingValue, SettingType.Sound);
                // View.VibrationToggleButton.AddListener(InGameDataManager.Instance.InGameData.SettingData.GetSettingSubData(SettingType.Vibration).Value, SetSettingValue, SettingType.Vibration);
            
                // View.ShowInterBtn.onClick.AddListener(ShowInter);
                // View.ShowBannerBtn.onClick.AddListener(ShowBanner);
                // View.HideBannerBtn.onClick.AddListener(HideBanner);
            }
            // public void SetSettingValue(SettingType type, bool value)
            // {
            //     InGameDataManager.Instance.InGameData.SettingData.SetSettingValue(type, value);
            // }

            void ShowInter()
            {
                //InGameAdsController.EventShowAdsInter?.Invoke("TestInter", null);
            }
            void ShowBanner()
            {
                //InGameAdsController.EventShowBanner?.Invoke();
            }
            void HideBanner()
            {
                //InGameAdsController.EventHideBanner?.Invoke();
            }   
        
            public void SetActionSelected()
            {
                Model.ActionSelected = true;
                LMotion.Create(0, 1f, 1.5f).WithOnComplete(() => { Model.ActionSelected = false; }).RunWithoutBinding().AddTo(View.MainView);
            }

            void RestorePurchase()
            {
                //Purchaser.Instance.RestorePurchases();
            }

            void HideUI()
            {
                // CheatUI.OnToggleUIEvent?.Invoke(true);
                // OnCloseModal();
            }
        
            public void DidPushEnter(Memory<object> args)
            {
                
            }
            
            public void DidPushExit(Memory<object> args)
            {
                
            }
        }
    }
}
using System;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.ScriptAbleObject;
using _Game.Scripts.UI.Core;
using BaseGame.Scripts.UI.Modals;
using Core.UI.Activities;
using Core.UI.Modals;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Game.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public bool _inGame;
        public LevelManager currentLevelManager;
        public bool isCanTouch = true;
        private PencilBase currentUnit;
        public ReactiveValue<int> currentLevel = new();
        public Camera mainCamera;
        public GameObject objLoadingFake;
        public LayerMask layerMask;

        private void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            //currentLevel = .Instance.PlayerLevel;
            if (currentLevel.Value == 0)
            {
                currentLevel.Value = 1;
                InGameDataManager.Instance.SaveData();
            }

            currentLevel.ReactiveProperty.Subscribe(ChangeLevel).AddTo(this);
            //_ = LoadCurrentLevel();
        }

        private void ChangeLevel(int levelChange)
        {
            InGameDataManager.Instance.SaveData();
        }

        #region Touch Controll

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && isCanTouch && !currentUnit &&
                !EventSystem.current.IsPointerOverGameObject())
            {
                if(!currentLevelManager) return;
                
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
                {
                    if (!currentLevelManager.isGamePlay) currentLevelManager.SetOnGamePlay();
                    currentUnit = currentLevelManager.pencilController.GetUnitByCollider(hit.collider);
                    if (currentUnit)
                    {
                        SetCanTouch(false);
                        currentUnit.TryMoveOut();
                        currentUnit = null;
                    }
                }
            }

// #if PLATFORM_ANDROID || PLATFORM_IOS
//             if (Input.touchCount > 0 && isCanTouch && !currentUnit && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
//             {
//                 var ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
//                 if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
//                 {
//                     currentUnit = currentLevelManager.pencilController.GetUnitByCollider(hit.collider);
//                     if (currentUnit)
//                     {
//                         SetCanTouch(false);
//                         currentUnit.TryMoveOut();
//                         currentUnit = null;
//                     }
//                 }
//             }
// #endif

        }

        public void CheckCanTouch()
        {
            SetCanTouch(currentLevelManager.pencilController.CheckCanTouch());
        }

        public void SetCanTouch(bool canTouch)
        {
            isCanTouch = canTouch;
        }

        #endregion

        #region Level Control


        [Button]
        public async UniTask LoadCurrentLevel()
        {
            //await ShowLoadingPanel();
            Debug.Log(currentLevel.Value);
            var levelConfig = LevelGlobalConfig.Instance.GetLevelConfig(currentLevel.Value);
            var handle = Addressables.LoadAssetAsync<GameObject>(levelConfig.levelPrefabReference);
            handle.Completed += task =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    currentLevelManager = Instantiate(task.Result).GetComponent<LevelManager>();
                    _ = currentLevelManager.InitData(levelConfig);
                    InitCamera(levelConfig);
                }
            };
            await handle.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        private async UniTask ShowLoadingPanel()
        {
            var option = new ViewOptions(nameof(ActivityLoading));
            await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(option, true, null, null);
            objLoadingFake.SetActive(false);
        }

        public async UniTask ReplayLevel()
        {
            await currentLevelManager.ReplayLevel();
        }

        [Button]
        public void LevelComplete()
        {
            currentLevel.Value++;
            if (currentLevel.Value > LevelGlobalConfig.Instance.GetMaxLevel())
            {
                currentLevel.Value = 1;
            }

            if (currentLevelManager)
                Destroy(currentLevelManager.gameObject);
            currentLevelManager = null;
            _ = ShowModalWin();
        }

        private async UniTask ShowModalWin()
        {
            var viewOptions = new ViewOptions(nameof(ModalWin));
            await ModalContainer.Find(ContainerKey.Modals).PushAsync(viewOptions);
        }

        private void InitCamera(LevelConfig levelConfig)
        {
            mainCamera.orthographicSize = levelConfig.cameraSize;
            mainCamera.transform.position = levelConfig.cameraPosition;
        }

        #endregion

        public void SetPause(bool active)
        {
            currentLevelManager?.SetPause(active);
        }

        public void ClearLevel()
        {
            //currentLevelManager?.ClearLevel();
        }
    }
}

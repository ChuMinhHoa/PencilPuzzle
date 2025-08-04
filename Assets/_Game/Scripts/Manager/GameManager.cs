using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.UI.Core;
using Core.UI.Activities;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Views;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Game.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public LevelManager currentLevelManager;
        public bool isCanTouch = true;
        private PencilBase currentUnit;
        public ReactiveValue<int> currentLevel = new();
        public Camera mainCamera;
        public GameObject objLoadingFake;
        private void Start()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
            currentLevel = PlayerDataSave.Instance.PlayerLevel;
            if (currentLevel.Value  == 0)
            {
                currentLevel.Value = 1;
                InGameDataManager.Instance.SaveData();
            }

            currentLevel.ReactiveProperty.Subscribe(ChangeLevel).AddTo(this);
            _ = SpawnLevel();
        }

        private void ChangeLevel(int levelChange)
        {
            InGameDataManager.Instance.SaveData();
        }

        #region Touch Controll

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && isCanTouch && !currentUnit)
            {
                var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    currentUnit = currentLevelManager.pencilController.GetUnitByCollider(hit.collider);
                    if (currentUnit)
                    {
                        SetCanTouch(false);
                        currentUnit.TryMoveOut();
                        currentUnit = null;
                    }
                }
            }
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
        private async UniTask SpawnLevel()
        {
            await ShowLoadingPanel();
            var levelConfig = LevelGlobalConfig.Instance.GetLevelConfig(currentLevel.Value);
            var handle = Addressables.LoadAssetAsync<GameObject>(levelConfig.levelPrefabReference);
            handle.Completed += task =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    currentLevelManager = Instantiate(task.Result).GetComponent<LevelManager>();
                    _ = currentLevelManager.InitData(levelConfig);
                }
            };
            await handle.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
           
        }

        private async UniTask ShowLoadingPanel()
        {
            var option = new ViewOptions(nameof(ActivityLoading));
            await ActivityContainer.Find(ContainerKey.Activities).ShowAsync(option, true);
            objLoadingFake.SetActive(false);
        }

        public void ReplayLevel()
        {
            _ = currentLevelManager.ReplayLevel();
        }

        [Button]
        public void LevelComplete()
        {
            currentLevel.Value++;
            if (currentLevel.Value > LevelGlobalConfig.Instance.GetMaxLevel())
            {
                currentLevel.Value = 1;
            }
            if(currentLevelManager)
                Destroy(currentLevelManager.gameObject);
            currentLevelManager = null;
            _ = SpawnLevel();
        }

        #endregion
        
    }
}

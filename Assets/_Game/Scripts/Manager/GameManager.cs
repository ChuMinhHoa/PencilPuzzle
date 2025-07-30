using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.Utility.DesignPattern;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace _Game.Scripts.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        public LevelManager currentLevelManager;
        public bool isCanTouch = true;
        private UnitBase currentUnit;
        public ReactiveValue<int> currentLevel = new();

        private void Start()
        {
            currentLevel.Value = 1;

            _ = SpawnLevel();
        }

        #region Touch Controll

        private void Update()
        {
            if (Input.GetMouseButtonDown(0) && isCanTouch && !currentUnit)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    currentUnit = currentLevelManager.unitController.GetUnitByCollider(hit.collider);
                    if (currentUnit)
                    {
                        currentUnit.TryMoveOut();
                        SetCanTouch(false);
                        currentUnit = null;
                    }
                }
            }
        }

        public void CheckCanTouch()
        {
            SetCanTouch(currentLevelManager.unitController.CheckCanTouch());
        }

        public void SetCanTouch(bool canTouch)
        {
            isCanTouch = canTouch;
        }

        #endregion

        #region Level Control
        private async UniTask SpawnLevel()
        {
            Debug.Log("SpawnLevel");
            var levelConfig = LevelGlobalConfig.Instance.GetLevelConfig(currentLevel.Value);
            var handle = Addressables.LoadAssetAsync<GameObject>(levelConfig.levelPrefabReference);
            handle.Completed += task =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    currentLevelManager = Instantiate(task.Result).GetComponent<LevelManager>();
                    Debug.Log(currentLevelManager);
                    Debug.Log($"{levelConfig}");
                    _ = currentLevelManager.InitData(levelConfig);
                    //currentLevelManager = task.Result.GetComponent<LevelManager>();
                }
            };
            await handle.ToUniTask(cancellationToken: this.GetCancellationTokenOnDestroy());
           
        }

        public void ReplayLevel()
        {
            _ = currentLevelManager.ReplayLevel();
        }

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

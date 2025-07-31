using System;
using _Game.Scripts.FTool;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager.Controller;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Manager
{
    public class LevelManager : MonoBehaviour
    {
        public int level = -1;
        public SharpenerController sharpenerController;

        [FormerlySerializedAs("unitController")]
        public PencilController pencilController;

        public LevelConfig currentLevelConfig;
        public int currentWaveIndex;

        public UnitPositionConfig GetUnitPositionConfig(int unitId) => currentLevelConfig.GetLevelUnitPositionConfig(unitId);

        public void SharpenerEndAnimAndCheck(int sharpenerID) => sharpenerController.SharpenerEndAnimAndCheck(sharpenerID);

        public void ClearSharpenerPoint(Sharpener sharpener) => sharpenerController.ClearSharpenerPoint(sharpener);
        
        [Button]
        public async UniTask ReplayLevel()
        {
            currentLevelConfig = LevelGlobalConfig.Instance.GetLevelConfig(level);
            pencilController.ResetAllUnits();
            await pencilController.InitData();
            sharpenerController.ResetAllSharpeners();
            currentWaveIndex = 0;
            SpawnNextWave();
        }

        [Button]
        public async UniTask InitData(LevelConfig levelConfig)
        {
            currentLevelConfig = levelConfig;
            await pencilController.InitData();
            currentWaveIndex = 0;
            SpawnFirstWave();
        }

        private void SpawnFirstWave()
        {
            for (var i = 0; i < currentLevelConfig.startSharpenerCount; i++)
            {
                var colorType = currentLevelConfig.GetColorNext(currentWaveIndex);
                sharpenerController.SpawnSharpener(colorType, currentWaveIndex);
                
                currentWaveIndex++;
            }
        }

        [Button]
        public void SpawnNextWave()
        {
            Debug.Log("Get color!");
            var colorType = currentLevelConfig.GetColorNext(currentWaveIndex);
            if (colorType == SharpenerColorType.None && sharpenerController.currentSharpeners.Count == 0)
            {
                GameManager.Instance.LevelComplete();
                return;
            }
            if (colorType == SharpenerColorType.None)
            {
                Debug.Log("No more colors to spawn, but sharpeners still exist.");
                return;
            }
            Debug.Log("Spawn color: " + colorType);
            sharpenerController.SpawnSharpener(colorType, currentWaveIndex);
          
            currentWaveIndex++;
        }

        #region Resolve Unit
        public bool TryResolveUnit(PencilBase pencilBase)
        {
            var sharpener = sharpenerController.TryGetSharpener(pencilBase.colorType);
            if (!sharpener)
            {
                return TryResolveToTemp(pencilBase);
            }

            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                return TryResolveToTemp(pencilBase);
            }
            
            ResolveDone(sharpener.id, pointGoal, pencilBase);
            return true;
        }

        private bool TryResolveToTemp(PencilBase pencilBase)
        {
            var sharpener = sharpenerController.TryGetTempSharpener();
            if (!sharpener)
                return false;
            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                return false;
            }

            ResolveDone(sharpener.id, pointGoal, pencilBase);
            GameManager.Instance.currentLevelManager.pencilController.AddUnitToTemp(pencilBase.unitId);
            return true;
        }

        private void ResolveDone(int sharpenerID, PointGoal pointGoal, PencilBase pencilBase)
        {
            pointGoal.SetObjOnPoint(pencilBase.unitId);
            pencilBase.SetPointGoal(pointGoal);
            pencilBase.SetIDSharpener(sharpenerID);
        }
        #endregion

        [Button(50)]
        private void SaveLevelData()
        {
            if (currentLevelConfig == null)
            {
                Debug.Log($"Current level config is null, find or create config: Level_{level}");
                var path = "Assets/_Game/ScriptAbleObject/Level/Level_" + level + ".asset";
                var config = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                if (config == null)
                {
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<LevelConfig>(), path);
                    config = AssetDatabase.LoadAssetAtPath<LevelConfig>(path);
                }
                currentLevelConfig = config;
                currentLevelConfig.InitData(level);
            }

            pencilController.SaveConfig(currentLevelConfig);
        }

        public void ResolveUnit(int unitId) => pencilController.ResolveUnit(unitId);
    }
}

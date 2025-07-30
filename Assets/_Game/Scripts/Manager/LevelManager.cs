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

namespace _Game.Scripts.Manager
{
    public class LevelManager : MonoBehaviour
    {
        public int level;
        public SharpenerController sharpenerController;
        public UnitController unitController;
        public LevelConfig currentLevelConfig;
        public int currentWaveIndex;

        [Button]
        public async UniTask ReplayLevel()
        {
            currentLevelConfig = LevelGlobalConfig.Instance.GetLevelConfig(level);
            unitController.ResetAllUnits();
            await unitController.InitData();
            sharpenerController.ResetAllSharpeners();
            currentWaveIndex = 0;
            SpawnNextWave();
        }

        [Button]
        public async UniTask InitData(LevelConfig levelConfig)
        {
            currentLevelConfig = levelConfig;
            await unitController.InitData();
            currentWaveIndex = 0;
            SpawnNextWave();
        }

        [Button]
        private void SpawnNextWave()
        {
            var waveConfig = currentLevelConfig.GetWaveConfig(currentWaveIndex);
            if (waveConfig == null)
            {
                GameManager.Instance.LevelComplete();
                return;
            }
            sharpenerController.SpawnSharpener(waveConfig, currentWaveIndex);
          
            currentWaveIndex++;
        }

        public bool TryResolveUnit(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetSharpener(unitBase.colorType);
            if (!sharpener)
            {
                return TryResolveToTemp(unitBase);
            }

            var pointGoal = sharpener.TryGetPointGoal();
            if (!pointGoal)
            {
                return TryResolveToTemp(unitBase);
            }
            
            ResolveDone(sharpener.id, pointGoal, unitBase);
            return true;
        }

        private bool TryResolveToTemp(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetTempSharpener();
            if (!sharpener)
                return false;
            var pointGoal = sharpener.TryGetPointGoal();
            if (!pointGoal)
            {
                return false;
            }

            ResolveDone(sharpener.id, pointGoal, unitBase);
            GameManager.Instance.currentLevelManager.unitController.AddUnitToTemp(unitBase.unitId);
            return true;
        }

        private void ResolveDone(int sharpenerID, PointGoal pointGoal, UnitBase unitBase)
        {
            pointGoal.SetUnit(unitBase.unitId);
            unitBase.SetPointGoal(pointGoal);
            unitBase.SetIDSharpener(sharpenerID);
        }

        public UnitPositionConfig GetUnitPositionConfig(int unitId)
        {
            return currentLevelConfig.GetLevelUnitPositionConfig(unitId);
        }

        public void SharpenerEndAnimAndCheck(int sharpenerID)
        {
            sharpenerController.SharpenerEndAnimAndCheck(sharpenerID);
        }

        public void CheckToNextWave(int waveIndex)
        {
            if (waveIndex != currentWaveIndex - 1)
                return;
            var result = sharpenerController.IsDoneThatWave();
            if (result)
            {
                SpawnNextWave();
            }
        }
    }
}

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
        public int level;
        public SharpenerController sharpenerController;
        [FormerlySerializedAs("unitController")] public PencilController pencilController;
        public LevelConfig currentLevelConfig;
        public int currentWaveIndex;

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

        public bool TryResolveUnit(PencilBase pencilBase)
        {
            var sharpener = sharpenerController.TryGetSharpener(pencilBase.colorType);
            if (!sharpener)
            {
                return TryResolveToTemp(pencilBase);
            }

            var pointGoal = sharpener.TryGetPointGoal();
            if (!pointGoal)
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
            if (!pointGoal)
            {
                return false;
            }

            ResolveDone(sharpener.id, pointGoal, pencilBase);
            GameManager.Instance.currentLevelManager.pencilController.AddUnitToTemp(pencilBase.unitId);
            return true;
        }

        private void ResolveDone(int sharpenerID, PointGoal pointGoal, PencilBase pencilBase)
        {
            pointGoal.SetUnit(pencilBase.unitId);
            pencilBase.SetPointGoal(pointGoal);
            pencilBase.SetIDSharpener(sharpenerID);
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

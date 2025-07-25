using _Game.Scripts.GameManager.Controller;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.GameManager
{
    public class LevelManager : Singleton<LevelManager>
    {
        public int level;
        public SharpenerController sharpenerController;
        public UnitController unitController;
        public LevelConfig currentLevelConfig;
        public int currentWaveIndex;

        protected override void Awake()
        {
            base.Awake();
            _ = InitData();
        }

        [Button]
        private async UniTask InitData()
        {
            currentLevelConfig = LevelGlobalConfig.Instance.GetLevelConfig(level);
            await unitController.InitData();
            currentWaveIndex = 0;
        }

        [Button]
        private void SpawnNextWave()
        {
            var waveConfig = currentLevelConfig.GetWaveConfig(currentWaveIndex);
            sharpenerController.SpawnSharpener(waveConfig);
            currentWaveIndex++;
        }

        public bool TryResolveUnit(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetSharpener(unitBase.colorType);
            if (sharpener == null)
            {
                Debug.Log("null sharpener");
                return false;
            }

            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                Debug.Log("null point goal");
                return TryResolveToTemp(unitBase);
            }
            
            ResolveDone(sharpener.id, pointGoal, unitBase);
            return true;
        }

        private bool TryResolveToTemp(UnitBase unitBase)
        {
            var sharpener = sharpenerController.TryGetTempSharpener();
            if (sharpener == null)
                return false;
            var pointGoal = sharpener.TryGetPointGoal();
            if (pointGoal == null)
            {
                Debug.Log("null point goal");
                return false;
            }

            ResolveDone(sharpener.id, pointGoal, unitBase);
            return true;
        }

        private void ResolveDone(int sharpenerID, PointGoal pointGoal, UnitBase unitBase)
        {
            pointGoal.SetUnit(unitBase);
            
            unitBase.SetPointGoal(pointGoal.pointGoal);
            unitBase.SetIDSharpener(sharpenerID);
            
            Debug.Log("point goal set for unit: " + unitBase.name);
        }

        public UnitPositionConfig GetUnitPositionConfig(int unitId)
        {
            return currentLevelConfig.GetLevelUnitPositionConfig(unitId);
        }

        public void SharpenerEndAnimAndCheck(int sharpenerID)
        {
            sharpenerController.SharpenerEndAnimAndCheck(sharpenerID);
        }
    }
}

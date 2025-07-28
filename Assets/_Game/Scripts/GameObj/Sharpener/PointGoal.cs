using System;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Unit;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class PointGoal : MonoBehaviour
    {
        public UnitBase currentUnit;
        private Action _actionCallBack;

        public void SetActionCallBack(Action action) => _actionCallBack = action;
        [field: SerializeField] public Transform pointGoal { get; set; }
        [field: SerializeField] public Transform pointSpawnHit { get; set; }

        public void SetUnit(UnitBase unit)
        {
            currentUnit = unit;
        }
        
        public bool IsFree()
        {
            return currentUnit == null;
        }

        public void ClearPointGoal()
        {
            currentUnit = null;
        }

        public async UniTask OnHit()
        {
            var effect = PoolingObject.Instance.GetHitEffect(pointSpawnHit);
            effect.parent = pointSpawnHit;
            await UniTask.WaitForSeconds(0.5f);
            effect.gameObject.SetActive(false);
        }
    }
}

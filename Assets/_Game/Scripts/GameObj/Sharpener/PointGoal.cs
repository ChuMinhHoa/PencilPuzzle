using System;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager;
using Cysharp.Threading.Tasks;
using LitMotion;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class PointGoal : MonoBehaviour
    {
        public int currentUnitId = -1;
        private bool _isMoveDone;
        [field: SerializeField] public Transform pointGoal { get; set; }
        [field: SerializeField] public Transform pointSpawnHit { get; set; }

        public void SetUnit(int unitId)
        {
            currentUnitId = unitId;
        }
        
        public bool IsFree()
        {
            return currentUnitId == -1;
        }

        public void ClearPointGoal()
        {
            currentUnitId = -1;
            _isMoveDone = false;
            if (pointGoal.childCount > 0)
                for (var i = pointGoal.childCount - 1; i >= 0; i--)
                {
                    Destroy(pointGoal.GetChild(i).gameObject);
                }
        }

        public async UniTask OnHit()
        {
            var effect = PoolingObject.Instance.GetHitEffect(pointSpawnHit);
            effect.parent = pointSpawnHit;
            _isMoveDone = true;
            await UniTask.WaitForSeconds(0.5f);
            PoolingObject.Instance.DeSpawnHitEffect(effect);
        }

        public bool IsMoveDone()
        {
            return _isMoveDone;
        }
    }
}

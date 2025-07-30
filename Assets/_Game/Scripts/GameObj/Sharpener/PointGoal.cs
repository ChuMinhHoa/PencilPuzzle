using System;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
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

        public void ResetPointGoal()
        {
            currentUnitId = -1;
            _isMoveDone = false;
        }


        public void ClearLastPencilController()
        {
            if (pointGoal.transform.childCount > 0)
            {
                var lastPencilController = pointGoal.GetChild(0).GetComponent<LastPencilController>();
                if (lastPencilController)
                {
                    PoolingObject.Instance.DeSpawnLastPencilController(lastPencilController);
                }
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

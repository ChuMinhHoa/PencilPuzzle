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
    [System.Serializable]
    public class PointGoal : PointHaveObj<int>
    {
        private bool _isMoveDone;
        public Transform pointSpawnHit;

        public override void ResetObjOnPoint()
        {
            objOnPoint = -1;
            _isMoveDone = false;
        }

        public override bool isFree => objOnPoint == -1;


        public void ClearLastPencilController()
        {
            if (trsPoint.transform.childCount > 0)
            {
                var lastPencilController = trsPoint.GetChild(0).GetComponent<LastPencilController>();
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

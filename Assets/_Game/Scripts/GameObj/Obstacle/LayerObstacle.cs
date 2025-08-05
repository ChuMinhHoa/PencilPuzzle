using System;
using _Game.Scripts.GameObj.Interface;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.Manager.Etc;
using UnityEngine;

namespace _Game.Scripts.GameObj.Obstacle
{
    public class LayerObstacle : ConditionFace
    {
        public int totalCountUnlockLayer = 0;
        private int _currentCountUnlockLayer = 0;
        public override bool IsConditionSatisfied()
        {
            return _currentCountUnlockLayer == totalCountUnlockLayer;
        }

        public override void ResetCondition()
        {
            _currentCountUnlockLayer = 0;
        }

        public override void InitCondition()
        {
            ResetCondition();
            GameGlobalEvent.OnPencilResolve += OnPencilResolve;
        }

        private void OnDisable()
        {
            GameGlobalEvent.OnPencilResolve -= OnPencilResolve;
        }

        private void OnPencilResolve(PencilBase pencilBase)
        { 
            if (_currentCountUnlockLayer == totalCountUnlockLayer)
                return;
            _currentCountUnlockLayer++;
            if (IsConditionSatisfied())
            {
                Debug.Log("broken layer obstacle");
                gameObject.SetActive(false);
            }
        }
    }
}

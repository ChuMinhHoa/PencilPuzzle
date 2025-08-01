using System;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.GlobalConfig;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public enum SharpenerColorType
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Purple = 5,
        Orange = 6,
        Pink = 7,
        White = 8,
        Black = 9,
        ColorTemp = 10, 
        Hidden = 11
    }

    public class Sharpener : MonoBehaviour
    {
        public int id;

        //public int waveIndex;
        [ShowInInspector] private bool _cleared; 

        public SharpenerColorType sharpenerColorType;

        public MeshRenderer sharpenerMesh;

        private MotionHandle _moveHandle;

        public Animator anim;

        public List<PointGoal> pointGoals = new();

        public virtual PointGoal TryGetPointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (pointGoals[i].isFree)
                {
                    return pointGoals[i];
                }
            }

            return null;
        }

        public virtual void ResetPointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ResetObjOnPoint();
            }
                
            GameManager.Instance.currentLevelManager.sharpenerController.RemoveSharpener(this);
        }

        public bool CheckSharpenerCanMoveTo(SharpenerColorType colorType)
        {
            return sharpenerColorType == colorType && IsHaveAtLeastOneFreePointGoal();
        }

        private bool IsHaveAtLeastOneFreePointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (pointGoals[i].isFree)
                {
                    return true;
                }
            }

            return false;
        }

        [Button]
        public void InitData(SharpenerColorType colorType)
        {
            id = transform.GetSiblingIndex();
            sharpenerColorType = colorType;
            sharpenerMesh.material = UnitGlobalConfig.Instance.GetTipMaterial(sharpenerColorType);
            //waveIndex = currentWaveIndex;
            ClearLastPencilController();
        }

        public void AnimMove(Transform trsTarget, Action onFinished = null)
        {
            TryCancelMove();
            _moveHandle = LMotion.Create(transform.position, trsTarget.position, UnitGlobalConfig.Instance.timeSharpenerMove)
                .WithOnComplete(() => { onFinished?.Invoke(); })
                .Bind(x => transform.position = x)
                .AddTo(this);
        }

        public void TryCancelMove()
        {
            if (_moveHandle.IsPlaying())
            {
                _moveHandle.TryCancel();
            }
        }

        private async UniTask CheckClear()
        {
            if (_cleared)
                return;
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (!pointGoals[i].IsMoveDone())
                    return;
            }

            _cleared = true;
            PlayAnimRoll();
            await UniTask.WaitForSeconds(UnitGlobalConfig.Instance.timeSharpenerRoll);
            ResetPointGoal();
            GameManager.Instance.currentLevelManager.ClearSharpenerPoint(this);
            GameManager.Instance.currentLevelManager.SpawnNextWave();
        }

        public virtual async UniTask AnimDone()
        {
            PlayAnimGoal();
            await UniTask.WaitForSeconds(UnitGlobalConfig.Instance.timeAnimGoal);
            await CheckClear();
        }

        #region Animation

        public void PlayAnimGoal() => anim.Play(MyCache.AnimHit);
        private void PlayAnimRoll() => anim.Play(MyCache.AnimRoll);
        private void PlayAnimIdle() => anim.Play(MyCache.AnimIdle);

        #endregion

        public void ResetSharpener()
        {
            PlayAnimIdle();
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ResetObjOnPoint();
            }

            _cleared = false;
        }
        
        public void ClearLastPencilController()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ClearLastPencilController();
            }
        }
    }
}

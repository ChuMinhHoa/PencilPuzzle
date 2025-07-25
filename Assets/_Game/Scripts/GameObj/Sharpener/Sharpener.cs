using System;
using System.Collections.Generic;
using _Game.Scripts.GameManager;
using _Game.Scripts.GlobalConfig;
using LitMotion;
using Sirenix.OdinInspector;
using SplineMesh;
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
    }

    public class Sharpener : MonoBehaviour
    {
        public int id;
        
        public SharpenerColorType sharpenerColorType;

        public MeshRenderer sharpenerMesh;

        private MotionHandle _moveHandle;
        //public ExampleContortAlong contortAlong;

        public List<PointGoal> pointGoals = new();

        public PointGoal TryGetPointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (pointGoals[i].IsFree())
                {
                    return pointGoals[i];
                }
            }

            return null;
        }

        public void ClearSharpener()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ClearPointGoal();
            }
            LevelManager.Instance.sharpenerController.RemoveSharpener(this);
            //contortAlong.gameObject.SetActive(true);
            //contortAlong.Play();
        }

        public bool IsSameColor(SharpenerColorType colorType)
        {
            return sharpenerColorType == colorType;
        }

        [Button]
        public void InitData(SharpenerColorType colorType)
        {
            id = transform.GetSiblingIndex();
            sharpenerColorType = colorType;
            sharpenerMesh.material = UnitGlobalConfig.Instance.GetTipMaterial(sharpenerColorType);
        }

        public void AnimMove(Transform trsTarget, Action onFinished = null)
        {
            TryCancelMove();
            _moveHandle = LMotion.Create(transform.position, trsTarget.position, .5f)
                .WithOnComplete(() =>
                {
                    onFinished?.Invoke();
                })
                .WithEase(Ease.OutBack)
                .Bind(x => transform.position = x);
        }

        public void TryCancelMove()
        {
            if (_moveHandle.IsPlaying())
            {
                _moveHandle.TryCancel();
            }
        }

        private void CheckClear()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (pointGoals[i].IsFree())
                    return;
            }
            
            ClearSharpener();
        }

        public void AnimDone()
        {
            Debug.Log("Anim Move Done");
            CheckClear();
        }
    }
}

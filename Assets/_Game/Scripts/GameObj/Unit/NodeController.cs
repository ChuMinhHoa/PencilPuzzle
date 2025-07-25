using System;
using System.Collections.Generic;
using _Game.Scripts.GlobalConfig;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Unit
{
    public enum NodeControllerState
    {
        Idle = 0,
        MoveOut = 1,
        MoveOutHit = 2,
        MoveBack = 3
    }
    [Serializable]
    public class NodeController
    {
        public NodeControllerState currentState;
        public int splineIndex;
        public GameObject objTokenCancelMove;
        public SplineNode splineNode;
        public Vector3 currentPosition = new();
        public List<float3> pointMoves = new();
        public float speed;

        private Action _moveDoneCallback;
        private Action<float3, float3> _onMoveUpdateCallback;

        public float3 defaultPosition;
        public bool isHit = false;

        public NodeController(SplineNode node, GameObject gameObject, float speedChange, int splineIndex)
        {
            splineNode = node;
            objTokenCancelMove = gameObject;
            speed = speedChange;
            defaultPosition = node.Position;
            currentPosition = node.Position;
            this.splineIndex = splineIndex;
        }
        
        public void ChangeState(NodeControllerState newState)
        {
            currentState = newState;
        }

        public Action moveDoneCallback
        {
            get => _moveDoneCallback;
            set => _moveDoneCallback = value;
        }
        
        public Action<float3, float3> moveUpdateCallback
        {
            get => _onMoveUpdateCallback;
            set => _onMoveUpdateCallback = value;
        }
        
        public void SetPathPoints(List<float3> pathPoints)
        {
            pointMoves.Clear();
            pointMoves.AddRange(pathPoints);
            _pointIndex = 0; 
        }

        private int _pointIndex = 0;
        private MotionHandle _moveHandle;

        public void MoveToNextPoint()
        {
            var distance = Vector3.Distance(currentPosition, pointMoves[_pointIndex]);
            
            var duration = distance / speed;
            if (_moveHandle.IsPlaying())
                _moveHandle.TryCancel();
            _moveHandle = 
                LMotion.Create(currentPosition, pointMoves[_pointIndex], duration)
                    .WithOnComplete(() =>
                    {
                        _pointIndex++;
                        if (_pointIndex < pointMoves.Count)
                        {
                            MoveToNextPoint();
                        }
                        else
                        {
                            if (!isHit && currentState == NodeControllerState.MoveOut)
                            {
                                splineNode.Up = Vector3.forward;
                            // Reset to the first point if needed
                            }
                            _moveDoneCallback?.Invoke();
                            _pointIndex = 0;
                            
                        }

                        if (_pointIndex == pointMoves.Count - 1)
                        {
                            if (isHit && splineIndex == 0 && currentState == NodeControllerState.MoveOutHit)
                            {
                                _ = ScaleHandle();
                            }  
                        }
                        
                         
                    }).WithEase(Ease.Linear)
                    .Bind(x =>
                        {
                            currentPosition = x;
                            splineNode.Position = x;
                            _onMoveUpdateCallback?.Invoke(x, splineNode.Direction);
                        }
                    ).AddTo(objTokenCancelMove);
        }

        [Button]
        private async UniTask ScaleHandle()
        {
            var vectorScaleHit = Vector2.one * UnitGlobalConfig.Instance.unitScaleHit;
            var duration = UnitGlobalConfig.Instance.unitScaleHitDuration;
            await LMotion.Create(splineNode.Scale, vectorScaleHit, duration/2).Bind(x =>splineNode.Scale = x).AddTo(objTokenCancelMove);
            await LMotion.Create(vectorScaleHit, Vector2.one, duration/2).Bind(x => splineNode.Scale = x).AddTo(objTokenCancelMove);
        }
        
        public void ReversePath()
        {
            pointMoves.Reverse();
            pointMoves.RemoveAt(0);
            pointMoves.Add(defaultPosition);
            _pointIndex = 0;
        }

        public void ClearPath()
        {
            splineNode.Up = Vector3.up;
            pointMoves.Clear();
        }

        public void TryCancelMove()
        {
            if (_moveHandle.IsPlaying())
            {
                _moveHandle.TryCancel();
                _pointIndex = 0; // Reset index to start from the first point
                currentPosition = splineNode.Position;
            }
        }

        public void SetUpMoveOut(Action actionMoveDoneCallBack, List<float3> pathPoints)
        {
            ChangeState(NodeControllerState.MoveOut);
            _moveDoneCallback = actionMoveDoneCallBack;
            SetPathPoints(pathPoints);
            isHit = false;
        }

        public void SetUpMoveOutFail(Action actionMoveDoneCallBack, List<float3> pathPoints)
        {
            ChangeState(NodeControllerState.MoveOutHit);
            _moveDoneCallback = actionMoveDoneCallBack;
            SetPathPoints(pathPoints);
            isHit = true;
        }
        
        public void SetUpMoveBack()
        {
            ChangeState(NodeControllerState.MoveBack);
            _moveDoneCallback = null;
            ReversePath();
            isHit = false;
        }
    }
}

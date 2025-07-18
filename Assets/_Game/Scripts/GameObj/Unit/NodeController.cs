
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    [System.Serializable]
    public class NodeController
    {
        public GameObject objTokenCancelMove;
        public SplineNode splineNode;
        public Vector3 currentPosition = new();
        public List<float3> pointMoves = new();
        public float speed;

        private Action _moveDoneCallback;
        private Action<float3, float3> _onMoveUpdateCallback;

        public float3 defaultPosition;

        public NodeController(SplineNode node, GameObject gameObject, float speedChange)
        {
            splineNode = node;
            objTokenCancelMove = gameObject;
            speed = speedChange;
            defaultPosition = node.Position;
            currentPosition = node.Position;
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
            Debug.Log(pathPoints.Count);
            pointMoves.Clear();
            pointMoves.AddRange(pathPoints);
            _pointIndex = 0; // Reset index to start from the first point
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
                            splineNode.Up = Vector3.forward;
                            _moveDoneCallback?.Invoke();
                            _pointIndex = 0; // Reset to the first point if needed
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
        
        
        

        public void MoveBack()
        {
            
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
    }
}

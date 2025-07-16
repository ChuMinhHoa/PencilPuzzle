using System;
using System.Collections.Generic;
using LitMotion;
using SplineMesh;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    [System.Serializable]
    public class NodeController
    {
        public UnityEngine.GameObject objTokenCancelMove;
        public SplineNode splineNode;
        public Vector3 currentPosition = new();
        public List<Vector3> pointMoves = new();
        public float speed = 1f;

        private void ChangePosition(Vector3 vectorChange)
        {
            Debug.Log($"node {splineNode.Position} change to {vectorChange}");
            splineNode.Position = vectorChange;
        }

        private Action _moveDoneCallback;
        public Action MoveDoneCallback
        {
            get => _moveDoneCallback;
            set => _moveDoneCallback = value;
        }
        
        public void SetPathPoints(List<Vector3> pathPoints)
        {
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
                            _moveDoneCallback?.Invoke();
                            _pointIndex = 0; // Reset to the first point if needed
                        }
                    })
                    .Bind(x =>
                    {
                        currentPosition = x;
                        splineNode.Position = x;
                    }
                    ).AddTo(objTokenCancelMove);
        }
    }
}

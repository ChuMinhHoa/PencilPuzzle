
using System;
using System.Collections.Generic;
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
        public List<Vector3> pointMoves = new();
        public float speed = 1f;

        private Action moveDoneCallback;
        private Action<float3, float3> onMoveUpdateCallback;
        public Action MoveDoneCallback
        {
            get => moveDoneCallback;
            set => moveDoneCallback = value;
        }
        
        public Action<float3, float3> MoveUpdateCallback
        {
            get => onMoveUpdateCallback;
            set => onMoveUpdateCallback = value;
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
                            moveDoneCallback?.Invoke();
                            _pointIndex = 0; // Reset to the first point if needed
                        }
                    })
                    .Bind(x =>
                    {
                        currentPosition = x;
                        splineNode.Position = x;
                        onMoveUpdateCallback?.Invoke(x, splineNode.Direction);
                    }
                    ).AddTo(objTokenCancelMove);
        }
    }
}

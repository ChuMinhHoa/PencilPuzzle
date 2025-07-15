using System.Collections.Generic;
using LitMotion;
using R3;
using Sirenix.OdinInspector;
using SplineMesh;
using TW.Reactive.CustomComponent;
using UnityEngine;

namespace _Game.Scripts.Unit
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField] private Transform trsHead;
        [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        public List<ReactiveValue<Vector3>> pointsSplineNode = new();
    
        [Button]
        private void Awake()
        {
            for (var i = 0; i < spline.nodes.Count; i++)
            {
                var point = spline.nodes[i].Position;
                pointsSplineNode.Add(new ReactiveValue<Vector3>(point));
            }

            for (var i = 0; i < pointsSplineNode.Count; i++)
            {
                pointsSplineNode[i].ReactiveProperty.Subscribe(ChangeValue).AddTo(this);
            }

            pointMove.ReactiveProperty.Subscribe(ChangeTarget).AddTo(this);
        }

        private void ChangeTarget(Vector3 value) {
            LMotion.Create(pointsSplineNode[0].Value, pointMove.Value, 1f)
                .Bind(x => pointsSplineNode[0].Value = x)
                .AddTo(this);
            for (var i = 1; i < pointsSplineNode.Count; i++)
            {
                var index = i;
                LMotion.Create(pointsSplineNode[i].Value, pointsSplineNode[i - 1].Value, 1f)
                    .Bind(x => pointsSplineNode[index].Value = x)
                    .AddTo(this);
            }
        }

        private void ChangeValue(Vector3 vectorChange)
        {
            var pointIndexChange = pointsSplineNode.FindIndex(x => x.Value == vectorChange);
            spline.nodes[pointIndexChange].Position = vectorChange;
            splineMeshTiling.CreateMeshes();
        }

        public ReactiveValue<Vector3> pointMove;

        public virtual void InitData()
        {
            
        }
    }
}

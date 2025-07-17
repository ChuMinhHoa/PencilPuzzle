using System.Collections.Generic;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class SplineOutController : MonoBehaviour
    {
        public Spline splineOut;

        public List<Vector3> GetPathOut(int pointIndex, Transform parents)
        {
            var pathPoints = new List<Vector3>();

            for (var i = 0; i < splineOut.nodes.Count-1; i++)
            {
                var point = splineOut.nodes[i].Position;
                var worldPoint = transform.TransformPoint(point);
                var newPoint = parents.InverseTransformPoint(worldPoint);
                pathPoints.Add(newPoint);
            }
            
            var newLastPoint = splineOut.nodes[^1].Position;
            var lastPoint = newLastPoint - Vector3.up * pointIndex;
            
            var worldLastPoint = transform.TransformPoint(lastPoint);
            var newLasPoint = parents.InverseTransformPoint(worldLastPoint);
            
            pathPoints.Add(newLasPoint);
            return pathPoints;
        }

        public float3 GetLastPointOut(Transform newParents)
        {
            var lastPoint = splineOut.nodes[^1].Position;
            var worldLastPoint = transform.TransformPoint(lastPoint);
            var newLastPoint = newParents.InverseTransformPoint(worldLastPoint);
            return newLastPoint;
        }

        public float3 GetLastPointDir(Transform newParents)
        {
            var dirPoint = splineOut.nodes[^1].Direction;
            var worldLastPoint = transform.TransformPoint(dirPoint);
            var newLasDir = newParents.InverseTransformPoint(worldLastPoint);
            return newLasDir;
        }
    }
}

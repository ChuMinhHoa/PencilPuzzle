using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class SplineOutController : MonoBehaviour
    {
        public Spline splineOut;

        public List<Vector3> GetPathOut(int pointIndex)
        {
            var pathPoints = new List<Vector3>();

            for (var i = 0; i < splineOut.nodes.Count-1; i++)
            {
                var newPoint = splineOut.nodes[i].Position;
                pathPoints.Add(newPoint);
            }
            var newLastPoint = splineOut.nodes[^1].Position;
            var lastPoint = newLastPoint - Vector3.up * pointIndex;
            pathPoints.Add(lastPoint);
            return pathPoints;
        }
    }
}

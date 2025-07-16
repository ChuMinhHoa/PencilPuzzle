using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

namespace _Game.Scripts.Unit
{
    public class SplineOutController : MonoBehaviour
    {
        public Spline splineOut;

        public List<Vector3> GetPathOut(int pointIndex)
        {
            var pathPoints = new List<Vector3>();

            for (var i = 0; i < splineOut.nodes.Count-1; i++)
            {
                pathPoints.Add(splineOut.nodes[i].Position);
            }
            
            var lastPoint = splineOut.nodes[^1].Position - Vector3.up * pointIndex;
            pathPoints.Add(lastPoint);
            return pathPoints;
        }
    }
}

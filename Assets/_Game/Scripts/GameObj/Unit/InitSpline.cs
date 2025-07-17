using System.Collections.Generic;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class InitSpline : MonoBehaviour
    {
        public void SetUpSpline(Spline spline, List<float3> pathPosition)
        {
            for (var i = 0; i < pathPosition.Count; i++)
            {
                if (i > spline.nodes.Count-1)
                {
                    var newNode = new SplineNode(Vector3.zero, Vector3.zero);
                    spline.AddNode(newNode);
                }
                spline.nodes[i].Position = pathPosition[i];
            }

            for (var i = spline.nodes.Count-1; i >= pathPosition.Count; i--)
            {
                spline.RemoveNode(spline.nodes[i]);
            }
        }
    }
}

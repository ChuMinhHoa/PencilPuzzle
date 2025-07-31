using System;
using System.Collections.Generic;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class PencilSplineController : MonoBehaviour
    {
        public GameObject objSpline;
        public Spline spline;
        public SplineOutController splineOut;
        public List<NodeController> nodes = new();
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        private Action alignHeader;
        private Action animComplete;
        private Action callResolveUnit;

        public void InitPathPointController(Action alignHeaderCallBack, Action animCompleteCallBack, Action callResolveUnitCallBack, float speed)
        {
            for (var i = 0; i < spline.nodes.Count; i++)
            {
                NodeController node = new(spline.nodes[i], gameObject, speed, i);
                nodes.Add(node);
            }

            nodes[0].moveUpdateCallback = alignHeaderCallBack;
            alignHeader = alignHeaderCallBack;
            animComplete = animCompleteCallBack;
            callResolveUnit = callResolveUnitCallBack;
        }

        #region Get Path Point

        /// <summary>
        /// Lấy các điểm di chuyển nếu không bị va chạm
        /// </summary>
        private List<float3> GetPathPoints(int nodeIndex)
        {
            var pathPoints = new List<float3>();
            pathPoints.AddRange(GetPathPointToOtherPoint(nodeIndex));
            pathPoints.AddRange(splineOut.GetPathOut(nodeIndex, transform));

            return pathPoints;
        }

        /// <summary>
        /// Lấy các điểm di chuyển đến điểm va chạm
        /// Nếu khoảng cách nhỏ hơn độ dài của spline thì cần phải xóa đi các điểm di chuyển
        /// </summary>
        public List<float3> GetPathPointToHit(int nodeIndex, float3 hit, bool getByHit = false,
            float distanceToHit = 0f)
        {
            var pathPoints = GetPathPointToOtherPoint(nodeIndex);
            pathPoints.Add(GetLastPointToHit(nodeIndex, hit));
            if (getByHit && distanceToHit < spline.nodes.Count / 2)
            {
                var countRemaining = (int)distanceToHit > 0.5f ? (int)(distanceToHit * 2) : 1;
                float3? lastPointRemove = null;
                for (var i = pathPoints.Count - 1; i >= 0; i--)
                {
                    if (pathPoints.Count > countRemaining)
                    {
                        if (i != pathPoints.Count - 1)
                            lastPointRemove = pathPoints[i];
                        pathPoints.RemoveAt(i);
                    }
                    else
                        break;
                }

                if (lastPointRemove != null)
                {
                    var lastPoint = GetLastPoint(lastPointRemove.Value, pathPoints[^1]);
                    pathPoints.Add(lastPoint);
                }
                // if (countRemaining == 1 && distanceToHit < 1)
                // {
                //     var lastPoint = GetPathPointNotMove(nodeIndex);
                //     if (pathPoints.Count > 0)
                //         pathPoints[^1] = lastPoint ?? pathPoints[^1];
                // }
                // else
                // {
                //    
                // }
            }

            return pathPoints;
        }

        /// <summary>
        /// Lấy các điểm di chuyển đến các điểm còn lại
        /// </summary>
        private List<float3> GetPathPointToOtherPoint(int nodeIndex)
        {
            var pathPoints = new List<float3>();
            for (var i = nodeIndex - 1; i >= 0; i--)
            {
                var newPoint = nodes[i].currentPosition;
                pathPoints.Add(newPoint);
            }

            return pathPoints;
        }

        /// <summary>
        /// Lấy điểm cuối cùng để dừng lại
        /// </summary>
        private float3 GetLastPointToHit(int nodeIndex, float3 hit)
        {
            Vector3 lastPoint = hit;
            var dir = (lastPoint - nodes[0].currentPosition).normalized;
            var distanceBtNode = UnitGlobalConfig.Instance.distanceBtNode;
            lastPoint -=
                dir * (distanceBtNode * nodeIndex) +
                dir * UnitGlobalConfig.Instance.sizeUnitHead /*- dir * (0.05f * (nodeIndex == 0 ? 0 : 1))*/;


            return lastPoint;
        }

        /// <summary>
        /// lấy điểm di chuyển cuối nếu khoảng cách nhỏ hơn 1
        /// </summary>>
        private float3? GetPathPointNotMove(int nodeIndex)
        {
            if (nodeIndex == 0) return null;
            var previousPosition = nodes[nodeIndex - 1].currentPosition;
            var dir = previousPosition - nodes[nodeIndex].currentPosition;
            dir.Normalize();
            var lastPoint = nodes[nodeIndex].currentPosition + dir * UnitGlobalConfig.Instance.distanceMoveToNearHit;
            return lastPoint;
        }

        private float3 GetLastPoint(Vector3 lastPointRemove, Vector3 lasPoint)
        {
            var dir = lasPoint - lastPointRemove;
            dir = dir.normalized;
            var vectorReturn = lasPoint - (dir * 0.15f);
            return vectorReturn;
        }

        #endregion
        
        public void SetUpSpline(List<float3> dataPathMesh) => InitSpline.SetUpSpline(spline, dataPathMesh);

        public void SetUpSplineOut(List<float3> dataWayOut) => InitSpline.SetUpSpline(splineOut.splineOut, dataWayOut);

        public void SetUpSplineMat(Material mat) => splineMeshTiling.material = mat;

        public void ClearNodes() => nodes.Clear();

        public Vector3 GetLastPointOut() => splineOut.GetLastPointOut(transform);

        public Vector3 GetSplineNodePosition(int nodeIndex) => spline.nodes[nodeIndex].Position;

        public Vector3 GetSplineTransformPoint(Vector3 position) => spline.transform.TransformPoint(position);

        public void SetObjSpline(bool active) => objSpline.SetActive(active);

        public void SetLastPointMoveOutY()
        {
            var length = spline.nodes.Count * 0.5f;
            splineOut.SetLastPointMoveOutY(length);
        }

        public void MoveOutFail(float3 hit, Action scaleHeadHit)
        {
            var distanceToHit = Vector3.Distance(GetSplineNodePosition(0), hit);
            for (var i = 0; i < nodes.Count; i++)
            {
                var pathPoints = GetPathPointToHit(i, hit,true, distanceToHit);
                nodes[i].SetUpMoveOutFail(MoveBack, pathPoints, scaleHeadHit);
                nodes[i].MoveToNextPoint();
            }
        }

        public void MoveOut()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                var pathPoints = GetPathPoints(i);
                nodes[i].SetUpMoveOut(ActionMoveDoneCallBack, pathPoints);
                nodes[i].MoveToNextPoint();
            }
        }
        
        private void ActionMoveDoneCallBack(int nodeIndex)
        {
            alignHeader();
            if (nodeIndex == nodes.Count- 1)
            {
                animComplete();
                callResolveUnit();
            }
        }
        
        /// <summary>
        /// Di chuyển ngược lại sau khi va chạm
        /// </summary>
        private void MoveBack()
        {
            for (var i = nodes.Count - 1; i >= 0; i--)
            {
                nodes[i].SetUpMoveBack(MoveBackDoneCallBack);
                nodes[i].MoveToNextPoint();
            }
        }
        
        private void MoveBackDoneCallBack(int nodeIndex)
        {
            if (nodeIndex == nodes.Count- 1)
            {
                GameManager.Instance.SetCanTouch(true);
            }
        }

        public void Reset()
        {
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].ClearPath();
            }
            objSpline.SetActive(true);
        }

        public float GetSplineLength() => spline.nodes.Count/2f;
    }
}

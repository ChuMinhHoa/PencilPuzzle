using System;
using System.Collections.Generic;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Sharpener;
using Sirenix.OdinInspector;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField] private Transform trsHead;
        [SerializeField] private Transform trsBottom;
        [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        [SerializeField] public SharpenerColorType colorType;
        
        public List<NodeController> nodes = new ();
        public SplineOutController splineOut;
        public float speed = 2f;

        private void Awake()
        {
            InitData();
            AlignHeaderWithFirstNode(trsHead, true);
            AlignHeaderWithFirstNode(trsBottom, false);
        }

        [Button]
        public virtual void InitData()
        {
            nodes.Clear();
            for (var i = 0; i < spline.nodes.Count; i++)
            {
                NodeController node = new()
                {
                    objTokenCancelMove = gameObject,
                    currentPosition = spline.nodes[i].Position,
                    splineNode = spline.nodes[i],
                    speed = speed
                };
                nodes.Add(node);
            }
            nodes[0].MoveUpdateCallback = AlignHeaderTransform;
            
            nodes[^1].MoveDoneCallback = ActionMoveDoneCallBack;
            nodes[^1].MoveUpdateCallback = AlignBottomTransform;
        }

        [Button]
        private void MoveOut()
        {
            if (LevelManager.Instance.TryResolveUnit(this))
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    nodes[i].SetPathPoints(GetPathPoints(i));
                    nodes[i].MoveToNextPoint();
                }
            }
        }

        private void ActionMoveDoneCallBack()
        {
            Debug.Log("all move done next step");
            AnimOnComplete();
        }
        
        private List<Vector3> GetPathPoints(int nodeIndex)
        {
            var pathPoints = new List<Vector3>();
            for (var i = nodeIndex - 1; i >= 0; i--)
            {
                var newPoint = nodes[i].currentPosition;
                pathPoints.Add(newPoint);
            }
            
            pathPoints.AddRange(splineOut.GetPathOut(nodeIndex));
            
            return pathPoints;
        }
        
        private void AlignHeaderTransform(float3 position, float3 dir)
        {
            trsHead.localPosition = position; 
            trsHead.LookAt(dir);
        }

        private void AlignBottomTransform(float3 position, float3 dir)
        {
            trsBottom.localPosition = position; 
            trsBottom.LookAt(dir);
        }

        [Button]
        private void Align()
        {
            AlignHeaderWithFirstNode(trsHead, true);
            AlignHeaderWithFirstNode(trsBottom, false);
        }

        private void AlignHeaderWithFirstNode(Transform trsLock, bool header = true)
        {
            if (spline.nodes.Count == 0)
            {
                Debug.LogError("Spline has no nodes to align with.");
                return;
            }
            var index = header ? 0 : spline.nodes.Count - 1;
            // Set the position of the header to the first node's position
            trsLock.localPosition = spline.nodes[index].Position;

            // Calculate the opposite direction of the first node's forward direction
            var oppositeDirection = spline.nodes[index].Direction;

            // Set the rotation of the header to face the opposite direction
            if (oppositeDirection != Vector3.zero)
            {
                trsLock.LookAt(oppositeDirection);
            }
        }

        #region Animation

        private void AnimOnBlock()
        {
        }
        
        private void AnimOnComplete()
        {
            //LMotion.Create(transform.position, )
        }

        #endregion
    }
}

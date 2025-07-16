using System;
using System.Collections.Generic;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.Unit;
using Sirenix.OdinInspector;
using SplineMesh;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class UnitBase : MonoBehaviour
    {
        [SerializeField] private Transform trsHead;
        [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        [SerializeField] public SharpenerColorType colorType;
        
        public List<NodeController> nodes = new ();
        public SplineOutController splineOut;
        public float speed = 2f;

        private void Awake()
        {
            InitData();
        }

        [Button]
        public virtual void InitData()
        {
            nodes.Clear();
            for (var i = 0; i < spline.nodes.Count; i++)
            {
                NodeController node = new();
                node.objTokenCancelMove = gameObject;
                node.currentPosition = spline.nodes[i].Position;
                node.splineNode = spline.nodes[i];
                node.speed = speed;
                nodes.Add(node);
                if (i == spline.nodes.Count - 1)
                {
                    node.MoveDoneCallback = ActionMoveDoneCallBack;
                }
            }
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
                pathPoints.Add(nodes[i].currentPosition);
            }
            
            pathPoints.AddRange(splineOut.GetPathOut(nodeIndex));
            
            return pathPoints;
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

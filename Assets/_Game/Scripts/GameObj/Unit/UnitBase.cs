using System;
using System.Collections.Generic;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.ScriptAbleObject;
using LitMotion;
using Sirenix.OdinInspector;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public enum UnitLengthType
    {
        None = 0,
        L1 = 1,
        L2 = 2,
        L3 = 3
    }

    public class UnitBase : MonoBehaviour
    {
        [Title("Unit define")] public int unitId;
        public UnitLengthType unitLength;
        public SharpenerColorType colorType;

        private UnitConfig _unitConfig;
        private UnitPositionConfig _unitPositionConfig;

        [Title("GameObject")] [SerializeField] private GameObject objSpline;

        [Title("Transform")] [SerializeField] private Transform trsHead;
        [SerializeField] private Transform trsBottom;
        [SerializeField] private Transform trsLastPencil;
        private Transform _trsGoal;

        [Title("Spline")] [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        public SplineOutController splineOut;
        public InitSpline initSpline;

        public List<NodeController> nodes = new();

        [Title("Animation")] public float speed = 2f;
        public float magnitude = 2f;

        public AnimationCurve curveComplete;
        public MeshRenderer lastPencilMeshRenderer;

        #region Init Data

        [Button]
        private void InitDataEditor()
        {
            _unitPositionConfig = LevelManager.Instance.GetUnitPositionConfig(unitId);

            colorType = _unitPositionConfig.unitColor;
            unitLength = _unitPositionConfig.unitLength;

            _unitConfig = UnitGlobalConfig.Instance.GetUnitConfig(unitLength);

            initSpline.SetUpSpline(spline, _unitPositionConfig.pathMesh);
            var mat = UnitGlobalConfig.Instance.GetUnitMaterial(colorType);
            splineMeshTiling.material = mat;
            lastPencilMeshRenderer.material = mat;
            AlignPencil();
            nodes.Clear();
            transform.position = _unitPositionConfig.position;
            transform.eulerAngles = _unitPositionConfig.eulerAngles;
        }

        public virtual void InitData()
        {
            //Spawn last pencil
            InitDataEditor();

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

            nodes[0].moveUpdateCallback = AlignHeaderTransform;

            nodes[^1].moveDoneCallback = ActionMoveDoneCallBack;
            nodes[^1].moveUpdateCallback = AlignBottomTransform;

            AlignHeaderWithFirstNode(trsHead, true);
            AlignHeaderWithFirstNode(trsBottom, false);
        }

        #endregion

        #region Move

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

            pathPoints.AddRange(splineOut.GetPathOut(nodeIndex, transform));

            return pathPoints;
        }

        public void SetPointGoal(Transform pointGoalPointGoal) => _trsGoal = pointGoalPointGoal;

        #endregion

        #region Align Head and Bottom

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

        [Button]
        private void AlignPencil()
        {
            var positionPencil = splineOut.GetLastPointOut(transform);
            positionPencil.y -= _unitConfig.size;

            trsLastPencil.transform.localPosition = positionPencil;
            trsLastPencil.eulerAngles = new float3(90, 0, 0);
        }

        private void AlignHeaderWithFirstNode(Transform trsLock, bool header = true)
        {
            if (spline.nodes.Count == 0)
            {
                Debug.LogError("Spline has no nodes to align with.");
                return;
            }

            var index = header ? 0 : spline.nodes.Count - 1;

            trsLock.localPosition = spline.nodes[index].Position;

            var oppositeDirection = spline.nodes[index].Direction;

            if (oppositeDirection != Vector3.zero)
            {
                trsLock.LookAt(oppositeDirection);
            }
        }

        #endregion

        #region Animation

        private MotionHandle _moveCompleteHandle;

        private void AnimOnBlock()
        {
        }

        private void AnimOnComplete()
        {
            Debug.Log("Animation complete for unit: " + gameObject.name);
            //LMotion.Create(transform.position, )

            objSpline.SetActive(false);
            trsLastPencil.gameObject.SetActive(true);
            if (_moveCompleteHandle.IsPlaying())
                _moveCompleteHandle.TryCancel();
            var progress = 0f;
            var duration = curveComplete.keys[^1].time;
            _moveCompleteHandle = LMotion.Create(trsLastPencil.position, _trsGoal.position, duration)
                .Bind(x =>
                {
                    progress = Mathf.Clamp01(progress / duration);
                    float3 position = x;
                    position.y += curveComplete.Evaluate(progress) * magnitude;

                    trsLastPencil.position = position;

                    progress += Time.deltaTime;
                })
                .AddTo(this);

            LMotion.Create(trsLastPencil.eulerAngles, new float3(-90, 0, 0), 0.25f)
                .Bind(x => trsLastPencil.eulerAngles = x)
                .AddTo(this);

            LMotion.Create(trsLastPencil.localScale, Vector3.one, 0.25f)
                .Bind(x => trsLastPencil.localScale = x)
                .AddTo(this);
        }

        #endregion

        public void ResetUnit()
        {
            trsLastPencil.gameObject.SetActive(false);
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].ClearPath();
            }

            objSpline.SetActive(true);
        }

        #region Save Data

        [Button]
        private void SaveData()
        {
            // Implement save logic here, e.g., saving unit state to a file or database
            Debug.Log($"Saving data for Unit ID: {unitId}, Length: {unitLength}, Color: {colorType}");
            var levelConfig = LevelManager.Instance.currentLevelConfig;
            if (levelConfig == null)
            {
                Debug.LogError("Current level config is null. Cannot save unit data.");
                return;
            }

            levelConfig.SaveUnitData(
                unitId,
                unitLength,
                colorType,
                spline.nodes,
                transform.eulerAngles,
                transform.position
            );
        }

        #endregion

        [Button]
        private void Update()
        {
            CheckCanMove();
        }

        public void CheckCanMove()
        {
            if (Physics.Linecast(trsHead.position, trsHead.position - trsHead.forward, out var hit))
            {
                //Debug.Log("Cannot move, hit: " + hit.collider.name);
                Debug.DrawLine(trsHead.position, hit.point, Color.red);
            }
            else
            {
                //Debug.Log("Can move");

                Debug.DrawLine(trsHead.position, trsHead.position - trsHead.forward, Color.green);
            }
        }
    }
}

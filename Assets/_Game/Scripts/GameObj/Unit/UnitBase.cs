using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
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
        [Title("Unit define")] 
        public int unitId;
        public int sharpenerID;
        public UnitLengthType unitLength;
        public SharpenerColorType colorType;

        private UnitConfig _unitConfig;
        private UnitPositionConfig _unitPositionConfig;

        [Title("GameObject")] 
        [SerializeField] private GameObject objSpline;
        [SerializeField] private MeshRenderer headMeshRenderer;

        [Title("Transform")] 
        [SerializeField] private Transform trsHead;
        [SerializeField] private Transform trsLastPencil;
        [SerializeField] private Transform trsCheckPoint;
        [SerializeField] private Transform lastPencilParents;
        private PointGoal _pointGoal;

        [Title("Spline")] 
        [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        [SerializeField] private SplineOutController splineOut;

        public List<NodeController> nodes = new();

        [Title("Animation")]
        public float speed = 2f;
        public float magnitude = 2f;

        public AnimationCurve curveComplete;
        
        [Title("Move out")]
        public float distanceCheck = 10f;
        public LastPencilController lastPencil;
        
        
        [Title("Collider")]
        public List<Collider> myColliders = new();
        
        #region Init Data

        [Button]
        public void InitDataEditor()
        {
            lastPencil.InitData(colorType, unitLength);
            _unitPositionConfig = GameManager.Instance.currentLevelManager.GetUnitPositionConfig(unitId);

            colorType = _unitPositionConfig.unitColor;
            unitLength = _unitPositionConfig.unitLength;

            _unitConfig = UnitGlobalConfig.Instance.GetUnitConfig(unitLength);
            var unitHeadScale = UnitGlobalConfig.Instance.unitHeadScale;

            InitSpline.SetUpSpline(spline, _unitPositionConfig.pathMesh);
            InitSpline.SetUpSpline(splineOut.splineOut, _unitPositionConfig.wayOut);
            
            var mat = UnitGlobalConfig.Instance.GetUnitMaterial(colorType);
            splineMeshTiling.material = mat;
            AlignPencil();
            nodes.Clear();
            var headMat = UnitGlobalConfig.Instance.GetTipMaterial(colorType);
            headMeshRenderer.material = headMat;
            trsLastPencil.gameObject.SetActive(true);
            trsHead.localScale = unitHeadScale;
            
            AlignHeaderEditor();
            AlignHeaderEditor();
        }

        public virtual void InitData()
        {
            //Spawn last pencil
            InitDataEditor();

            for (var i = 0; i < spline.nodes.Count; i++)
            {
                NodeController node = new(spline.nodes[i], gameObject, speed, i);
                nodes.Add(node);
            }

            nodes[0].moveUpdateCallback = AlignHeaderEditor;
            trsLastPencil.gameObject.SetActive(false);
            objSpline.SetActive(true);
            //nodes[^1].moveDoneCallback = ActionMoveDoneCallBack;
        }

        #endregion

        #region Move
        /// <summary>
        /// Cố gắng di chuyển ra ngoài nếu không có va chạm
        /// </summary>
        [Button]
        public void TryMoveOut()
        {
            var hitTemp = CheckCanMove();
            if (hitTemp != null)  
            {
                MoveOutFail(hitTemp.Value);
            }
            else
            {
                MoveOut();
            }
        }

        /// <summary>
        /// Di chuyển đến điểm va chạm nếu có
        /// </summary>
        private void MoveOutFail(float3 hit)
        {
            var distanceToHit = Vector3.Distance(trsCheckPoint.position, hit);
            for (var i = 0; i < nodes.Count; i++)
            {
                var pathPoints = GetPathPointToHit(i, hit,true, distanceToHit);
                nodes[i].SetUpMoveOutFail(MoveBack, pathPoints, () => _ = ScaleHeadHit());
                nodes[i].MoveToNextPoint();
            }
        }
        
        private async UniTask ScaleHeadHit()
        {
            var duration = UnitGlobalConfig.Instance.unitScaleHitDuration;
            await LMotion.Create(trsHead.localScale, UnitGlobalConfig.Instance.vectorHeadScaleHit,
                    duration/2)
                .Bind(x => trsHead.localScale = x)
                .AddTo(this);
            await LMotion.Create(trsHead.localScale, UnitGlobalConfig.Instance.unitHeadScale,
                    duration/2)
                .Bind(x => trsHead.localScale = x)
                .AddTo(this);
        }
        /// <summary>
        /// Di chuyển ra ngoài theo đường dẫn đã được xác định
        /// </summary>
        [Button]
        private void MoveOut()
        {
            if (GameManager.Instance.currentLevelManager.TryResolveUnit(this))
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    var pathPoints = GetPathPoints(i);
                    nodes[i].SetUpMoveOut(ActionMoveDoneCallBack, pathPoints);
                    nodes[i].MoveToNextPoint();
                }
            }
        }

        private void ActionMoveDoneCallBack(int nodeIndex)
        {
            AlignHeaderEditor();
            if (nodeIndex == nodes.Count- 1)
            {
                AnimOnComplete();
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
        /// Lấy các điểm di chuyển đến điểm va chạm
        /// Nếu khoảng cách nhỏ hơn độ dài của spline thì cần phải xóa đi các điểm di chuyển
        /// </summary>
        private List<float3> GetPathPointToHit(int nodeIndex, float3 hit, bool getByHit = false, float distanceToHit = 0f)
        {
            var pathPoints = GetPathPointToOtherPoint(nodeIndex);
            pathPoints.Add(GetLastPointToHit(nodeIndex, hit));
            if (getByHit && distanceToHit < spline.nodes.Count/2)
            {
                var countRemaining = (int)distanceToHit > 1? (int)distanceToHit : 1;
                for (var i = pathPoints.Count - 1; i >= 0; i--)
                {
                    if (pathPoints.Count > countRemaining/*(nodeIndex == nodes.Count - 1?countRemaining+1:countRemaining)*/)
                        pathPoints.RemoveAt(i);
                    else
                        break;
                }

                if (countRemaining == 1 && distanceToHit < 1)
                {
                    var lastPoint = GetPathPointNotMove(nodeIndex);
                    pathPoints[^1] = lastPoint ?? pathPoints[^1];
                }
            }

            return pathPoints;
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

        /// <summary>
        /// Lấy điểm cuối cùng để dừng lại
        /// </summary>
        private float3 GetLastPointToHit(int nodeIndex, float3 hit)
        {
            Vector3 lastPoint = hit;
            var dir = (lastPoint - nodes[0].currentPosition).normalized;
            var distanceBtNode = UnitGlobalConfig.Instance.distanceBtNode;
            lastPoint -= dir * (distanceBtNode * nodeIndex) + dir * UnitGlobalConfig.Instance.sizeUnitHead - dir * (0.35f * (nodeIndex == 0 ? 0 : 1));
            

            return lastPoint;
        }

        public void SetPointGoal(PointGoal pointGoalPointGoal) => _pointGoal = pointGoalPointGoal;

        public void SetIDSharpener(int id) => sharpenerID = id;

        #endregion

        #region Align Head and Bottom

        [Button]
        private void AlignHeaderEditor()
        {
            var position = spline.nodes[0].Position;
            var dir = spline.nodes[0].Position - spline.nodes[1].Position;
            
            if (dir.magnitude > 0.0001f)
            {
                trsHead.LookAt(spline.transform.TransformPoint(spline.nodes[0].Position - dir));
                trsCheckPoint.LookAt(spline.transform.TransformPoint(spline.nodes[0].Position - dir));
            }
            trsHead.localPosition = position;
            trsCheckPoint.localPosition = position;
        }
        [Button]
        private void AlignPencil()
        {
            var positionPencil = splineOut.GetLastPointOut(transform);
            //positionPencil.y -= _unitConfig.size;

            trsLastPencil.transform.localPosition = positionPencil;
            trsLastPencil.eulerAngles = new float3(90, 0, 0);
        }

        #endregion

        #region Complete animation

        private MotionHandle _moveCompleteHandle;

        private void AnimOnComplete()
        {
            //Debug.Log("Animation complete for unit: " + gameObject.name);
            //LMotion.Create(transform.position, )
            objSpline.SetActive(false);
            trsLastPencil.gameObject.SetActive(true);
            
            lastPencil.AnimOnComplete(_pointGoal, sharpenerID);
        }

        public void AnimForUnitTemp(PointGoal pointGoal, int sharpenerId)
        {
            Debug.Log("anim for unit temp: " + unitId);
            lastPencil.AnimOnComplete(pointGoal, sharpenerId);
        }

        #endregion

        #region Save Data

        [Button]
        private void SaveData()
        {
            // Implement save logic here, e.g., saving unit state to a file or database
            Debug.Log($"Saving data for Unit ID: {unitId}, Length: {unitLength}, Color: {colorType}");
            var levelConfig = GameManager.Instance.currentLevelManager.currentLevelConfig;
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
                splineOut.splineOut.nodes
            );
        }

        #endregion

        #region Collider

        public bool IsHaveThatCollider(Collider colliderCheck) => myColliders.Contains(colliderCheck);

        #endregion

        [Button]
        private float3? CheckCanMove()
        {
            if (Physics.Linecast(trsCheckPoint.position, trsCheckPoint.position - trsCheckPoint.forward * distanceCheck, out var hit))
            {
                return transform.TransformPoint(hit.point);
            }
            return null;
        }

        public void ResetUnit()
        {
            trsLastPencil.gameObject.SetActive(false);
            for (var i = 0; i < nodes.Count; i++)
            {
                nodes[i].ClearPath();
            }
            objSpline.SetActive(true);
            gameObject.SetActive(false);
            lastPencil.transform.SetParent(lastPencilParents);
            AlignPencil();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(trsCheckPoint.position, trsCheckPoint.position - trsCheckPoint.forward * distanceCheck);
        }

        public bool CheckCanTouch()
        {
            return lastPencil.currentState == PencilState.Idle;
        }
    }
}

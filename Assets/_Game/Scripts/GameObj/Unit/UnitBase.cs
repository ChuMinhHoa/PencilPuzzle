using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.Scripts.GameManager;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using SplineMesh;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

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
        [FormerlySerializedAs("sharpener")] [FormerlySerializedAs("goalId")] public int sharpenerID;
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
        private Transform _trsGoal;

        [Title("Spline")] 
        [SerializeField] private Spline spline;
        [SerializeField] private SplineMeshTiling splineMeshTiling;
        public SplineOutController splineOut;
        public InitSpline initSpline;

        public List<NodeController> nodes = new();

        [Title("Animation")]
        public float speed = 2f;
        public float magnitude = 2f;

        public AnimationCurve curveComplete;
        
        [Title("Move out")]
        public float distanceCheck = 10f;
        public LastPencilController lastPencil;
        
        #region Init Data

        [Button]
        private void InitDataEditor()
        {
            lastPencil.InitData(colorType, unitLength);
            _unitPositionConfig = LevelManager.Instance.GetUnitPositionConfig(unitId);

            colorType = _unitPositionConfig.unitColor;
            unitLength = _unitPositionConfig.unitLength;

            _unitConfig = UnitGlobalConfig.Instance.GetUnitConfig(unitLength);

            initSpline.SetUpSpline(spline, _unitPositionConfig.pathMesh);
            var mat = UnitGlobalConfig.Instance.GetUnitMaterial(colorType);
            splineMeshTiling.material = mat;
            AlignPencil();
            nodes.Clear();
            var headMat = UnitGlobalConfig.Instance.GetTipMaterial(colorType);
            headMeshRenderer.material = headMat;
            trsLastPencil.gameObject.SetActive(true);
            //transform.position = _unitPositionConfig.position;
            //transform.eulerAngles = _unitPositionConfig.eulerAngles;
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

            nodes[0].moveUpdateCallback = AlignHeaderTransform;
            trsLastPencil.gameObject.SetActive(false);
            //nodes[^1].moveDoneCallback = ActionMoveDoneCallBack;
        }

        #endregion

        #region Move
        /// <summary>
        /// Cố gắng di chuyển ra ngoài nếu không có va chạm
        /// </summary>
        [Button]
        private void TryMOveOut()
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
            currentNodeBack = 0;
            var distanceToHit = Vector3.Distance(trsCheckPoint.position, hit);
            for (var i = 0; i < nodes.Count; i++)
            {
                var pathPoints = GetPathPointToHit(i, hit,true, distanceToHit);
                nodes[i].SetUpMoveOutFail(()=>_ = MoveBack(), pathPoints);
                nodes[i].MoveToNextPoint();
            }
        }
        /// <summary>
        /// Di chuyển ra ngoài theo đường dẫn đã được xác định
        /// </summary>
        [Button]
        private void MoveOut()
        {
            if (LevelManager.Instance.TryResolveUnit(this))
            {
                for (var i = 0; i < nodes.Count; i++)
                {
                    var pathPoints = GetPathPoints(i);
                    nodes[i].SetUpMoveOut(ActionMoveDoneCallBack, pathPoints);
                    nodes[i].MoveToNextPoint();
                }
            }
        }

        private void ActionMoveDoneCallBack()
        {
           // Debug.Log("all move done next step");
            AnimOnComplete();
        }

        private int currentNodeBack;
        /// <summary>
        /// Di chuyển ngược lại sau khi va chạm
        /// </summary>
        private async Task MoveBack()
        {
            currentNodeBack++;
            if(currentNodeBack == nodes.Count)
            {
                await UniTask.WaitForSeconds(UnitGlobalConfig.Instance.unitScaleHitDuration);
                for (var i = nodes.Count - 1; i >= 0; i--)
                {
                    nodes[i].SetUpMoveBack();
                    nodes[i].MoveToNextPoint();
                }
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
            if (getByHit && distanceToHit < spline.nodes.Count)
            {
                var countRemaining = (int)distanceToHit > 1? (int)distanceToHit : 1;
                //Debug.Log($"total remove{countRemaining}");
                for (var i = pathPoints.Count - 1; i >= 0; i--)
                {
                    if (pathPoints.Count > countRemaining)
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
        /// lấy điểm di chuển cuối nếu khoảng cách nhỏ hơn 1
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
            lastPoint -= dir * nodeIndex + dir * UnitGlobalConfig.Instance.sizeUnitHead - dir * 0.25f * (nodeIndex == 0 ? 0 : 1);
            return lastPoint;
        }

        public void SetPointGoal(Transform pointGoalPointGoal) => _trsGoal = pointGoalPointGoal;

        public void SetIDSharpener(int id) => sharpenerID = id;

        #endregion

        #region Align Head and Bottom

        [Button]
        private void AlignHeaderTransform(float3 position, float3 dir)
        {
            trsHead.localPosition = position;
            trsHead.LookAt(spline.transform.TransformPoint(dir));
            trsCheckPoint.localPosition = position;
            trsCheckPoint.LookAt(spline.transform.TransformPoint(dir));
        }
        
        [Button]
        private void AlignHeaderEditor()
        {
            var position = spline.nodes[0].Position;
            var dir = spline.nodes[0].Direction;
            trsHead.localPosition = position;
            trsHead.LookAt(spline.transform.TransformPoint(dir));
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

        #region Animation

        private MotionHandle _moveCompleteHandle;

        private void AnimOnComplete()
        {
            //Debug.Log("Animation complete for unit: " + gameObject.name);
            //LMotion.Create(transform.position, )

            objSpline.SetActive(false);
            trsLastPencil.gameObject.SetActive(true);
            if (_moveCompleteHandle.IsPlaying())
                _moveCompleteHandle.TryCancel();
            var progress = 0f;
            var duration = curveComplete.keys[^1].time;
            _moveCompleteHandle = LMotion.Create(trsLastPencil.position, _trsGoal.position, duration)
                .WithOnComplete(() =>
                {
                    trsLastPencil.SetParent(_trsGoal);
                    LevelManager.Instance.SharpenerEndAnimAndCheck(sharpenerID);
                })
                .Bind(x =>
                {
                    progress = Mathf.Clamp01(progress / duration);
                    float3 position = x;
                    position.y += curveComplete.Evaluate(progress) * magnitude;

                    trsLastPencil.position = position;

                    progress += Time.deltaTime;
                })
                .AddTo(this);
            var currentEuler = trsLastPencil.eulerAngles.x;
            LMotion.Create(currentEuler, 270f, 0.25f)
                .Bind(x =>
                {
                    Debug.Log(x);
                    trsLastPencil.eulerAngles = new float3(x, 0f, 0f);
                })
                .AddTo(this);

            LMotion.Create(trsLastPencil.localScale, Vector3.one*0.7f, 0.25f)
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
                spline.nodes
            );
        }

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
    }
}

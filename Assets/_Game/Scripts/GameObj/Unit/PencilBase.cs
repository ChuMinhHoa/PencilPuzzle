using System.Collections.Generic;
using _Game.Scripts.GameObj.Interface;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager;
using _Game.Scripts.ScriptAbleObject;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using TW.Utility.CustomType;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class PencilBase : BotBase<UnitPositionConfig>
    {
        [Title("Unit define")] 
        public int unitId;
        public int sharpenerID;
        public SharpenerColorType colorType;
        
        [Title("Transform")] 
        [SerializeField] private Transform trsHead;
        [SerializeField] private Transform trsLastPencil;
        [SerializeField] private Transform trsCheckPoint;
        [SerializeField] private Transform lastPencilParents;
        [SerializeField] public Transform trsWayOut;
        private PointGoal pointGoal;
        
        [Title("GameObject")] 
        [SerializeField] private MeshRenderer headMeshRenderer;
        
        [Title("Move out")]
        public float distanceCheck = 10f;
        public LastPencilController lastPencil;

        [Title("Spline")] 
        public PencilSplineController splineController;
        
        [Title("Collider")]
        public List<Collider> myColliders = new();

        [Range(5, 50)]
        public float speed = 10f;
        
        
        public void SetPointGoal(PointGoal pointGoalChange) => pointGoal = pointGoalChange;
        public void SetIDSharpener(int id) => sharpenerID = id;
        
        [Button]
        public void InitData()
        {
            var dataLoad = GameManager.Instance.currentLevelManager.GetUnitPositionConfig(unitId);
            LoadData(dataLoad);
            splineController.InitPathPointController(AlignHeader, AnimOnComplete, speed);
            trsLastPencil.gameObject.SetActive(false);
            splineController.SetObjSpline(true); 
        }

        #region Obj Data

        public override void LoadData(UnitPositionConfig dataLoad)
        {
            lastPencil = PoolingObject.Instance.SpawnLastPencilController(lastPencilParents);
            
            trsLastPencil = lastPencil.transform;
            trsLastPencil.SetParent(lastPencilParents);
            
            lastPencil.InitData(colorType);
            data = dataLoad;

            colorType = data.unitColor;

            var unitHeadScale = UnitGlobalConfig.Instance.unitHeadScale;

            splineController.SetUpSpline(data.pathMesh);
            splineController.SetUpSplineOut(data.wayOut);
            var mat = UnitGlobalConfig.Instance.GetUnitMaterial(colorType);
            splineController.SetUpSplineMat(mat);
            splineController.ClearNodes();
            
            AlignPencil();
            var headMat = UnitGlobalConfig.Instance.GetTipMaterial(colorType);
            headMeshRenderer.material = headMat;
            trsLastPencil.gameObject.SetActive(true);
            trsHead.localScale = unitHeadScale;
            
            AlignHeader();
            AlignHeader();
        }

        public override void ReloadData()
        {
        }

        public override void ResetData()
        {
        }

        public override void Despawn()
        {
        }

        public override void SaveConfig()
        {
        }

        public override void Reset()
        {
            trsLastPencil.gameObject.SetActive(false);
            splineController.Reset();
            trsLastPencil.parent = null;
            PoolingObject.Instance.DeSpawnLastPencilController(lastPencil);
            gameObject.SetActive(false);
        }

        #endregion

        #region AlignPencil
        [Button]
        private void AlignPencil()
        {
            var positionPencil = splineController.GetLastPointOut();
            trsLastPencil.transform.localPosition = positionPencil;
            trsLastPencil.transform.parent = null;
            trsLastPencil.eulerAngles = new float3(90, 0, 0);
        }
        [Button]
        private void AlignHeader()
        {
            var position = splineController.GetSplineNodePosition(0); 
            var dir = position - splineController.GetSplineNodePosition(1);
            
            if (dir.magnitude > 0.0001f)
            {
                var lookAtPoint = splineController.GetSplineTransformPoint(position - dir);
                trsHead.LookAt(lookAtPoint);
                trsCheckPoint.LookAt(lookAtPoint);
            }
            trsHead.localPosition = position;
            trsCheckPoint.localPosition = position;
        }
        #endregion
        
        #region Complete animation

        private MotionHandle moveCompleteHandle;

        private void AnimOnComplete()
        {
            splineController.SetObjSpline(false);
            trsLastPencil.gameObject.SetActive(true);
            lastPencil.AnimOnComplete(pointGoal, sharpenerID);
        }

        public void AnimForUnitTemp(PointGoal pointGoalTemp, int sharpenerId)
        {
            Debug.Log("anim for unit temp: " + unitId);
            lastPencil.AnimOnComplete(pointGoalTemp, sharpenerId);
        }

        #endregion
        
        #region Collider

        public bool IsHaveThatCollider(Collider colliderCheck) => myColliders.Contains(colliderCheck);

        #endregion

        #region Move out

        /// <summary>
        /// Cố gắng di chuyển ra ngoài nếu không có va chạm
        /// </summary>
        [Button]
        public virtual void TryMoveOut()
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
            splineController.MoveOutFail(hit, () => _ = ScaleHeadHit());
        }
        /// <summary>
        /// Di chuyển ra ngoài theo đường dẫn đã được xác định
        /// </summary>
        [Button]
        private void MoveOut()
        {
            if (GameManager.Instance.currentLevelManager.TryResolveUnit(this))
                splineController.MoveOut();
        }

        #endregion
        
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
       
        
        private float3? CheckCanMove()
        {
            if (Physics.Linecast(trsCheckPoint.position, trsCheckPoint.position - trsCheckPoint.forward * distanceCheck, out var hit))
            {
                return transform.TransformPoint(hit.point);
            }
            return null;
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

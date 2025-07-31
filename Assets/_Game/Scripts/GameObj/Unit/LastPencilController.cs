using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager;
using Cysharp.Threading.Tasks;
using LitMotion;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public enum PencilState
    {
        Idle,
        Moving
    }

    public class LastPencilController : MonoBehaviour
    {
        public SharpenerColorType colorType;
        public MeshRenderer pencilRenderer;
        public MeshRenderer tipRenderer;
        public Transform pencilBody;
        public float tipLength = 1f;
        public PencilState currentState;
        [Title("Animation")]
        public float speed = 2f;
        public float magnitude = 2f;

        public AnimationCurve curveComplete;
        [Button]
        public void InitData(SharpenerColorType color)
        {
            colorType = color;
            var colorMat = UnitGlobalConfig.Instance.GetUnitMaterial(color);
            var colorTipMat = UnitGlobalConfig.Instance.GetTipMaterial(color);
            if (colorMat != null && colorTipMat != null)
            {
                pencilRenderer.material = colorMat;
                tipRenderer.material = colorTipMat;
            }
            else
            {
                Debug.LogWarning($"Material for color {color} not found.");
            }

            //SetLength(lengthType);
        }

        public void SetLength(float lengthType)
        {
            var localScale = pencilBody.localScale;
            var length = (lengthType - 0.5f) / 1.5f;
            pencilBody.localScale = new Vector3(localScale.x, localScale.y, length);
        }

        [BoxGroup("Anim hit")]
        public Vector3 vectorScaleHit;

        private MotionHandle _moveCompleteHandle;
        private MotionHandle scaleBodyHandle;

        [BoxGroup("Anim hit")]
        [Button]
        public async UniTask AnimHit()
        {
            await LMotion.Create(pencilBody.localScale, vectorScaleHit, 0.15f)
                .Bind(x=>pencilBody.localScale = x).AddTo(this);
            await LMotion.Create(vectorScaleHit, Vector3.one, 0.15f)
                .Bind(x=>pencilBody.localScale = x).AddTo(this);
        }

        public void AnimOnComplete(PointGoal pointGoal, int sharpenerID)
        {
            if (_moveCompleteHandle.IsPlaying())
                _moveCompleteHandle.TryCancel();
            var progress = 0f;
            var duration = curveComplete.keys[^1].time;
            transform.SetParent(pointGoal.trsPoint);
            currentState = PencilState.Moving;
            _moveCompleteHandle = LMotion.Create(transform.localPosition, Vector3.zero, duration)
                .WithOnComplete(() =>
                {
                    transform.SetParent(pointGoal.trsPoint);
                    _ = AnimHit();
                    _ = pointGoal.OnHit();
                    currentState = PencilState.Idle;
                    GameManager.Instance.currentLevelManager.SharpenerEndAnimAndCheck(sharpenerID);
                    GameManager.Instance.CheckCanTouch();
                })
                .Bind(x =>
                {
                    progress = Mathf.Clamp(progress, 0f, duration);
                    var position = x;
                    position.y += curveComplete.Evaluate(progress) * magnitude;

                    transform.localPosition = position;

                    progress += Time.deltaTime;
                })
                .AddTo(this);
            if (scaleBodyHandle.IsPlaying())
                scaleBodyHandle.TryCancel();
            var currentScale = pencilBody.localScale;
            scaleBodyHandle = LMotion.Create(currentScale, Vector3.one, 0.15f)
                .Bind(x => pencilBody.localScale = x)
                .AddTo(this);
            
            var currentEuler = transform.eulerAngles.x;
            LMotion.Create(currentEuler, 270f, 0.25f)
                .Bind(x =>
                {
                    transform.eulerAngles = new float3(x, 0f, 0f);
                })
                .AddTo(this);

            LMotion.Create(transform.localScale, Vector3.one*0.7f, 0.25f)
                .Bind(x => transform.localScale = x)
                .AddTo(this);
        }
    }
}

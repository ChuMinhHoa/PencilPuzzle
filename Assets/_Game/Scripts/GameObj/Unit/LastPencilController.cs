using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class LastPencilController : MonoBehaviour
    {
        public SharpenerColorType colorType;
        public MeshRenderer pencilRenderer;
        public MeshRenderer tipRenderer;
        public Transform pencilBody;
        public float tipLength = 1f;
        [Button]
        public void InitData(SharpenerColorType color, UnitLengthType lengthType)
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

            SetLength(lengthType);
        }

        private void SetLength(UnitLengthType lengthType)
        {
            var length = ((float)lengthType-0.5f) / 1.5f;
            pencilBody.localScale = new Vector3(1, 1, length);
        }
    }
}

using _Game.Scripts.GlobalConfig;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class SharpenerAds : SharpenerTemp
    {
        public bool isUnlock;
        public GameObject objCanvas;
        
        public override PointGoal TryGetPointGoal()
        {
            if (!isUnlock) return null;
            return base.TryGetPointGoal();
        }

        public void OnMouseDown()
        {
            isUnlock = true;
            objCanvas.SetActive(false);
            sharpenerMesh.material = UnitGlobalConfig.Instance.defaultMaterial;
        }
    }
}

using _Game.Scripts.GameObj.Sharpener;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.GameManager
{
    public class PoolingObject : Singleton<PoolingObject>
    {
        public TPool<Sharpener> sharpenerPool;
        
        public TPool<Transform> hitEffectPool;
        
        public Transform GetHitEffect(Transform pointHit)
        {
            var hitEffect = hitEffectPool.Spawn();
            hitEffect.transform.position = pointHit.position;
            return hitEffect;
        }
    }
}

using _Game.Scripts.GameObj.Sharpener;
using TW.Utility.DesignPattern;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class PoolingObject : Singleton<PoolingObject>
    {
        #region HitEffect

        public TPool<Transform> hitEffectPool;
        
        public Transform GetHitEffect(Transform pointHit)
        {
            var hitEffect = hitEffectPool.Spawn();
            hitEffect.transform.position = pointHit.position;
            return hitEffect;
        }

        public void DeSpawnHitEffect(Transform effect)
        {
            hitEffectPool.Despawn(effect);
        }

        #endregion

        #region Sharpener
        public TPool<Sharpener> sharpenerPool;
        public Sharpener SpawnSharpener(Transform pointSpawn)
        {
            var sharpenerTemp = sharpenerPool.Spawn();
            sharpenerTemp.transform.position = pointSpawn.position;
            return sharpenerTemp;
        }

        public void DeSpawnSharpener(Sharpener sharpener)
        {
            sharpenerPool.Despawn(sharpener);
        }

        #endregion
    }
}

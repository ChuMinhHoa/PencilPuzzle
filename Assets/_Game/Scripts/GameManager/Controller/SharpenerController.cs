using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.ScriptAbleObject;
using UnityEngine;

namespace _Game.Scripts.GameManager.Controller
{
    public class SharpenerController : MonoBehaviour
    {
        public List<Sharpener> currentSharpeners;
        
        public List<Transform> trsMoveTo;
        
        public TPool<Sharpener> sharpenerPool;

        public Transform trsOut;
        
        public Sharpener TryGetSharpener(SharpenerColorType colorType)
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                if (currentSharpeners[i].IsSameColor(colorType))
                    return currentSharpeners[i];
            }

            return null;
        }
        
        public Sharpener TryGetTempSharpener()
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                if (currentSharpeners[i].IsSameColor(SharpenerColorType.ColorTemp))
                    return currentSharpeners[i];
            }
            return null;
        }

        public void AddSharpener()
        {
            var sharpener = sharpenerPool.Spawn();
            currentSharpeners.Add(sharpener);
        }

        public void RemoveSharpener(Sharpener sharpener)
        {
            currentSharpeners.Remove(sharpener);
            
            sharpener.AnimMove(trsOut, () =>
            {
                sharpenerPool.Despawn(sharpener);
            });
         
        }

        public void SpawnSharpener(WaveConfig waveConfig)
        {
            for (var i = 0; i < waveConfig.sharpenerColors.Count; i++)
            {
                var sharpenerTemp = sharpenerPool.Spawn();
                sharpenerTemp.InitData(waveConfig.sharpenerColors[i]);
                sharpenerTemp.AnimMove(trsMoveTo[i]);
                currentSharpeners.Add(sharpenerTemp);
            }
        }

        public void SharpenerEndAnimAndCheck(int sharpenerID)
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                if (currentSharpeners[i].id == sharpenerID)
                {
                    currentSharpeners[i].AnimDone();
                }
            }
        }
    }
}

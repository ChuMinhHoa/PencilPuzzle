using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.ScriptAbleObject;
using UnityEngine;

namespace _Game.Scripts.Manager.Controller
{
    public class SharpenerController : MonoBehaviour
    {
        public List<Sharpener> currentSharpeners;
        public List<Sharpener> currentSharpenersTemp;
        
        public List<Transform> trsMoveTo;
        public Transform sharpenersParent;
        //public TPool<Sharpener> sharpenerPool;

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
            for (var i = 0; i < currentSharpenersTemp.Count; i++)
            {
                if (currentSharpenersTemp[i].IsSameColor(SharpenerColorType.ColorTemp))
                    return currentSharpenersTemp[i];
            }
            return null;
        }

        public void RemoveSharpener(Sharpener sharpener)
        {
            currentSharpeners.Remove(sharpener);
            
            sharpener.AnimMove(trsOut, () =>
            {
                sharpener.ClearLastPencilController();
                sharpener.ResetSharpener();
                PoolingObject.Instance.DeSpawnSharpener(sharpener);
            });
         
        }

        public void SpawnSharpener(WaveConfig waveConfig, int currentWaveIndex)
        {
            for (var i = 0; i < waveConfig.sharpenerColors.Count; i++)
            {
                Debug.Log("Spawn Sharpener: " + waveConfig.sharpenerColors[i]);
                var sharpenerTemp =  PoolingObject.Instance.SpawnSharpener(sharpenersParent);
                sharpenerTemp.InitData(waveConfig.sharpenerColors[i], currentWaveIndex);
                sharpenerTemp.AnimMove(trsMoveTo[i],
                    () => GameManager.Instance.currentLevelManager.unitController.CheckUnitTemps(currentSharpeners));
                currentSharpeners.Add(sharpenerTemp);
            }
        }

        public void SharpenerEndAnimAndCheck(int sharpenerID)
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                if (currentSharpeners[i].id == sharpenerID)
                {
                    _ = currentSharpeners[i].AnimDone();
                    return;
                }
            }

            for (var i = 0; i < currentSharpenersTemp.Count; i++)
            {
                if (currentSharpenersTemp[i].id == sharpenerID)
                {
                    _ = currentSharpenersTemp[i].AnimDone();
                    return;
                }
            }
        }

        public bool IsDoneThatWave()
        {
            return currentSharpeners.Count == 0;
        }

        public void ResetAllSharpeners()
        {
            for (var i = 0; i < currentSharpenersTemp.Count; i++)
            {
                currentSharpenersTemp[i].ResetSharpener();
            }
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                currentSharpeners[i].ResetSharpener();
                PoolingObject.Instance.DeSpawnSharpener(currentSharpeners[i]);
            }
            currentSharpeners.Clear();
        }
    }
}

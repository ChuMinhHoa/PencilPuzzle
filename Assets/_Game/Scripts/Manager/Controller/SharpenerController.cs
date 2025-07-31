using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.ScriptAbleObject;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.Manager.Controller
{
    public class SharpenerController : MonoBehaviour
    {
        public List<Sharpener> currentSharpeners;
        public List<Sharpener> currentSharpenersTemp;
        
        //public List<Transform> trsMoveTo;
        public List<PointHaveObj<Sharpener>> pointHaveSharpeners;
        public Transform sharpenersParent;
        //public int currentPointSharpenerIndex = 0;
        //public TPool<Sharpener> sharpenerPool;

        public Transform trsOut;
        
        public Sharpener TryGetSharpener(SharpenerColorType colorType)
        {
            for (var i = 0; i < currentSharpeners.Count; i++)
            {
                if (currentSharpeners[i].CheckSharpenerCanMoveTo(colorType))
                    return currentSharpeners[i];
            }

            return null;
        }
        
        public Sharpener TryGetTempSharpener()
        {
            for (var i = 0; i < currentSharpenersTemp.Count; i++)
            {
                if (currentSharpenersTemp[i].CheckSharpenerCanMoveTo(SharpenerColorType.ColorTemp))
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

        public void SpawnSharpener(SharpenerColorType colorType, int currentWaveIndex)
        {
            var pointMoveTo = GetPointMoveTo();
            if (pointMoveTo == null)
            {
                Debug.Log("No point to move sharpener to!");
                return;
            }
            Debug.Log("Spawn Sharpener: " + colorType);
            var sharpenerTemp =  PoolingObject.Instance.SpawnSharpener(sharpenersParent);
            sharpenerTemp.InitData(colorType);
            sharpenerTemp.AnimMove(pointMoveTo.trsPoint,
                () => GameManager.Instance.currentLevelManager.pencilController.CheckUnitTemps(currentSharpeners));
            pointMoveTo.SetObjOnPoint(sharpenerTemp);
            currentSharpeners.Add(sharpenerTemp);
        }
        
        public void ClearSharpenerPoint(Sharpener sharpener)
        {
            Debug.Log(sharpener.id +" "+ sharpener.sharpenerColorType);
            for (var i = 0; i < pointHaveSharpeners.Count; i++)
            {
                if (pointHaveSharpeners[i].objOnPoint == sharpener)
                {
                    pointHaveSharpeners[i].objOnPoint = null;
                    return;
                }
            }
        }

        private PointHaveObj<Sharpener> GetPointMoveTo()
        {
            for (var i = 0; i < pointHaveSharpeners.Count; i++)
            {
                if(pointHaveSharpeners[i].isFree) return pointHaveSharpeners[i];
            }

            return null;
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

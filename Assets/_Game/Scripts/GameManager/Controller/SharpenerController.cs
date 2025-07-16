using System.Collections.Generic;
using _Game.Scripts.GameObj.Sharpener;
using UnityEngine;

namespace _Game.Scripts.GameManager.Controller
{
    public class SharpenerController : MonoBehaviour
    {
        public List<Sharpener> sharpeners;
        
        public List<Sharpener> currentSharpeners;
        
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
    }
}

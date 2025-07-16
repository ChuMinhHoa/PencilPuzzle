using System.Collections.Generic;
using SplineMesh;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public enum SharpenerColorType
    {
        None = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        Yellow = 4,
        Purple = 5,
        Orange = 6,
        Pink = 7,
        White = 8,
        Black = 9,
        ColorTemp = 10, 
    }

    public class Sharpener : MonoBehaviour
    {
        public SharpenerColorType sharpenerColorType;

        //public ExampleContortAlong contortAlong;
        
        public List<PointGoal> pointGoals = new();

        public PointGoal TryGetPointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                if (pointGoals[i].IsFree())
                {
                    return pointGoals[i];
                }
            }

            return null;
        }
        
        public void ClearSharpener()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ClearPointGoal();
            }
            //contortAlong.gameObject.SetActive(true);
            //contortAlong.Play();
        }

        public bool IsSameColor(SharpenerColorType colorType)
        {
            return sharpenerColorType == colorType;
        }
    }
}

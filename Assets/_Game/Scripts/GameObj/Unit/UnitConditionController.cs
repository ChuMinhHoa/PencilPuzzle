using System.Collections.Generic;
using _Game.Scripts.GameObj.Interface;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Unit
{
    public enum ConditionType
    {
        None = 0,
        UnderHidden = 1,
        Obstacle = 2,
    }
    
    public class UnitConditionController : MonoBehaviour
    {
        public List<ConditionFace> conditionFaces = new();

        public void InitConditions()
        {
            for (var i = 0; i < conditionFaces.Count; i++)
            {
                conditionFaces[i].InitCondition();
            }
        }

        public void ResetCondition()
        {
            for (var i = 0; i < conditionFaces.Count; i++)
            {
                conditionFaces[i].ResetCondition();
            }
        }
        
        [Button]
        public bool IsConditionSatisfied()
        {
            for (var i = 0; i < conditionFaces.Count; i++)
            {
                if (!conditionFaces[i].IsConditionSatisfied())
                    return false;
            }

            return true;
        }
    }
}

using System.Collections.Generic;
using _Game.Scripts.GameObj.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Unit
{
    public class UnitConditionController : MonoBehaviour
    {
        public List<ConditionFace> ConditionFaces = new();

        [Button]
        public bool IsConditionSatisfied()
        {
            for (var i = 0; i < ConditionFaces.Count; i++)
            {
                if (!ConditionFaces[i].IsConditionSatisfied())
                    return false;
            }

            return true;
        }

        [Button]
        private void AddLock()
        {
            for (int i = 0; i < ConditionFaces.Count; i++)
            {
                  ConditionFaces[i].AddPropertyCondition(new ConditionFaceArg{targetId = 1, data = new object[]{0,1,1,1}});
            }
        }
    }
}

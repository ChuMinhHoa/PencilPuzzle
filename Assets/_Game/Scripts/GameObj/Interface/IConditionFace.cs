using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public interface IConditionFace
    {
        public bool IsConditionSatisfied();
    }

    public class ConditionFace : MonoBehaviour, IConditionFace
    {
        public virtual bool IsConditionSatisfied()
        {
            return true;
        }

        public virtual void AddPropertyCondition(ConditionFaceArg args)
        {
        }
    }
    
    public class ConditionFaceArg : EventArgs
    {
        public int targetId;
        public object[] data;
    }

}

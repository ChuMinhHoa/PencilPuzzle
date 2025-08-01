using System;
using System.Collections.Generic;
using _Game.Scripts.GameObj.Unit;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public abstract class ConditionFace : MonoBehaviour
    {
        public ConditionType conditionType;

        public abstract bool IsConditionSatisfied();
        public abstract void ResetCondition();
        
        public void AddPropertyCondition(params object[] args)
        {
            AddProperty(args);
        }

        protected abstract void AddProperty(Memory<object> args);


    }

}

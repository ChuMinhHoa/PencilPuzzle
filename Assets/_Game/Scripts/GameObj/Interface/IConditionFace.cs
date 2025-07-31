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

    public abstract class ConditionFace : MonoBehaviour, IConditionFace
    {
        public ConditionType conditionType;

        public abstract bool IsConditionSatisfied();

        public void AddPropertyCondition(params object[] args)
        {
            AddProperty(args);
        }

        protected abstract void AddProperty(Memory<object> args);


    }

}

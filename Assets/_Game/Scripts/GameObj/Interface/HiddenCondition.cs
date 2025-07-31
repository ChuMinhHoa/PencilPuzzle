using System;
using TW.UGUI.Core.Modals;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Interface
{
    public class HiddenMatCondition : ConditionFace
    {
        public int unitUnlockHidden = -1;
        public bool isResolved;
        
        public override bool IsConditionSatisfied()
        {
            return isResolved;
        }

        protected override void AddProperty(Memory<object> args)
        {
            if (args.Span[0] is int value && value == unitUnlockHidden)
            {
                isResolved = true;
            }
        }

    }
}



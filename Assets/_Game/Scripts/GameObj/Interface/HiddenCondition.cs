using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Interface
{
    public class HiddenMatCondition : ConditionFace
    {
        public int unitUnlockHidden;
        public override bool IsConditionSatisfied()
        {
            return true;
        }

        public override void AddPropertyCondition(ConditionFaceArg args)
        {
            Debug.Log(args.targetId);
            Debug.Log(args.data[0]);
            base.AddPropertyCondition(args);
        }
    }
}



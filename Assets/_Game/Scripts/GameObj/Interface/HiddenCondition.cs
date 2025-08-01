using System;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GlobalConfig;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Modals;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Interface
{
    public class HiddenMatCondition : ConditionFace
    {
        public MeshRenderer tipMesh;
        public MeshRenderer bodyMesh;
        public int unitUnlockHidden = -1;
        public bool isResolved;
        
        public Material tipMatDefault;
        public Material tipMatChange;
        
        public Material bodyMatDefault;
        public Material bodyMatChange;

        private void OnEnable()
        {
            ResetCondition();
        }

        public override bool IsConditionSatisfied()
        {
            return isResolved;
        }

        public override void ResetCondition()
        {
            isResolved = false;
            bodyMesh.material = tipMatDefault;
            tipMesh.material = bodyMatDefault;
        }

        protected override void AddProperty(Memory<object> args)
        {
            if (args.Span[0] is int value && value == unitUnlockHidden)
            {
                isResolved = true;
                bodyMesh.material = tipMatChange;
                tipMesh.material = bodyMatChange;
            }
        }

    }
}



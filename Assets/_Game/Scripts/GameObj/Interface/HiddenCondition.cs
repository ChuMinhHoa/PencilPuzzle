using System;
using _Game.Scripts.GameObj.Sharpener;
using _Game.Scripts.GameObj.Unit;
using _Game.Scripts.GlobalConfig;
using _Game.Scripts.Manager.Etc;
using Sirenix.OdinInspector;
using SplineMesh;
using TW.UGUI.Core.Modals;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Game.Scripts.GameObj.Interface
{
    public class HiddenMatCondition : ConditionFace
    {
        public MeshRenderer tipMesh;
        public MeshRenderer bodyMesh;
        public SplineMeshTiling bodySplineMesh;
        public int unitUnlockHidden = -1;
        public bool isResolved;
        
        public Material tipMatDefault;
        public Material tipMatChange;
        
        public Material bodyMatDefault;
        public Material bodyMatChange;

        public override bool IsConditionSatisfied()
        {
            return isResolved;
        }

        [Button]
        public override void ResetCondition()
        {
            isResolved = false;
            bodyMesh.material = bodyMatDefault;
            bodySplineMesh.material = bodyMatDefault;
            tipMesh.material = tipMatDefault;
        }

        public override void InitCondition()
        {
            ResetCondition();
            GameGlobalEvent.OnPencilResolve += ResolvePencil;
        }

        private void ResolvePencil(PencilBase pencilBase)
        {
            if (pencilBase.unitId == unitUnlockHidden)
            {
                isResolved = true;
                bodyMesh.material = bodyMatChange;
                bodySplineMesh.material = bodyMatChange;
                tipMesh.material = tipMatChange;
            }
        }

        private void OnDisable()
        {
            Debug.Log("On Disable");
            GameGlobalEvent.OnPencilResolve -= ResolvePencil;
        }

    }
}



using _Game.Scripts.Manager;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class SharpenerTemp : Sharpener
    {
        public override UniTask AnimDone()
        {
            PlayAnimGoal();
            return UniTask.CompletedTask;
        }

        public override void ClearSharpener()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ClearPointGoal();
            }
        }
    }
}

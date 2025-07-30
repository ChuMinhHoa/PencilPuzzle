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

        public override void ResetPointGoal()
        {
            for (var i = 0; i < pointGoals.Count; i++)
            {
                pointGoals[i].ResetPointGoal();
            }
        }
    }
}

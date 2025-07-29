using Cysharp.Threading.Tasks;

namespace _Game.Scripts.GameObj.Sharpener
{
    public class SharpenerAds : SharpenerTemp
    {
        public bool isUnlock;
        public override PointGoal TryGetPointGoal()
        {
            if (!isUnlock) return null;
            return base.TryGetPointGoal();
        }
    }
}

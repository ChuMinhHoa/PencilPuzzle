using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public interface IBotFace<TData>
    {
        public void LoadData(TData data);
        public void ReloadData();
        public void ResetData();
        public void Despawn();
        public void SaveConfig();
        public void Reset();
    }
}

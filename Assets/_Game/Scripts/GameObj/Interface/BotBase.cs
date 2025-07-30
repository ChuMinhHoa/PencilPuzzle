using _Game.Scripts.GlobalConfig;
using UnityEngine;

namespace _Game.Scripts.GameObj.Interface
{
    public abstract class BotBase<TData>: MonoBehaviour, IBotFace<TData>
    {
        public TData data;

        public abstract void LoadData(TData dataLoad);

        public abstract void ReloadData();

        public abstract void ResetData();

        public abstract void Despawn();

        public abstract void SaveConfig();
        public abstract void Reset();
    }
}

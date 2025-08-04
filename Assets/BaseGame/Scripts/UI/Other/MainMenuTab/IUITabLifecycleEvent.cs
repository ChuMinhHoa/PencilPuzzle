using System;
using Cysharp.Threading.Tasks;

namespace BaseGame.Scripts.UI.Other.MainMenuTab
{
    public interface IUITabLifecycleEvent
    {
        UniTask Initialize(Memory<object> args);
        void OnTabEnter(Memory<object> args);
        void OnTabExit(Memory<object> args);
    }
}
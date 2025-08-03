using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;

namespace _Game.Scripts.Manager
{
    public class LoadSceneManager : Singleton<LoadSceneManager>
    {
        public async void LoadSDKScene()
        {
            await UniTask.Delay(1000);
            InGameController.OnAddEvent?.Invoke();
        }
    }
}

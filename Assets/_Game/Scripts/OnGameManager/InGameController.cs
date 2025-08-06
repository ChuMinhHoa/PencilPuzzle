using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.Manager
{
    public class InGameController : MonoBehaviour
    {
        public static Action OnAddEvent { get; set; }
        public static Action OnRemoveEvent { get; set; }
        
        private void Start()
        {
            transform.SetParent(null);
            DontDestroyOnLoad(this);
            OnAddEvent += AddEvent;
            OnRemoveEvent += RemoveEvent;
        }

        async UniTask LoadEvent()
        {
            await UniTask.Delay(1000);
            // InGameAnalyticController.OnAddEvent?.Invoke();
            // InGameIAPController.OnAddEvent?.Invoke();
            // InGameAdsController.OnAddEvent?.Invoke();
            Debug.Log("InGameController LoadEvent");
        }
    
        public void AddEvent()
        {
            LoadEvent().Forget();
        }
        
        public void RemoveEvent()
        {
            // InGameAnalyticController.OnRemoveEvent?.Invoke();
            // InGameIAPController.OnRemoveEvent?.Invoke();
            // InGameAdsController.OnRemoveEvent?.Invoke();
        }
    
        private void OnDestroy()
        {
            RemoveEvent();
            OnAddEvent -= AddEvent;
            OnRemoveEvent -= RemoveEvent;
        }
    }
}

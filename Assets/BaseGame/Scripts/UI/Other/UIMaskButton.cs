using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Other
{
    public class UIMaskButton : MonoBehaviour
    {
        [field: SerializeField] private Button MainButton {get; set;}

        public void SetMainButtonParent(Transform parent)
        {
            MainButton.transform.SetParent(parent);
        }
        public void SetOnClickDestination(Func<UniTask> task)
        {
            MainButton.SetOnClickDestination(task);
        }
        public void SetOnClickDestination(Action action)
        {
            MainButton.SetOnClickDestination(action);
        }
    }
}
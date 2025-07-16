using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Game.Scripts.GameObj.Unit;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Game.Scripts.GameManager.Controller
{
    public class UnitController : MonoBehaviour
    {
        public List<UnitBase> units = new();

        private async void Awake()
        {
            for (var i = 0; i < units.Count; i++)
            {
                units[i].gameObject.SetActive(true);
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}

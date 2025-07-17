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
        public async UniTask InitData()
        {
            for (var i = 0; i < units.Count; i++)
            {
                units[i].gameObject.SetActive(true);
                units[i].InitData();
                await UniTask.WaitForEndOfFrame();
            }
        }
    }
}

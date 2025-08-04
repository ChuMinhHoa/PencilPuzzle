using System;
using BaseGame.Scripts.Data;
using UnityEngine;

namespace CoreData
{
    [Serializable]
    public class InGameData
    {
        [field: SerializeField] public UserData UserData { get; set; }= new UserData();
        [field: SerializeField] public ResourceData ResourceData { get; set; } = new ResourceData();
        [field: SerializeField] public SettingData SettingData { get; set; } = new SettingData();
    }
}
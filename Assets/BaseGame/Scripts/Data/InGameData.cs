using System;
using CoreData;
using UnityEngine;

namespace BaseGame.Scripts.Data
{
    [Serializable]
    public class InGameData
    {
        [field: SerializeField] public UserData UserData { get; set; }= new UserData();
        [field: SerializeField] public ResourceData ResourceData { get; set; } = new ResourceData();
        [field: SerializeField] public SettingData SettingData { get; set; } = new SettingData();
    }
}
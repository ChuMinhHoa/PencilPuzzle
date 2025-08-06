using System;
using _Game.Scripts.OnGameManager;
using UnityEngine;

namespace _Game.Scripts.ScriptAbleObject
{
    [Serializable]

    public class AudioConfig
    {
        [field: SerializeField] public AudioKey AudioKey { get; private set; }
        [field: SerializeField] public AudioClip[] AudioClip { get; private set; }
    }
}
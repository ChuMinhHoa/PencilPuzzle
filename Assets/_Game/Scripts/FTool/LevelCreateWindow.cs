using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.ScriptAbleObject;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.FTool
{
    public class LevelCreateWindow : EditorWindow
    {
        public static LevelManager levelManager { get; set; }
        public static LevelConfig levelConfig { get; set; }
        private Vector2 scrollPosition;
        [MenuItem("Tools/Level Create")]
        public static void ShowWindow()
        {
            GetWindow<LevelCreateWindow>("Level Create");
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Create Tab", EditorStyles.boldLabel);

            levelManager = (LevelManager)EditorGUILayout.ObjectField(
                "Level Manager",
                levelManager,
                typeof(LevelManager),
                true
            );
            
            levelConfig = (LevelConfig)EditorGUILayout.ObjectField(
                "Level Config",
                levelConfig,
                typeof(LevelConfig),
                true
            );
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            levelConfig = levelManager.currentLevelConfig;
            
            EditorGUILayout.EndScrollView();
        }
    }
}

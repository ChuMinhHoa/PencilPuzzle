using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace _Game.Scripts.FTool
{
    public class FTool : MonoBehaviour
    {
        [BoxGroup("Create Material")]
        public List<Texture> spritesMaterialCreate = new();

        [BoxGroup("Create Material")] 
        public string materialPath = "Assets/Materials/";
        [BoxGroup("Create Material")]
        [Button("Create Material", 50)]
        private void CreateMaterialFollowTexture()
        {
            foreach (var sprite in spritesMaterialCreate)
            {
                if (sprite == null) continue;
                var material = new Material(Shader.Find("Standard"));
                material.mainTexture = sprite;
                material.name = sprite.name;
                AssetDatabase.CreateAsset(material, $"{materialPath}{sprite.name}.mat");
            }
        }
    }
}

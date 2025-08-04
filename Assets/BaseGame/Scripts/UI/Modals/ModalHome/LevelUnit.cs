using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnit : MonoBehaviour
{
    public RectTransform rectTransform;
    public Image bgImg;
    public Sprite clearedSpr;
    public Sprite lockedSpr;
    public Sprite currentSpr;
    public Sprite hardSpr;
    public Sprite superHardSpr;
    public TextMeshProUGUI levelTxt;
    public TextMeshProUGUI titleTxt;
    public Vector2 currentLevelSize;
    public Vector2 normalLevelSize;

    public void Init(int level)
    {
        if(level < GameManager.Instance.currentLevel.Value)
        {
            bgImg.sprite = clearedSpr;
            levelTxt.text = "";
            titleTxt.text = "";
            rectTransform.sizeDelta = normalLevelSize;
        }
        else if (level == GameManager.Instance.currentLevel.Value)
        {
            bgImg.sprite = currentSpr;
            levelTxt.text = "Level";
            levelTxt.text = level.ToString();
            levelTxt.fontSize = 135f;
            titleTxt.fontSize = 70f;
            rectTransform.sizeDelta = currentLevelSize;
        }
        else
        {
            bgImg.sprite = lockedSpr;
            levelTxt.text = "Level";
            levelTxt.text = level.ToString();
            levelTxt.fontSize = 100f;
            titleTxt.fontSize = 55f;
            rectTransform.sizeDelta = normalLevelSize;
        }
    }
}

using System;
using System.Globalization;
using _Game.Scripts.Manager;
using R3;
using TMPro;
using UnityEngine;

public class FillHeartTimeBar : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI timeTxt;
    public SerializableReactiveProperty<float> fillHeartTime;
    void Start()
    {
        fillHeartTime = RewardManager.Instance.fillHeartTime;
        fillHeartTime.Subscribe(OnFillHeartTimeChanged).AddTo(this);
    }
    void OnFillHeartTimeChanged(float time)
    {
        canvasGroup.alpha = fillHeartTime.Value > 0 ? 1 : 0;
        timeTxt.text = TimeSpan.FromSeconds((double)time)
            .ToString(@"mm\:ss", CultureInfo.InvariantCulture);
    }
}

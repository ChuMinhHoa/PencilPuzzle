using _Game.Scripts.Manager.Etc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BaseGame.Scripts.UI.Other
{
    public class TimeBar : MonoBehaviour
    {
        [field: SerializeField] private TextMeshProUGUI TextTime {get; set;}
        [field: SerializeField] private GameObject FrozenGroup {get; set;}
        [field: SerializeField] private Slider FreezeProcess {get; set;}
        private void Start()
        {
            GameGlobalEvent.OnTimeInGameChange += UpdateTimeBar;
            GameGlobalEvent.OnFreezeTimeChange += UpdateFreezeBar;
        }
        private void OnDestroy()
        {
            GameGlobalEvent.OnTimeInGameChange -= UpdateTimeBar;
            GameGlobalEvent.OnFreezeTimeChange -= UpdateFreezeBar;
        }

        public void InitTimeBar(float time)
        {
            int roundTime = Mathf.RoundToInt(time);
            TextTime.SetTextFormat("{0:00}:{1:00}", roundTime / 60, roundTime % 60);
            FrozenGroup.SetActive(false);
        }

        private void UpdateTimeBar(float currentTime, float maxTime, float deltaTime)
        {
            int roundTime = Mathf.RoundToInt(currentTime);
            TextTime.SetTextFormat("{0:00}:{1:00}", roundTime / 60, roundTime % 60);
        }

        private void UpdateFreezeBar(float currentTime, float maxTime, float deltaTime)
        {
            if (maxTime < 0.1f) return;
            FrozenGroup.SetActive(currentTime > 0);
            FreezeProcess.value = Mathf.Clamp01(currentTime / maxTime);
        }
    }
}
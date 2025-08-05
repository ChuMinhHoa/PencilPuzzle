using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Manager;
using _Game.Scripts.Manager.Etc;
using _Game.Scripts.UI.Core;
using BaseGame.Scripts.UI.Modals;
using Core.UI;
using Core.UI.Modals;
using CoreData;
using Cysharp.Text;
using Manager;
using R3;
using TMPro;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Other
{
    public class UILifeResource : MonoBehaviour
    {
        [field: SerializeField] public CurrencyType CurrencyType { get; private set; } = CurrencyType.Money;
        [field: SerializeField] public Image Icon { get; private set; }
        [field: SerializeField] public Sprite InfinityIcon { get; private set; }
        [field: SerializeField] public Sprite NormalIcon { get; private set; }
        [field: SerializeField] private TextMeshProUGUI TextAmount {get; set;}
        [field: SerializeField] private bool RealValue {get; set;} = true;
        [field: SerializeField] public Button ShowResourseBtn {get; set;}
        public SerializableReactiveProperty<float> infinityLifeTime = new(0);
        private float Amount { get; set; }
        private void Start()
        {
            GameResource gameResource = ResourceData.Instance.GetResource(ResourceType.Currency, (int)CurrencyType);
            gameResource.Value.Subscribe(OnResourceChange).AddTo(this);
            if (ShowResourseBtn != null)
            {
                ShowResourseBtn.SetOnClickDestination(ShowResource);
            }
            infinityLifeTime = InGameDataManager.Instance.InGameData.ResourceData.GetResource(ResourceType.Special, (int)SpecialResourceType.InfiniteLife).Value;
            infinityLifeTime.Subscribe(OnInfinityLifeTimeChange).AddTo(this);
        }
        private void OnResourceChange(float value)
        {
            if(infinityLifeTime.Value > 0)
            {
                return;
            }
            Icon.sprite = NormalIcon;
            if(GameManager.Instance._inGame && CurrencyType == CurrencyType.Life)
            {
                RealValue = false;
            }
            Amount = RealValue ? value : value + 1;
            TextAmount.SetText($"{Amount:0}");
        }
        
        void OnInfinityLifeTimeChange(float value)
        {
            if(value > 0)
            {
                Icon.sprite = InfinityIcon;
                TextAmount.text = $"{TimeUtil.TimeToString(value, TimeFommat.Symbol)}";
            }
            else
            {
                OnResourceChange(ResourceData.Instance.GetResource(ResourceType.Currency, (int)CurrencyType).Value.Value);
            }
        }

        void ShowResource()
        {
            switch (CurrencyType)
            {
                case CurrencyType.None:
                    break;
                case CurrencyType.Money:
                    break;
                case CurrencyType.Gem:
                    break;
                case CurrencyType.Star:
                    break;
                case CurrencyType.Life:
                    ViewOptions viewOptions = new ViewOptions(nameof(ModalFillHeart));
                    ModalContainer.Find(ContainerKey.Modals).PushAsync(viewOptions);
                    break;
                default:
                    break;
            }
        }
    }
}


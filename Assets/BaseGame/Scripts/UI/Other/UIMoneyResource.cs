using System;
using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using BaseGame.Scripts.UI.Modals;
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
    public class UIMoneyResource : MonoBehaviour
    {
        [field: SerializeField] public CurrencyType CurrencyType { get; private set; } = CurrencyType.Money;
        [field: SerializeField] private TextMeshProUGUI TextAmount {get; set;}
        [field: SerializeField] private bool RealValue {get; set;} = true;
        [field: SerializeField] public Button ShowResourseBtn {get; set;}
        private float Amount { get; set; }
        private void Start()
        {
            GameResource gameResource = ResourceData.Instance.GetResource(ResourceType.Currency, (int)CurrencyType);
            gameResource.Value.Subscribe(OnResourceChange).AddTo(this);
            if (ShowResourseBtn != null)
            {
                ShowResourseBtn.SetOnClickDestination(ShowResource);
            }
        }
        private void OnResourceChange(float value)
        {
            if(GameManager.Instance._inGame && CurrencyType == CurrencyType.Life)
            {
                RealValue = false;
            }
            Amount = RealValue ? value : value + 1;
            TextAmount.SetText($"{Amount:0}");
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
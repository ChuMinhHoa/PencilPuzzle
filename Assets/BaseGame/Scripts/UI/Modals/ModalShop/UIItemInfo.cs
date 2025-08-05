using _Game.Scripts.Manager.Etc;
using CoreData;
using Cysharp.Threading.Tasks;
using LitMotion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIItemInfo : MonoBehaviour
{
    [field: SerializeField] public bool InitIcon { get; set; } = true;
    [field: SerializeField] public bool PlayAnim { get; set; }
    [field: SerializeField] public Image ImgItemIcon { get; set; }
    [field: SerializeField] public TextMeshProUGUI TxtItemAmount1 { get; set; }
    [field: SerializeField] public Text TxtItemAmount2 { get; set; }

    public void Init(GameResource gameResource)
    {
        if(PlayAnim)
        {
            AnimateItem();
        }
        switch (gameResource.Type)
        {
            case ResourceType.None:
                break;
            case ResourceType.Currency:
                if (ImgItemIcon != null && InitIcon)
                    ImgItemIcon.sprite =
                        ItemGlobalConfig.Instance.GetItemSprite((CurrencyType)gameResource.SpecificType);
                if((CurrencyType)gameResource.SpecificType == CurrencyType.Money && ImgItemIcon != null && InitIcon)
                    ImgItemIcon.sprite = ItemGlobalConfig.Instance.GoldRewardIcon;
                if (TxtItemAmount1 != null)
                {
                    TxtItemAmount1.text = $"{gameResource.Value.Value}";
                }
                if (TxtItemAmount2 != null)
                {
                    TxtItemAmount2.text = $"{gameResource.Value.Value}";
                }

                break;
            case ResourceType.Booster:
                if (ImgItemIcon != null && InitIcon)
                    ImgItemIcon.sprite =
                        ItemGlobalConfig.Instance.GetItemSprite((BoosterType)gameResource.SpecificType);
                if (TxtItemAmount1 != null)
                {
                    TxtItemAmount1.text = $"{gameResource.Value.Value}";
                }
                if (TxtItemAmount2 != null)
                {
                    TxtItemAmount2.text = $"{gameResource.Value.Value}";
                }

                break;
            case ResourceType.Special:
                Debug.Log((SpecialResourceType)gameResource.SpecificType);
                if (ImgItemIcon != null && InitIcon)
                    ImgItemIcon.sprite =
                        ItemGlobalConfig.Instance.GetItemSprite((SpecialResourceType)gameResource.SpecificType);
                if (TxtItemAmount1 != null)
                {
                    SpecialResourceType specialResourceType = (SpecialResourceType)gameResource.SpecificType;
                    switch (specialResourceType)
                    {
                        case SpecialResourceType.NoAds:
                        case SpecialResourceType.BattlePass:
                        case SpecialResourceType.Vip:
                            if (TxtItemAmount1 != null)
                            {
                                TxtItemAmount1.text = $"";
                            }
                            if (TxtItemAmount2 != null)
                            {
                                TxtItemAmount2.text = $"";
                            }
                            break;
                        case SpecialResourceType.InfiniteLife:
                            if (TxtItemAmount1 != null)
                            {
                                TxtItemAmount1.text =
                                $"{TimeUtil.TimeToString(gameResource.Value.Value, TimeFommat.Keyword)}";
                            }
                            if (TxtItemAmount2 != null)
                            {
                                TxtItemAmount2.text =
                                $"{TimeUtil.TimeToString(gameResource.Value.Value, TimeFommat.Keyword)}";
                            }
                            break;
                    }
                }

                break;
        }
    }

    public void AnimateItem()
    {
        LMotion.Create(ImgItemIcon.transform.position, ImgItemIcon.transform.position, 0.5f).Bind(x => ImgItemIcon.transform.position = x).AddTo(this);
    }

    public async UniTask ShowItemAnim(float delay = 0f)
    {
        transform.localScale = Vector3.zero;
        await UniTask.Delay((int)(delay * 1000), cancellationToken: this.GetCancellationTokenOnDestroy());
        await LMotion.Create(0f, 1f, 0.15f).WithEase(Ease.OutBack)
            .Bind(x => transform.localScale = Vector3.one * x)
            .AddTo(this);
    }
    
}

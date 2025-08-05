using System;
using System.Collections.Generic;
using System.Globalization;
using _Game.Scripts.Manager;
using Core.UI;
using Manager;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using R3;

public class UIShopPack : MonoBehaviour
{
    [field: SerializeField] public PackageName PackageName { get; set; }
    [field: SerializeField] public TextMeshProUGUI PackageNameTxt { get; set; }
    [field: SerializeField] public List<UIItemInfo> UIItemInfos { get; set; } = new();
    [field: SerializeField] public ShopPackageDataConfig ShopPackageDataConfig { get; set; } = new();
    [field: SerializeField] public Button PurchaseBtn { get; set; }
    [field: SerializeField] public TextMeshProUGUI TxtPackPrice { get; set; }
    private UnityAction callBackFromParent;
    bool showReward;
    bool reInit;
    [BoxGroup("Editor Only")]
    [SerializeField] Image bgImage;
    [BoxGroup("Editor Only")]
    [SerializeField] List<Image> bgItem;
    [BoxGroup("Editor Only")]
    [SerializeField] Image mainItems;
    [BoxGroup("Editor Only")]
    [SerializeField] Sprite bgSprite;
    [BoxGroup("Editor Only")]
    [SerializeField] Sprite bgItemSprite;
    [BoxGroup("Editor Only")]
    [SerializeField] Sprite mainItemsSprite;


    private void OnEnable()
    {
        Init();
    }

    private void Start()
    {
        PurchaseBtn.SetOnClickDestination(OnPurchase);
        InGameDataManager.Instance.InGameData.SettingData.LanguageCode
            .Subscribe(OnLanguageChanged).AddTo(this);
    }

    public void Init(UnityAction callBackFromParent = null, bool showReward = true, bool reInit = true)
    {
        if(!CheckPackAvaiable())
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        this.callBackFromParent = callBackFromParent;
        this.showReward = showReward;
        this.reInit = reInit;
        InitUI();
    }

    [Button]
    public void InitDefault()
    {
        if (bgImage != null)
            bgImage.sprite = bgSprite;
        for (int i = 0; i < bgItem.Count; i++)
        {
            if (bgItem[i] != null)
            {
                bgItem[i].sprite = bgItemSprite;
            }
        }
        if (mainItems != null)
            mainItems.sprite = mainItemsSprite;
        ShopPackageDataConfig = ShopGlobalConfig.Instance.GetShopPackageDataConfig(PackageName);
        SetPackageName();
    }

    [Button]
    public void InitUI()
    {
        ShopPackageDataConfig = ShopGlobalConfig.Instance.GetShopPackageDataConfig(PackageName);
        if(ShopPackageDataConfig == null)
        {
            // Debug.LogError($"ShopPackageDataConfig Null {PackageName}");
            gameObject.SetActive(false);
            return;
        }
        SetPackPrice();
        SetPackageName();
        //int itemCount = 0;
        for (int i = 0; i < UIItemInfos.Count; i++)
        {
            if (i < ShopPackageDataConfig.GameResources.Count )
            {
                if (ShopPackageDataConfig.GameResources[i] != null
                    && ShopPackageDataConfig.GameResources[i].Value.Value > 0)
                {
                    UIItemInfos[i].Init(ShopPackageDataConfig.GameResources[i]);
                    UIItemInfos[i].gameObject.SetActive(true);
                }
                else
                {
                    UIItemInfos[i].gameObject.SetActive(false);
                }
            }
            else
            {
                UIItemInfos[i].gameObject.SetActive(false);
            }
        }
    }

    bool CheckPackAvaiable()
    {
        if(PackageName == PackageName.removeads)
            return !InGameDataManager.Instance.InGameData.ResourceData.IsPackPurchased(PackageName.ToString());
        return true;
    }
    
    void SetPackPrice()
    {
        switch (ShopPackageDataConfig.priceType)    
        {
            case PriceType.IAP:
                //TxtPackPrice.text = $"{InGameIAPController.Instance.GetIAPPackage(ShopPackageDataConfig.packageId).GetPrice()}";
                PurchaseBtn.interactable = true;
                break;
            // case PriceType.Ads:
            //     TxtPackPrice.text = "Free";
            //     AdsPurchaseObject.SetActive(true);
            //     adsWaitingTime = InGameDataManager.Instance.InGameData.ResourceDataSave.GetPackageCondion(ShopPackageDataConfig.packageName);
            //     CheckPurchaseByAds();
            //     break;
            case PriceType.Money:
                TxtPackPrice.text = $"{ShopPackageDataConfig.price}";
                PurchaseBtn.interactable = true;
                break;
        }
        
    }

    void OnPurchase()
    {
        //UIAnimationBase.ButtonBasic(PurchaseBtn.transform);
        Debug.Log($"OnPurchase {ShopPackageDataConfig.packageName} - {ShopPackageDataConfig.priceType}");
        switch (ShopPackageDataConfig.priceType)    
        {
            case PriceType.IAP:
                OnIAPPurchase();
                break;
            case PriceType.Ads:
                OnAdsPurchase();
                break;
            case PriceType.Money:
                OnMoneyPurchase();
                break;
        }
    }

    void OnIAPPurchase()
    {
        //InGameIAPController.EventPurchaseIAPProduct?.Invoke(ShopPackageDataConfig.packageId, PurchaseSuccess);
    }

    void OnAdsPurchase()
    {
        //InGameAdsController.EventShowAdsReward?.Invoke($"AdsRw_GetResource_{ShopPackageDataConfig.packageName}", OnAdsPurchaseSuccess, null);
    }

    void OnAdsPurchaseSuccess()
    {
        // InGameDataManager.Instance.InGameData.ResourceDataSave.SetPackageCondition(ShopPackageDataConfig.packageName, DateTime.Now.AddMinutes(10).ToString(CultureInfo.InvariantCulture));
        //         adsWaitingTime = InGameDataManager.Instance.InGameData.ResourceDataSave.GetPackageCondion(ShopPackageDataConfig.packageName);
        //         CheckPurchaseByAds();
        //         PurchaseSuccess();
    }
    
    void OnMoneyPurchase()
    {
        PurchaseSuccess();
    }

    public void PurchaseSuccess()
    {
        RewardManager.Instance.AddReward(ShopPackageDataConfig.GameResources);
        if (showReward)
        {
            RewardManager.Instance.ShowReward();
        }
        InGameDataManager.Instance.InGameData.ResourceData.AddPurchasedPack(ShopPackageDataConfig.packageName.ToString());
        callBackFromParent?.Invoke();
        if(reInit)
            gameObject.SetActive(CheckPackAvaiable());
    }

    private void OnLanguageChanged(string language)
    {
        SetPackageName();
    }

    void SetPackageName()
    {
        if (PackageNameTxt != null)
        {
            // string language = InGameDataManager.Instance.InGameData.SettingDataSave.GetLanguage().Value;
            // var data = LanguageGlobalConfig.Instance.GetTextData(
            //     InGameDataManager.Instance.InGameData.SettingDataSave.GetLanguage().Value,
            //     ShopPackageDataConfig.packageNameToUI);
            // PackageNameTxt.text = $"{data.textValue}";
            // PackageNameTxt.font = GameManager.Instance.GetFont();
            PackageNameTxt.text = $"{ShopPackageDataConfig.packageNameToUI}";
        }
    }
}

using System;
using System.Collections.Generic;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.Extension;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopGlobalConfig", menuName = "GlobalConfigs/ShopGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ShopGlobalConfig : GlobalConfig<ShopGlobalConfig>
{
    private string linkSheet = "1CfvMgG4uKwWG_6-GMKni8ArPv4paKwC-AF1oiD6GWUU";
    string sheetTab = "ShopConfig";
    public string packageName;
    public List<ShopPackageDataConfig> shopPackages;
    public List<GameResourseData> gameResourseDatas;
    public Sprite noAdsIcon;
    
    #if UNITY_EDITOR

    [Button]
    void GetPackageName()
    {
        // Get package name of project
        packageName = Application.identifier;
    }
    
    [Button]
    private async UniTask LoadDataFromGoogleSheet()
    {
        linkSheet = DefaultGlobalConfig.Instance.LinkSheet;
        GetPackageName();
        shopPackages.Clear();
        List<Dictionary<string, string>> tableData = await ABakingSheet.GetDataTable(linkSheet, sheetTab);
        foreach (var item in tableData)
        {
            ShopPackageDataConfig shopPackageDataConfig;
            if (item["PackageId"] == ";")
            {
                shopPackageDataConfig = shopPackages[^1];
            }
            else
            {
                shopPackageDataConfig = new ShopPackageDataConfig();
                shopPackages.Add(shopPackageDataConfig);
                shopPackageDataConfig.packageNameToUI = item["PackageNameToUI"];
                //shopPackageDataConfig.packageId = $"{packageName}.{item["PackageId"]}";
                shopPackageDataConfig.packageId = $"{item["PackageId"]}";
                string packName = item["PackageId"];
                if (packName.Contains("."))
                {
                    string[] split = packName.Split('.');
                    packName = split[split.Length - 1];
                }
                shopPackageDataConfig.packageName = (PackageName)Enum.Parse(typeof(PackageName), packName);
                shopPackageDataConfig.GameResources = new List<GameResource>();
                shopPackageDataConfig.priceType = (PriceType)Enum.Parse(typeof(PriceType), item["PriceType"]);
                shopPackageDataConfig.price = float.Parse(item["Price"]);
                
            }
            
            ResourceType gameResourceType = (ResourceType)Enum.Parse(typeof(ResourceType), item["ResourceType"]);
            if (gameResourceType != ResourceType.None)
            {
                float amount = float.Parse(item["Value"]);
                string specificType = item["SpecificType"];
                GameResource GameResource = new GameResource(gameResourceType, GetSpecificType(gameResourceType, specificType), amount);
                shopPackageDataConfig.GameResources.Add(GameResource);
            }
        }
    }
    #endif
    
    public int GetSpecificType(ResourceType resourceType, string specificType)
    {
        switch (resourceType)
        {
            case ResourceType.None:
                return 0;
            case ResourceType.Currency:
                return (int)((CurrencyType)Enum.Parse(typeof(CurrencyType), specificType));
            case ResourceType.Booster:
                return (int)((BoosterType)Enum.Parse(typeof(BoosterType), specificType));
            case ResourceType.Special:
                return (int)((SpecialResourceType)Enum.Parse(typeof(SpecialResourceType), specificType));
            default:
                return 0;           
        }
    }
    
    public ShopPackageDataConfig GetShopPackageDataConfig(PackageName packageName)
    {
        return shopPackages.Find(x => x.packageName == packageName);
    }
}

public enum PackageName
{
    None,
    removeads = 1,
    revivepack = 2,
    bigbundle = 5,
    greatbundle = 6,
    ultrabundle = 7,
    removeadssale = 10,
    starterpack = 11,
    limitedpack = 12,
    superiorbundle = 13,
    legendarybundle = 14,
    
    coin1 = 101,
    smallcoin = 102,
    coin3 = 103,
    coin4 = 104,
    coin5 = 105,
    coin6 = 106,
    coin2 = 107,
    coin7 = 108,
    
    battlepass = 200,
}
public enum PriceType
{
    IAP,
    Ads,
    Money,
}

[System.Serializable]
public class ShopPackageDataConfig
{
    public PackageName packageName;
    public string packageNameToUI;
    public string packageTag;
    public string packageId;
    public PriceType priceType;
    public float price;
    public List<GameResource> GameResources;
}

[System.Serializable]
public class GameResourseData
{
    public ResourceType resourceType;
    public Sprite icon;
}
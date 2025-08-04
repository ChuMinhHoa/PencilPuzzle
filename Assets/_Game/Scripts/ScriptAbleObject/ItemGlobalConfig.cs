using System.Collections.Generic;
using CoreData;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using TW.Utility.Extension;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemGlobalConfig", menuName = "GlobalConfigs/ItemGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ItemGlobalConfig : GlobalConfig<ItemGlobalConfig>
{
    private string linkSheet = "1CfvMgG4uKwWG_6-GMKni8ArPv4paKwC-AF1oiD6GWUU";
    public List<CurrencyData> CurrencyDatas;
    public List<BoosterData> BoosterDatas;
    public List<SpecialItemData> SpecialItemDatas;
    public List<ProfileSpriteData> AvatarSpriteDatas;
    public List<ProfileSpriteData> AvatarFrameSpriteDatas;
    public List<BlockData> BlockDatas;
    public Sprite GoldRewardIcon;
    
    public Sprite GetItemSprite(CurrencyType currencyType)
    {
        return CurrencyDatas.Find(x => x.CurrencyType == currencyType).Sprite;
    }
    public Sprite GetItemSprite(BoosterType boosterType)
    {
        return BoosterDatas.Find(x => x.BoosterType == boosterType).Sprite;
    }
    public Sprite GetItemSprite(SpecialResourceType specialResourceType)
    {
        return SpecialItemDatas.Find(x => x.SpecialResourceType == specialResourceType).Sprite;
    }
    
    public BoosterData GetBoosterData(BoosterType boosterType)
    {
        return BoosterDatas.Find(x => x.BoosterType == boosterType);
    }
    
    public ProfileSpriteData GetAvatarSprite(int id)
    {
        return AvatarSpriteDatas.Find(x => x.Id == id);
    }
    public ProfileSpriteData GetAvatarFrameSprite(int id)
    {
        return AvatarFrameSpriteDatas.Find(x => x.Id == id);
    }
    
    public Sprite GetAvatarSpr(int index)
    {
        return AvatarSpriteDatas[index].Sprite;
    }
    
    public Sprite GetFrameSpr(int index)
    {
        return AvatarFrameSpriteDatas[index].Sprite;
    }
    
    public int GetRandomAvatarIndex()
    {
        return Random.Range(0, AvatarSpriteDatas.Count);
    }

    public int GetRandomFrameIndex()
    {
        return Random.Range(0, AvatarFrameSpriteDatas.Count);
    }

    public BoosterData GetBoosterData(int level)
    {
        return BoosterDatas.Find(x => x.UnlockLevel == level);
    }
    
    public BlockData GetBlockData(int level)
    {
        return BlockDatas.Find(x => x.UnlockLevel == level);
    }
    
    public string GetNewBoosterUnlock(int level)
    {
        string boosterName = string.Empty;
        for (var i = 0; i < BoosterDatas.Count; i++)
        {
            if(BoosterDatas[i].UnlockLevel == level)
            {
                boosterName = $"Booster_{BoosterDatas[i].BoosterType}";
                break;
            }
        }
        return boosterName;
    }
    
    #if UNITY_EDITOR
    [Button]
    private void LoadAvatars()
    {
        AvatarSpriteDatas = new();
        AvatarFrameSpriteDatas = new();
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Avatar");
        int avatarCount = 0;
        int frameCount = 0;
        for (int i = 0; i < sprites.Length; i++)
        {
            // Check if sprite name contains "avatar" or "frame"
            if (sprites[i].name.Contains("Avatar"))
            {
                AvatarSpriteDatas.Add(new ProfileSpriteData { Id = avatarCount, Sprite = sprites[i] });
                avatarCount++;
            }
            else if (sprites[i].name.Contains("Frame"))
            {
                AvatarFrameSpriteDatas.Add(new ProfileSpriteData { Id = frameCount, Sprite = sprites[i] });
                frameCount++;
            }
            
        }
        Debug.Log("Loaded " + sprites.Length + " avatars.");
    }
    [Button]
    private async UniTask LoadDataFromGoogleSheet()
    {
        BlockDatas = new();
        List<Dictionary<string, string>> tableData = await ABakingSheet.GetDataTable(linkSheet, "SpecialBlock");
        Debug.Log("Loaded " + tableData.Count + " blocks.");
        foreach (var item in tableData)
        {
            BlockData blockData = new();
            BlockDatas.Add(blockData);
            blockData.BlockName = item["BlockName"];
            blockData.Description = item["Description"];
            blockData.UnlockLevel = int.Parse(item["UnlockLevel"]);
        }
    }
    #endif
}

[System.Serializable]
public class CurrencyData
{
    public CurrencyType CurrencyType;
    [PreviewField] public Sprite Sprite;
}

[System.Serializable]
public class BoosterData
{
    [PreviewField] public Sprite Sprite;
    public BoosterType BoosterType;
    public string BoosterName;
    public string Description;
    public float Price;
    public float Effect;
    public int UnlockLevel;
    public SkeletonDataAsset skeletonDataAsset;
    public Material skeletonMaterial;
    public bool limitAds;
}

[System.Serializable]
public class SpecialItemData
{
    public SpecialResourceType SpecialResourceType;
    [PreviewField] public Sprite Sprite;
}

[System.Serializable]
public class ProfileSpriteData
{
    public int Id;
    [PreviewField] public Sprite Sprite;
}

[System.Serializable]
public class BlockData
{
    [PreviewField] public Sprite Sprite;
    public string BlockName;
    public string Description;
    public int UnlockLevel;
    public SkeletonDataAsset skeletonDataAsset;
    public Material skeletonMaterial;
}
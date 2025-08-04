using System.Collections.Generic;
using CoreData;
using Sirenix.Utilities;
using UnityEngine;
using R3;

[CreateAssetMenu(fileName = "DefaultGlobalConfig", menuName = "GlobalConfigs/DefaultGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class DefaultGlobalConfig : GlobalConfig<DefaultGlobalConfig>
{
    [field: SerializeField] public string LinkSheet {get; private set;}
    [field: SerializeField] public bool PlayGameAtStart { get; set; } = true;
    [field: SerializeField] public bool ShowModalStartGame {get; private set;} 
    [field: SerializeField] public bool ShowModalOfferBooster {get; private set;} 
    [field: SerializeField] public bool ActiveDailyReward {get; private set;} 
    [field: SerializeField] public float WinGameReward {get; private set;} = 10;
    [field: SerializeField] public int RaceUnlock {get; private set;}  = 10;
    [field: SerializeField] public int DefaultLife {get; private set;}  = 5;
    [field: SerializeField] public int DefaultBackToMenuLevel {get; private set;}  = 5;
    [field: SerializeField] public int DefaultShowWinRewardAds {get; private set;}  = 5;
    [field: SerializeField] public int DefaultActiveQuest {get; private set;}  = 10;
    [field: SerializeField] public int DefaultDailyGift {get; private set;}  = 15;
    [field: SerializeField] public int DefaultDailyReward {get; private set;}  = 15;
    [field: SerializeField] public float DefaultFillHeartTime {get; private set;}  = 5; // minute
    [field: SerializeField] public float DefaultFullHeartCoin {get; private set;}  = 800;
    [field: SerializeField] public float ReviveCoin {get; private set;}  = 900;
    [field: SerializeField] public float ReviveTime {get; private set;}  = 45;
    [field: SerializeField] public SynchronizedReactiveProperty<int> DeadCountRemain {get; private set;}  = new(1);
    [field: SerializeField] public ReviveValue ReviveValue {get; private set;}  = new ReviveValue(30,1000);
    [field: SerializeField] public float BoosterAmountOnPurchase {get; private set;}  = 1;
    [field: SerializeField] public float DefaultTimeFillHeart{get; private set;}  = 10;
    [field: SerializeField] public List<string> AdminId{get; private set;}
    [field: SerializeField] public LevelRemoteConfig LevelRemoteConfig{get; private set;}
    [field: SerializeField] public int LevelUnlockFreezeClock {get; private set;}
    [field: SerializeField] public int FreezeClockCost {get; private set;}
    [field: SerializeField] public int LevelUnlockPropeller {get; private set;}
    [field: SerializeField] public int PropellerCost {get; private set;}
    [field: SerializeField] public int LevelUnlockMagnet {get; private set;}
    [field: SerializeField] public int MagnetCost {get; private set;}
        
    
    
    public void FetchRemoteConfig()
    {
        try
        {
            // LevelGlobalConfig.Instance.TryFetchLevelTimeData().Forget();
            //
            // ConfigValue winRewardValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_win_reward);
            // WinGameReward = (int)(winRewardValue.DoubleValue);
            //
            // ConfigValue backToMenuValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_back_to_menu);
            // DefaultBackToMenuLevel = (int)(backToMenuValue.DoubleValue);
            //
            // ConfigValue reviveTimeValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_revive_time);
            // ReviveTime = (float)(reviveTimeValue.DoubleValue);
            //
            // ConfigValue reviveCoinValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_revive_coin);
            // ReviveCoin = (float)(reviveCoinValue.DoubleValue);

            // FetchReviveValue();
            // FetchLevelRemoteConfig();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in FetchRemoteConfig: {ex.Message}");
            SetDefaultReviveValue();
        }
    }

    void FetchReviveValue()
    {
        // // Safe JSON parsing with error handling
        // ConfigValue reviveValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_revive_value);
        // if (!string.IsNullOrEmpty(reviveValue.StringValue))
        // {
        //     try
        //     {
        //         // Validate JSON before parsing
        //         string jsonString = reviveValue.StringValue.Trim();
        //         
        //         if (IsValidJson(jsonString))
        //         {
        //             ReviveValue = JsonUtility.FromJson<ReviveValue>(jsonString);
        //             Debug.Log($"Successfully parsed ReviveValue: {jsonString}");
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"Invalid JSON format for key_revive_value: {jsonString}");
        //             SetDefaultReviveValue();
        //         }
        //     }
        //     catch (System.ArgumentException ex)
        //     {
        //         Debug.LogError($"JSON parse error for key_revive_value: {ex.Message}. Using default values.");
        //         Debug.LogError($"Problematic JSON: {reviveValue.StringValue}");
        //         SetDefaultReviveValue();
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Unexpected error parsing key_revive_value: {ex.Message}");
        //         SetDefaultReviveValue();
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("key_revive_value is null or empty, using default values");
        //     SetDefaultReviveValue();
        // }
    }
    
    private void SetDefaultReviveValue()
    {
        ReviveValue = new ReviveValue(45, 1);
        Debug.Log("Using default ReviveValue: reviveTime=45, deadCountRemain=1");
    }

    void FetchLevelRemoteConfig()
    {
        // // Safe JSON parsing with error handling
        // ConfigValue levelValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_level_config);
        // if (!string.IsNullOrEmpty(levelValue.StringValue))
        // {
        //     try
        //     {
        //         // Validate JSON before parsing
        //         string jsonString = levelValue.StringValue.Trim();
        //         
        //         if (IsValidJson(jsonString))
        //         {
        //             LevelRemoteConfig = JsonUtility.FromJson<LevelRemoteConfig>(jsonString);
        //             Debug.Log($"Successfully parsed LevelRemoteConfig: {jsonString}");
        //         }
        //         else
        //         {
        //             Debug.LogWarning($"Invalid JSON format for key_level_config: {jsonString}");
        //             SetDefaultLevelRemoteConfig();
        //         }
        //     }
        //     catch (System.ArgumentException ex)
        //     {
        //         Debug.LogError($"JSON parse error for key_level_config: {ex.Message}. Using default values.");
        //         Debug.LogError($"Problematic JSON: {levelValue.StringValue}");
        //         SetDefaultLevelRemoteConfig();
        //     }
        //     catch (System.Exception ex)
        //     {
        //         Debug.LogError($"Unexpected error parsing key_level_config: {ex.Message}");
        //         SetDefaultLevelRemoteConfig();
        //     }
        // }
        // else
        // {
        //     Debug.LogWarning("key_level_config is null or empty, using default values");
        //     SetDefaultLevelRemoteConfig();
        // }
        // LevelGlobalConfig.Instance.FetchLevelRemoteConfig();
    }

    void SetDefaultLevelRemoteConfig()
    {
        LevelRemoteConfig = new LevelRemoteConfig();
        Debug.Log("Using default LevelRemoteConfig with empty levelUpdateConfigs");
    }

    private bool IsValidJson(string jsonString)
    {
        if (string.IsNullOrWhiteSpace(jsonString))
            return false;
        
        jsonString = jsonString.Trim();
    
        // Basic JSON validation
        return (jsonString.StartsWith("{") && jsonString.EndsWith("}")) ||
               (jsonString.StartsWith("[") && jsonString.EndsWith("]"));
    }

    public LevelUpdateConfig GetLevelConfigTime(int level)
    {
        if(LevelRemoteConfig != null && LevelRemoteConfig.levelUpdateConfigs != null)
        {
            for (var i = 0; i < LevelRemoteConfig.levelUpdateConfigs.Count; i++)
            {
                if(LevelRemoteConfig.levelUpdateConfigs[i].level == level)
                {
                    return LevelRemoteConfig.levelUpdateConfigs[i];
                }
            }
        }
        return null;
    }

    public bool CheckAdmin(string id)
    {
        return AdminId.Contains(id);
    }
    
    public int GetLevelUnlockBooster(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.FreezeClock:
                return LevelUnlockFreezeClock;
            case BoosterType.Propeller:
                return LevelUnlockPropeller;
            case BoosterType.Magnet:
                return LevelUnlockMagnet;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    public int GetBoosterCost(BoosterType type)
    {
        switch (type)
        {
            case BoosterType.FreezeClock:
                return FreezeClockCost;
            case BoosterType.Propeller:
                return PropellerCost;
            case BoosterType.Magnet:
                return MagnetCost;
            default:
                throw new System.ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}
[System.Serializable]
public class ReviveValue 
{
    public int reviveTime;
    public int deadCountRemain;
    
    public ReviveValue(int reviveTime, int deadCountRemain)
    {
        this.reviveTime = reviveTime;
        this.deadCountRemain = deadCountRemain;
    }
}

[System.Serializable]
public class LevelRemoteConfig
{
    public List<LevelUpdateConfig> levelUpdateConfigs;
    
    public LevelRemoteConfig()
    {
        levelUpdateConfigs = new List<LevelUpdateConfig>();
    }
}

[System.Serializable]
public class LevelUpdateConfig
{
    public int level;
    public int time;
}
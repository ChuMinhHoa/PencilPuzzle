
using _Game.Scripts.Manager;

namespace BaseGame.Scripts.Data
{
    [System.Serializable]
    public class UserData
    {
        public static UserData Instance => InGameDataManager.Instance.InGameData.UserData;
        public int CurrentLevel = 1;
        public int LastLevelPlayed = 1;
    }
}
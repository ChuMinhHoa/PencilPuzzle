using System;

namespace _Game.Scripts.Manager.Etc
{
    public static class GameGlobalEvent
    {
        // public static Action<Hole> OnStartDragHole {get; set;}
        // public static Action<Hole> OnEndDragHole {get; set;}
        
        public static Action CheckWinCondition {get; set;}
        public static Action OnLevelWin {get; set;}
        public static Action OnLevelLose {get; set;}
        //public static Action<People, Hole, float> OnPeopleResolve {get; set;}
        public static Action<float, float, float> OnTimeInGameChange { get; set; }
        public static Action<float, float, float> OnFreezeTimeChange { get; set; }
    }
}
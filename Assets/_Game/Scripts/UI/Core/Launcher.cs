using System;
using Core.UI.Activities;
using Core.UI.Screens;
using Cysharp.Threading.Tasks;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine;
using ZBase.UnityScreenNavigator.Core;

namespace _Game.Scripts.UI.Core
{
    public class Launcher : UnityScreenNavigatorLauncher
    {
        protected override void Start()
        {
            base.Start();
            ViewOptions activityLoading = new ViewOptions(nameof(ActivityLoading));
            Memory<object> args = new Memory<object>(new object[]
            {
                (Func<UniTask>)(async () =>
                {
                    ScreenOptions screenOptions = new ScreenOptions(nameof(ScreenMainMenu), stack: false);
                    await ScreenContainer.Find(ContainerKey.Screens).PushAsync(screenOptions);
                }),
                null
            });
            ActivityContainer.Find(ContainerKey.Activities).ShowAsync(activityLoading, args);
        }
    }
}

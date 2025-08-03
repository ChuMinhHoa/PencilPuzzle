using _Game.Scripts.UI.Modal;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using ZBase.UnityScreenNavigator.Core;

namespace _Game.Scripts.UI.Core
{
    public class Launcher : UnityScreenNavigatorLauncher
    {
        protected override void Start()
        {
            base.Start();
            ShowPanel();
        }

        private void ShowPanel()
        {
            // var option = new ViewOptions(nameof(ModalSetting));
            // ModalContainer.Find(ContainerKey.Modals).Push(option);
        }
    }
}

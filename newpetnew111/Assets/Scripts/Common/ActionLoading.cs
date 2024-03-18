using PFramework;
using PFramework.Page;
using PVM.Server;
using UniRx.Async;

namespace PVM
{
    public class ActionLoading : PageBase
    {
        private const string PATH = "Common/UIActionLoading";
        private static ActionLoading currentLoading;

        public static async void Show()
        {
            if (IsShow()) return;
            currentLoading = await Peach.OpenPageAsync<ActionLoading>(PATH);
            DelayCancel();
        }

        public static bool IsShow()
        {
            return currentLoading != null;
        }

        public static void Cancel()
        {
            if (currentLoading != null)
            {
                currentLoading.Close();
                currentLoading = null;
            }
        }

        public static void SetupServerManger()
        {
          
        }

        private static async void DelayCancel()
        {
            await UniTask.Delay(5000);
            Cancel();
        }
    }
}
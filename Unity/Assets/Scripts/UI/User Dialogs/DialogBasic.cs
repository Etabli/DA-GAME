using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public class DialogBasic : Dialog
    {
        [SerializeField]
        protected Button OKButton;

        protected override void Awake()
        {
            base.Awake();

            if (OKButton == null)
                throw new UserDialogException("User dialog's reference to OK button is not set!");

            OKButton.onClick.AddListener(Close);
        }

        public static (Dialog dialog, Task<DialogResult> result) Show(string message)
        {
            var t = new TaskCompletionSource<DialogResult>();

            return (DialogController.Show(message, () => t.TrySetResult(DialogResult.OK)), t.Task);
        }

        public static (Dialog dialog, Task<DialogResult> result) ShowBlocking(string message)
        {
            var dialogResult = Show(message);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }
    }
}

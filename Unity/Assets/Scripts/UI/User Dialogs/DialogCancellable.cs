using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public class DialogCancellable : DialogBasic
    {
        public event Action OnAccept;
        public event Action OnCancel;

        [SerializeField]
        protected Button CancelButton;

        protected override void Awake()
        {
            base.Awake();

            if (CancelButton == null)
                throw new UserDialogException("User dialogs's reference to Cancel button is not set!");

            CancelButton.onClick.AddListener(Close);
            CancelButton.onClick.AddListener(Cancel);
            OKButton.onClick.AddListener(Accept);
        }

        void Accept() => OnAccept?.Invoke();
        void Cancel() => OnCancel?.Invoke();

        public static new (DialogCancellable dialog, Task<DialogResult> result) Show(string message)
        {
            var t = new TaskCompletionSource<DialogResult>();

            return (DialogController.Show(message, () => t.TrySetResult(DialogResult.OK), () => t.TrySetResult(DialogResult.OK)), t.Task);
        }

        public static new (DialogCancellable dialog, Task<DialogResult> result) ShowBlocking(string message)
        {
            var dialogResult = Show(message);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }
    }
}

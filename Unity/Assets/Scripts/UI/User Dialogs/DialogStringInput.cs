using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public class DialogStringInput : DialogCancellable
    {
        public event Action<string> OnSubmit;

        [SerializeField]
        Text placeholderText;
        [SerializeField]
        InputField inputField;

        Predicate<string> validator;
        Action<string> invalidCallback;

        public string PlaceholderText
        {
            set { placeholderText.text = value; }
        }

        protected override void Awake()
        {
            base.Awake();

            if (placeholderText == null)
                throw new UserDialogException("User dialogs's reference to placeholder text is not set!");
            if (inputField == null)
                throw new UserDialogException("User dialogs's reference to input field is not set!");

            OKButton.onClick.RemoveAllListeners();
            OKButton.onClick.AddListener(Submit);

            inputField.onEndEdit.AddListener(text =>
            {
                if (Input.GetKeyDown(KeyCode.Return))
                    Submit();
            });

            // Set default validator and invalid callback, can be overwritten later
            validator = text => !string.IsNullOrWhiteSpace(text);
            invalidCallback = _ => DialogBasic.Show("Input may not be empty!");
        }

        void Submit()
        {
            if (!validator(inputField.text))
            {
                invalidCallback(inputField.text);
                return;
            }

            OnSubmit?.Invoke(inputField.text);
            Close();
        }

        public void DisableCancelling() => CancelButton.gameObject.SetActive(false);
        public void EnableCancelling() => CancelButton.gameObject.SetActive(true);
        public void SetContentType(InputField.ContentType type) => inputField.contentType = type;

        /// <summary>
        /// Sets a function for validating the contents of the input field. This is checked
        /// on submit and if the contents are invalid the dialog stays open.
        /// </summary>
        public void SetValidator(Predicate<string> validator)
        {
            this.validator = validator;
            invalidCallback = _ => DialogBasic.Show("Input is invalid!");
        }

        /// <summary>
        /// Sets a function for validating the contents of the input field. This is checked
        /// on submit and if the contents are invalid the dialog stays open.
        /// </summary>
        /// <param name="invalidCallback">A function to be called if the user tries to submit an invalid input.</param>
        public void SetValidator(Predicate<string> validator, Action<string> invalidCallback)
        {
            SetValidator(validator);
            this.invalidCallback = invalidCallback;
        }

        #region Show

        #region Normal
        // Explicitly hide base class shows since they have different arguments
        private static new void Show(string message) { }
        private static new void ShowBlocking(string message) { }

        /// <summary>
        /// Shows a prompt for the user to enter a string. By default, the user's input may not be empty.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, bool cancellable = true)
        {
            var t = new TaskCompletionSource<StringInputResult>();

            DialogStringInput dialog;
            if (cancellable)
                dialog = DialogController.Show(message, SetOkResult, () => new StringInputResult());
            else
                dialog = DialogController.Show(message, SetOkResult);

            return (dialog, t.Task);

            void SetOkResult(string result) => t.TrySetResult(new StringInputResult(result));
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. Only allows the user to submit if the validator returns true.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator, bool cancellable = true)
        {
            var dialogResult = Show(message, cancellable);
            dialogResult.dialog.SetValidator(contentValidator);
            return dialogResult;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. If the input is invalid invalidCallback is called with the input.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator, Action<string> invalidCallback, bool cancellable = true)
        {
            var dialogResult = Show(message, cancellable);
            dialogResult.dialog.SetValidator(contentValidator, invalidCallback);
            return dialogResult;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. If the input is invalid a custom message is shown.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator, string invalidMessage, bool cancellable = true) =>
            Show(message, contentValidator, _ => DialogBasic.Show(invalidMessage), cancellable);

        /// <summary>
        /// Shows a prompt for the user to enter a string.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, cancellable);
            dialogResult.dialog.SetContentType(contentType);
            return dialogResult;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. Only allows the user to submit if the validator returns true.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator,
            InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, cancellable);
            dialogResult.dialog.SetContentType(contentType);
            return dialogResult;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. If the input is invalid invalidCallback is called with the input.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator,
            Action<string> invalidCallback, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidCallback, cancellable);
            dialogResult.dialog.SetContentType(contentType);
            return dialogResult;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with a custom content validator. If the input is invalid a custom message is shown.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) Show(string message, Predicate<string> contentValidator,
            string invalidMessage, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidMessage, cancellable);
            dialogResult.dialog.SetContentType(contentType);
            return dialogResult;
        }
        #endregion // Show | Normal

        #region Blocking
        /// <summary>
        /// Shows a blocking prompt for the user to enter a string. By default, the user's input may not be empty.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, bool cancellable = true)
        {
            var dialogResult = Show(message, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. Only allows the user to submit if the validator returns true.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. If the input is invalid invalidCallback is called with the input.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator,
            Action<string> invalidCallback, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidCallback, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. If the input is invalid a custom message is shown.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator,
            string invalidMessage, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidMessage, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentType, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. Only allows the user to submit if the validator returns true.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator,
            InputField.ContentType contentType, bool cancellable = true)
        { 
            var dialogResult = Show(message, contentValidator, contentType, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. If the input is invalid invalidCallback is called with the input.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator,
            Action<string> invalidCallback, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidCallback, contentType, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }

        /// <summary>
        /// Shows a blocking prompt for the user to enter a string with a custom content validator. If the input is invalid a custom message is shown.
        /// </summary>
        public static (DialogStringInput dialog, Task<StringInputResult> result) ShowBlocking(string message, Predicate<string> contentValidator,
            string invalidMessage, InputField.ContentType contentType, bool cancellable = true)
        {
            var dialogResult = Show(message, contentValidator, invalidMessage, contentType, cancellable);
            DialogController.Block(dialogResult.dialog);
            return dialogResult;
        }
        #endregion // Show | Blocking

        #endregion // Show
    }

    public class StringInputResult : DialogResult
    {
        public string Value { get; }

        public StringInputResult() : base(Cancelled)
        { }

        public StringInputResult(string value) : base(OK) => Value = value;
    }
}

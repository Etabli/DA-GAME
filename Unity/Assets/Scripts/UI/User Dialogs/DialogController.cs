using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public class DialogController : MonoBehaviour
    {
        [Serializable]
        public struct NamedPrefab
        {
            public string name;
            public GameObject prefab;
        }

        [SerializeField]
        Image inputBlocker;
        [SerializeField]
        NamedPrefab[] prefabs;

        static Dictionary<string, GameObject> dialogPrefabs = new Dictionary<string, GameObject>();
        static List<Dialog> activeDialogs = new List<Dialog>();
        static HashSet<Dialog> blockingDialogs = new HashSet<Dialog>();

        public static DialogController Instance { get; private set; }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            foreach (var prefab in prefabs)
            {
                dialogPrefabs.Add(prefab.name, prefab.prefab);
            }

            // Should always be last so input blocking works
            transform.SetAsLastSibling();
        }

        #region Show

        #region Basic

        #region Normal
        /// <summary>
        /// Shows a dialog with the specified message.
        /// </summary>
        public static DialogBasic Show(string message)
        {
            if (message == null)
                message = "";

            return CreateDialog<DialogBasic>(message);
        }

        /// <summary>
        /// Shows a dialog with the specified message and calls the callback when it is closed
        /// </summary>
        public static DialogBasic Show(string message, Action callback)
        {
            if (callback == null)
                throw new ArgumentException("Close callback can't be null!", nameof(callback));

            var dialog = CreateDialog<DialogBasic>(message);
            dialog.OnClose += callback;
            return dialog;
        }
        #endregion

        #region Blocking
        /// <summary>
        /// Shows a dialog with the specified message.
        /// </summary>
        public static DialogBasic ShowBlocking(string message)
        {
            var dialog = Show(message);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a dialog with the specified message and calls the callback when it is closed
        /// </summary>
        public static DialogBasic ShowBlocking(string message, Action callback)
        {
            var dialog = Show(message, callback);
            Block(dialog);
            return dialog;
        }
        #endregion

        #endregion

        #region Cancellable

        #region Normal
        /// <summary>
        /// Shows a dialog with the specified message and sets up the appropriate callbacks
        /// </summary>
        /// <param name="acceptCallback">The function to be called when the user hits OK</param>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel.
        /// Leave null if nothing should happen.</param>
        /// <returns></returns>
        public static DialogCancellable Show(string message, Action acceptCallback, Action cancelCallback)
        {
            if (acceptCallback == null)
                throw new ArgumentException("Accept callback can't be null!", nameof(acceptCallback));

            var dialog = CreateDialog<DialogCancellable>(message);
            dialog.OnAccept += acceptCallback;

            if (cancelCallback != null)
                dialog.OnCancel += cancelCallback;

            return dialog;
        }
        #endregion

        #region Blocking
        /// <summary>
        /// Shows a dialog with the specified message and sets up the appropriate callbacks
        /// </summary>
        /// <param name="acceptCallback">The function to be called when the user hits OK</param>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel.
        /// Leave null if nothing should happen.</param>
        /// <returns></returns>
        public static DialogCancellable ShowBlocking(string message, Action acceptCallback, Action cancelCallback)
        {
            var dialog = Show(message, acceptCallback, cancelCallback);
            Block(dialog);
            return dialog;
        }
        #endregion

        #endregion

        #region String Input

        #region No Cancel

        #region Normal
        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        public static DialogStringInput Show(string message, Action<string> submitCallback)
        {
            if (submitCallback == null)
                throw new ArgumentException("Submit callback can't be null!", nameof(submitCallback));

            var dialog = CreateDialog<DialogStringInput>(message);
            dialog.OnSubmit += submitCallback;
            dialog.DisableCancelling();

            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Predicate<string> contentValidator)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));

            var dialog = Show(message, submitCallback);
            dialog.SetValidator(contentValidator);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Predicate<string> contentValidator, Action<string> invalidCallback)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));
            if (invalidCallback == null)
                throw new ArgumentException("InvalidCallback can't be null!", nameof(invalidCallback));

            var dialog = Show(message, submitCallback);
            dialog.SetValidator(contentValidator, invalidCallback);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        public static DialogStringInput Show(string message, Action<string> submitCallback, InputField.ContentType contentType)
        {
            if (!Enum.IsDefined(typeof(InputField.ContentType), contentType))
                throw new ArgumentException("Contenty type argument has to be a valid InputField.ContentType!", nameof(contentType));

            var dialog = Show(message, submitCallback) as DialogStringInput;
            dialog.SetContentType(contentType);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback,
            Predicate<string> contentValidator, InputField.ContentType contentType)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));

            var dialog = Show(message, submitCallback, contentType);
            dialog.SetValidator(contentValidator);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback, InputField.ContentType contentType)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));
            if (invalidCallback == null)
                throw new ArgumentException("InvalidCallback can't be null!", nameof(invalidCallback));

            var dialog = Show(message, submitCallback, contentType);
            dialog.SetValidator(contentValidator, invalidCallback);
            return dialog;
        }
        #endregion

        #region Blocking
        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback)
        {
            var dialog = Show(message, submitCallback);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Predicate<string> contentValidator)
        {
            var dialog = Show(message, submitCallback, contentValidator);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Predicate<string> contentValidator,
            Action<string> invalidCallback)
        {
            var dialog = Show(message, submitCallback, contentValidator, invalidCallback);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, contentType);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback,
            Predicate<string> contentValidator, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, contentValidator, contentType);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with no option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, contentValidator, invalidCallback, contentType);
            Block(dialog);
            return dialog;
        }
        #endregion

        #endregion

        #region With Cancel

        #region Normal
        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel. Leave null if nothing should happen.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback)
        {
            if (submitCallback == null)
                throw new ArgumentException("Submit callback can't be null!", nameof(submitCallback));

            var dialog = CreateDialog<DialogStringInput>(message);
            dialog.OnSubmit += submitCallback;

            if (cancelCallback != null)
                dialog.OnCancel += cancelCallback;

            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));

            var dialog = Show(message, submitCallback, cancelCallback);
            dialog.SetValidator(contentValidator);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));
            if (invalidCallback == null)
                throw new ArgumentException("InvalidCallback can't be null!", nameof(invalidCallback));

            var dialog = Show(message, submitCallback, cancelCallback);
            dialog.SetValidator(contentValidator, invalidCallback);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string.
        /// </summary>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel. Leave null if nothing should happen.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback,
            InputField.ContentType contentType)
        {
            if (!Enum.IsDefined(typeof(InputField.ContentType), contentType))
                throw new ArgumentException("Contenty type argument has to be a valid InputField.ContentType!", nameof(contentType));

            var dialog = Show(message, submitCallback, cancelCallback);
            dialog.SetContentType(contentType);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, InputField.ContentType contentType)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));

            var dialog = Show(message, submitCallback, cancelCallback, contentType);
            dialog.SetValidator(contentValidator);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput Show(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback, InputField.ContentType contentType)
        {
            if (contentValidator == null)
                throw new ArgumentException("Content validator can't be null!", nameof(contentValidator));
            if (invalidCallback == null)
                throw new ArgumentException("InvalidCallback can't be null!", nameof(invalidCallback));

            var dialog = Show(message, submitCallback, cancelCallback, contentType);
            dialog.SetValidator(contentValidator, invalidCallback);
            return dialog;
        }
        #endregion

        #region Blocking
        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel. Leave null if nothing should happen.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback)
        {
            var dialog = Show(message, submitCallback, cancelCallback);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator)
        {
            var dialog = Show(message, submitCallback, cancelCallback, contentValidator);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback)
        {
            var dialog = Show(message, submitCallback, cancelCallback, contentValidator, invalidCallback);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string.
        /// </summary>
        /// <param name="cancelCallback">The function to be called when the user hits Cancel. Leave null if nothing should happen.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, cancelCallback, contentType);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, cancelCallback, contentValidator, contentType);
            Block(dialog);
            return dialog;
        }

        /// <summary>
        /// Shows a prompt for the user to enter a string with an option to cancel.
        /// </summary>
        /// <param name="contentValidator">A condition that has to be true before the user is allowed to submit.</param>
        /// <param name="invalidCallback">The function that should be called when the user attempts to submit something invalid.</param>
        public static DialogStringInput ShowBlocking(string message, Action<string> submitCallback, Action cancelCallback,
            Predicate<string> contentValidator, Action<string> invalidCallback, InputField.ContentType contentType)
        {
            var dialog = Show(message, submitCallback, cancelCallback, contentValidator, invalidCallback, contentType);
            Block(dialog);
            return dialog;
        }
        #endregion

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Makes a dialog block input to other UI elements (excluding other dialogs)
        /// </summary>
        public static void Block(Dialog dialog)
        {
            blockingDialogs.Add(dialog);
            Instance.inputBlocker.enabled = true;
            dialog.OnClose += () => UnBlock(dialog);
        }

        static T CreateDialog<T>(string message) where T : Dialog
        {
            var dialogGO = Instantiate(dialogPrefabs[typeof(T).Name], Instance.transform).GetComponent<Dialog>();
            dialogGO.transform.Translate(new Vector3(10, -10, 0) * activeDialogs.Count);
            (dialogGO.transform as RectTransform).ClampToScreen();

            var dialog = dialogGO.GetComponent<Dialog>() as T;
            dialog.Message = message;

            activeDialogs.Add(dialog);
            dialog.OnClose += () => activeDialogs.Remove(dialog);

            return dialog;
        }

        static void UnBlock(Dialog dialog)
        {
            blockingDialogs.Remove(dialog);
            if (blockingDialogs.Count == 0)
            {
                Instance.inputBlocker.enabled = false;
            }
        }
    }
}

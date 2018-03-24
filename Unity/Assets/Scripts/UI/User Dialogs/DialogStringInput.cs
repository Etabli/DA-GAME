using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        void Submit()
        {
            if (validator != null)
            {
                if (!validator(inputField.text))
                {
                    invalidCallback?.Invoke(inputField.text);
                    return;
                }
            }
            else if (inputField.text == "") // If no validator, default is no empty input
                return;

            OnSubmit?.Invoke(inputField.text);
            Close();
        }

        public void DisableCancelling()
        {
            CancelButton.gameObject.SetActive(false);
        }

        public void EnableCancelling()
        {
            CancelButton.gameObject.SetActive(true);
        }

        public void SetContentType(InputField.ContentType type)
        {
            inputField.contentType = type;
        }

        /// <summary>
        /// Sets a function for validating the contents of the input field. This is checked
        /// on submit and if the contents are invalid the dialog stays open.
        /// </summary>
        public void SetValidator(Predicate<string> validator)
        {
            this.validator = validator;
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
    }
}

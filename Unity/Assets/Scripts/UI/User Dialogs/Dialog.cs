using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public abstract class Dialog : MonoBehaviour
    {
        /// <summary>
        /// Called when the dialog is closed, whatever the reason
        /// </summary>
        public event Action OnClose;

        [SerializeField]
        protected Text messageText;

        public string Message
        {
            get { return messageText.text; }
            set { messageText.text = value; }
        }

        protected virtual void Awake()
        {
            if (messageText == null)
                throw new UserDialogException("User Dialog's reference to text object not set!");
        }

        /// <summary>
        /// Closes the dialog and destroys the gameobject
        /// </summary>
        public void Close()
        {
            OnClose?.Invoke();
            Destroy(gameObject);
        }

        public class UserDialogException : Exception
        {
            public UserDialogException(string message)
                : base(message)
            { }

            public UserDialogException(string message, Exception innerException)
                : base(message, innerException)
            { }
        }
    }

    public class DialogResult
    {
        protected uint ID;

        protected DialogResult(uint id) => ID = id;
        protected DialogResult(DialogResult src) => ID = src.ID;

        public bool IsOK => this == OK;
        public bool IsCancelled => this == Cancelled;

        public static bool operator ==(DialogResult lhs, DialogResult rhs) => lhs.ID == rhs.ID;
        public static bool operator !=(DialogResult lhs, DialogResult rhs) => !(lhs == rhs);

        public override bool Equals(object obj)
        {
            if (obj is DialogResult dialogResult)
                return this == dialogResult;
            return base.Equals(obj);
        }

        public override int GetHashCode() => ID.GetHashCode();

        public static readonly DialogResult OK = new DialogResult(0);
        public static readonly DialogResult Cancelled = new DialogResult(1);
    }
}

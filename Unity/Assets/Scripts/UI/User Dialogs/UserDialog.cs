using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UserDialog : MonoBehaviour
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

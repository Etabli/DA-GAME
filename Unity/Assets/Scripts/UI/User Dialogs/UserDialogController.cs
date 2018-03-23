using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDialogController : MonoBehaviour
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
    static List<UserDialog> activeDialogs = new List<UserDialog>();
    static HashSet<UserDialog> blockingDialogs = new HashSet<UserDialog>();

    public static UserDialogController Instance { get; private set; }

	void Awake ()
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

    /// <summary>
    /// Shows a dialog with the specified message.
    /// </summary>
    public static UserDialog Show(string message)
    {
        if (message == null)
            message = "";

        return CreateDialog<UserDialogBasic>(message);
    }

    /// <summary>
    /// Shows a dialog with the specified message and calls the callback when it is closed
    /// </summary>
    public static UserDialog Show(string message, Action callback)
    {
        if (callback == null)
            throw new ArgumentException("Close callback can't be null!", nameof(callback));

        var dialog = CreateDialog<UserDialogBasic>(message);
        dialog.OnClose += callback;
        return dialog;
    }

    /// <summary>
    /// Shows a dialog with the specified message and sets up the appropriate callbacks
    /// </summary>
    /// <param name="acceptCallback">The function to be called when the user hits OK</param>
    /// <param name="cancelCallback">The function to be called when the user hits Cancel.
    /// Leave null if nothing should happen.</param>
    /// <returns></returns>
    public static UserDialog Show(string message, Action acceptCallback, Action cancelCallback)
    {
        if (acceptCallback == null)
            throw new ArgumentException("Accept callback can't be null!", nameof(acceptCallback));

        var dialog = CreateDialog<UserDialogCancellable>(message);
        dialog.OnAccept += acceptCallback;

        if (cancelCallback != null)
            dialog.OnCancel += cancelCallback;

        return dialog;
    }

    /// <summary>
    /// Shows a dialog with the specified message.
    /// </summary>
    public static UserDialog ShowBlocking(string message)
    {
        var dialog = Show(message);
        Block(dialog);
        return dialog;
    }

    /// <summary>
    /// Shows a dialog with the specified message and calls the callback when it is closed
    /// </summary>
    public static UserDialog ShowBlocking(string message, Action callback)
    {
        var dialog = Show(message, callback);
        Block(dialog);
        return dialog;
    }

    /// <summary>
    /// Shows a dialog with the specified message and sets up the appropriate callbacks
    /// </summary>
    /// <param name="acceptCallback">The function to be called when the user hits OK</param>
    /// <param name="cancelCallback">The function to be called when the user hits Cancel.
    /// Leave null if nothing should happen.</param>
    /// <returns></returns>
    public static UserDialog ShowBlocking(string message, Action acceptCallback, Action cancelCallback)
    {
        var dialog = Show(message, acceptCallback, cancelCallback);
        Block(dialog);
        return dialog;
    }

    /// <summary>
    /// Makes a dialog block input to other UI elements (excluding other dialogs)
    /// </summary>
    public static void Block(UserDialog dialog)
    {
        blockingDialogs.Add(dialog);
        Instance.inputBlocker.enabled = true;
        dialog.OnClose += () => UnBlock(dialog);
    }

    static T CreateDialog<T>(string message) where T : UserDialog
    {
        var dialogGO = Instantiate(dialogPrefabs[typeof(T).Name], Instance.transform).GetComponent<UserDialog>();
        dialogGO.transform.Translate(new Vector3(10, -10, 0) * activeDialogs.Count);
        (dialogGO.transform as RectTransform).ClampToScreen();

        var dialog = dialogGO.GetComponent<UserDialog>() as T;
        dialog.Message = message;

        activeDialogs.Add(dialog);
        dialog.OnClose += () => activeDialogs.Remove(dialog);

        return dialog;
    }

    static void UnBlock(UserDialog dialog)
    {
        blockingDialogs.Remove(dialog);
        if (blockingDialogs.Count == 0)
        {
            Instance.inputBlocker.enabled = false;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDialogController : MonoBehaviour
{
    [SerializeField]
    List<GameObject> dialogPrefabs;
    [SerializeField]
    Image inputBlocker;

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

        // Should always be last so input blocking works
        transform.SetAsLastSibling();
	}

    /// <summary>
    /// Shows a dialog with the specified message.
    /// </summary>
    public static UserDialog Show(string message)
    {
        var dialogGO = Instantiate(Instance.dialogPrefabs[0], Instance.transform);
        var dialog = dialogGO.GetComponent<UserDialog>();
        dialogGO.transform.Translate(new Vector3(10, -10, 0) * activeDialogs.Count);
        (dialogGO.transform as RectTransform).ClampToScreen();

        activeDialogs.Add(dialog);
        dialog.OnClose += () => activeDialogs.Remove(dialog);

        dialog.Message = message;
        return dialog;
    }

    /// <summary>
    /// Shows a dialog with the specified message and calls the callback when it is closed
    /// </summary>
    public static UserDialog Show(string message, Action callback)
    {
        var dialog = Show(message);
        dialog.OnClose += callback;
        return dialog;
    }

    public static UserDialog ShowBlocking(string message)
    {
        var dialog = Show(message);
        Block(dialog);
        return dialog;
    }

    public static UserDialog ShowBlocking(string message, Action callback)
    {
        var dialog = Show(message, callback);
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

    static void UnBlock(UserDialog dialog)
    {
        blockingDialogs.Remove(dialog);
        if (blockingDialogs.Count == 0)
        {
            Instance.inputBlocker.enabled = false;
        }
    }
}

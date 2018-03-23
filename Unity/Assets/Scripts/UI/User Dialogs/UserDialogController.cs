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

    static int blockCounter = 0;

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

    /// <summary>
    /// Makes a dialog block input to other UI elements (excluding other dialogs)
    /// </summary>
    public static void Block(UserDialog dialog)
    {
        blockCounter++;
        Instance.inputBlocker.enabled = true;
        dialog.OnClose += UnBlock;
    }

    static void UnBlock()
    {
        if (--blockCounter <= 0)
        {
            blockCounter = 0;
            Instance.inputBlocker.enabled = false;
        }
    }
}

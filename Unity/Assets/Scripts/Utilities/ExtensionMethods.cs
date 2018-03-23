using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Moves this rect transform back into screen bounds
    /// </summary>
    /// <param name="transform"></param>
    public static void ClampToScreen(this RectTransform transform)
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float left_offset = transform.anchorMin.x * transform.sizeDelta.x;
        float right_offset = (1 - transform.anchorMin.x) * transform.sizeDelta.x;
        float bottom_offset = transform.anchorMin.y * transform.sizeDelta.y;
        float top_offset = (1 - transform.anchorMin.y) * transform.sizeDelta.y;

        if (x - left_offset < 0)
            x = left_offset;
        else if (x + right_offset > Screen.width)
            x = Screen.width - right_offset;

        if (y - bottom_offset < 0)
            y = bottom_offset;
        else if (y + top_offset > Screen.height)
            y = Screen.height - top_offset;

        transform.position = new Vector3(x, y);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Converter {

    public static Vector3 V2ToV3(Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector2 V3ToV2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

    public static Vector3[] V2ArrayToV3Array(Vector2[] points)
    {
        Vector3[] vertices = new Vector3[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            vertices[i] = V2ToV3(points[i]);
        }

        return vertices;
    }
}

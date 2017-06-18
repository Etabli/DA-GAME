using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static  class HexCellMeshGenerator {

    /// <summary>
    /// Generate a Mesh for a given HexCell
    /// </summary>
    /// <param name="cell">The Cell</param>
    /// <param name="repeat">Specifies if texture is repeat or clamping, repeat when true, otherwise clamp</param>
    /// <returns>The mesh for a hex cell</returns>
    public static Mesh GenerateHexCellMesh(HexCell cell,bool repeat = true)
    {
        Mesh hexMesh = new Mesh();
        Vector2[] corners = cell.GetAllCorners();
        Vector2 center = Map.HexCellToWorldPosition(cell);
        for (int i = 0; i < 6; i++)
        {
            corners[i] = corners[i] - center;
        }
        hexMesh.vertices = Converter.V2ArrayToV3Array(corners);
        hexMesh.triangles = HexTriangles();

        if(repeat)
        {
            hexMesh.uv = corners;
        }
        else
        {
            hexMesh.uv = GetUvs();
        }
        return hexMesh;
    }


    /// <summary>
    /// returns an int array with the triangels uesed
    /// ordering: counter clockwise
    /// </summary>
    /// <returns></returns>
    static int[] HexTriangles()
    {
        // CLOCKWISE winding order
        //return new int[] { 0, 1, 3,
        //                   1, 2, 3,
        //                   0, 3, 4,
        //                   0, 4, 5 };

        //counter clockwise
        return new int[]{ 0, 3, 1,
                          1, 3, 2,
                          0, 4, 3,
                          0, 5, 4};
    }

    static Vector2[] GetUvs()
    {
        Vector2[] uvs = new Vector2[6];

        uvs[0] = new Vector2(1.0f,0.25f); // right below center point
        uvs[1] = new Vector2(0.5f,0.0f); // below center point
        uvs[2] = new Vector2(0.0f,0.25f);//left below center point
        uvs[3] = new Vector2(0.0f,0.75f);//left above center point
        uvs[4] = new Vector2(0.5f,1.0f);// above center point
        uvs[5] = new Vector2(1.0f,0.75f);// right above center point
        return uvs;
    }


}

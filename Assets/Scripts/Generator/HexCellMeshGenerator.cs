using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static  class HexCellMeshGenerator {

    /// <summary>
    /// Generate a Mesh for a given HexCell
    /// </summary>
    /// <param name="cell">The Cell</param>
    /// <returns>The mesh for a hex cell</returns>
    public static Mesh GenerateHexCellMesh(HexCell cell)
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

        hexMesh.uv =  corners;
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class WorldController : MonoBehaviour {

    #region Member Variables

    Map map = new Map(20);




    public static WorldController Instance { get; protected set; }

    #endregion


    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;
	}
	
	// Update is called once per frame
	void Update () {

        Vector2 mouse = V3TOV2(Input.mousePosition);
        //Debug.Log(mouse);


        HexCell cell = map.WorldPositionToHexCell(mouse);


        if (cell != null)
        {
            Debug.Log(cell.Coords.ToString());
        }

    }

    private void OnDrawGizmos()
    {
        if( map != null)
        {

            Gizmos.color = Color.black;
            HexCell center = map[0, 0];

            Gizmos.DrawSphere(V2TOV3(Map.HexCellToWorldPosition(center)),1);

            DrawHexagonInEditor(center, Color.black);

            Neighbors neigh = map.GetNeighbors(center);

            foreach (HexCell cell in neigh.GetNeighbors())
            {
                Gizmos.DrawSphere(V2TOV3(Map.HexCellToWorldPosition(cell)), 1);
                DrawHexagonInEditor(cell, Color.blue);
            }
        }
    }


    void DrawHexagonInEditor(HexCell cell, Color color)
    {
        Gizmos.color = color;
        Vector2[] corners = cell.GetAllCorners();

        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawSphere(V2TOV3(corners[i]), 1);
            if (i == 5)
            {
                Gizmos.DrawLine(V2TOV3(corners[i]), corners[0]);
                break;
            }

            Gizmos.DrawLine(V2TOV3(corners[i]), corners[i + 1]);

        }
    }

     public static Vector3 V2TOV3 (Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

    public static Vector2 V3TOV2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }

}

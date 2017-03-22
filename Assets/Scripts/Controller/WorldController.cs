using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

    #region Member Variables

    Map map = new Map(1);

    #endregion



    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        if( map != null)
        {

            Gizmos.color = Color.black;
            HexCell center = map[0, 0];

            DrawHexagonInEditor(center, Color.black);

            Neighbors neigh = map.GetNeighbors(center);

            foreach (HexCell cell in neigh.GetNeighbors())
            {
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
            if (i == 5)
            {
                Gizmos.DrawLine(V2TOV3(corners[i]), corners[0]);
                break;
            }

            Gizmos.DrawLine(V2TOV3(corners[i]), corners[i + 1]);

        }
    }

    Vector3 V2TOV3 (Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }

}

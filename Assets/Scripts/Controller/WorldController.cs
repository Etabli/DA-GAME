using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;


public class WorldController : MonoBehaviour {

    #region Member Variables

    public Map map;// = new Map(20);

    public Material testMat;
    public Material markMaterial;

    Area hoveroverArea;
    Area prevHoverOver;

    public static WorldController Instance { get; protected set; }

    Dictionary<HexCell, GameObject> hexCellToGameObjectDictionary;


    #endregion


    public void OnHexCellCreated(HexCell cell)
    {
        GameObject hexGO = new GameObject();

        //set parent to mabye not clogg hirachy view
        //set name for identification
        //set center to hex cell Center

        hexGO.name = "HexCell : " + cell.Coords.ToString();
        hexGO.transform.position = Map.HexCellToWorldPosition(cell);
        hexGO.transform.SetParent(transform, true);

        MeshFilter meshFilter = hexGO.AddComponent<MeshFilter>();
        meshFilter.mesh = HexCellMeshGenerator.GenerateHexCellMesh(cell);
        MeshRenderer renderer = hexGO.AddComponent<MeshRenderer>();
        renderer.material = testMat;


        if(hexCellToGameObjectDictionary.ContainsKey(cell))
        {
            Debug.LogError("WorldController::OnHexCellCreated - There already exists an entry for this hex cell in the <HexCell,GameObject> Dictionary");
            return;
        }
        hexCellToGameObjectDictionary.Add(cell, hexGO);

    }


    void OnEnable()
    {
        if (Instance != null)
        {
            Debug.LogError("There should never be two world controllers.");
        }
        Instance = this;
	}


    void Start()
    {
        hexCellToGameObjectDictionary = new Dictionary<HexCell, GameObject>();
        hoveroverArea = null;
        prevHoverOver = null;
        map = new Map(20);
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // Update is called once per frame
    void Update()
    {

        Vector2 mouse = Converter.V3ToV2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        ////Debug.Log(mouse);



        if (map != null)
        {
            HexCell cell = map.WorldPositionToHexCell(mouse);
            if (cell != null)
            { 
                if (hoveroverArea != null)
                {
                    foreach (HexCell c in hoveroverArea.Cells)
                    {
                        UpdateCellTexture(c);
                    }
                    prevHoverOver = hoveroverArea;
                    hoveroverArea = cell.ParentArea;
                    if (prevHoverOver != hoveroverArea)
                    {
                        Debug.Log("Area Size: " + hoveroverArea.PossibleAreaSize);
                    }
                    foreach (HexCell c in hoveroverArea.Cells)
                    {
                        if (hexCellToGameObjectDictionary.ContainsKey(c))
                        {
                            hexCellToGameObjectDictionary[c].GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                    }
                }
                else
                {
                    hoveroverArea = cell.ParentArea;
                }
            }

        }
        else
        {
           // Debug.Log("MAP IS NULL WHAT IS GOING ON");
        }
        //if (cell != null && hexCellToGameObjectDictionary.ContainsKey(cell))
        //{
        //    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = markMaterial;
        //}



        //ColorRing(1, Color.red);
        //ColorRing(2, Color.blue);
        //ColorRing(3, Color.black);
        //ColorRing(20, Color.cyan);

    }


    public void UpdateCellTexture(HexCell cell)
    {
        if(hexCellToGameObjectDictionary.ContainsKey(cell))
        {
            switch(cell.ParentArea.BiomeType)
            {
                case BiomeType.Ice:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.white;
                    break;
                case BiomeType.Grass:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.green;
                    break;
                case BiomeType.DickGrease:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.yellow;
                    break;
                case BiomeType.House:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.cyan;
                    break;
                case BiomeType.Swamp:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.red;
                    break;
                default:
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = Color.black;
                    break;
            }
        }
    }


    void ColorRing(int rad, Color color)
    {
        List<HexCell> ring = map.GetRing(rad);

        foreach (HexCell cell in ring)
        {
            hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    

    //private void OnDrawGizmos()
    //{
    //    if (map != null)
    //    {

    //        Gizmos.color = Color.black;
    //        HexCell center = map[0, 0];

    //        Gizmos.DrawSphere(Converter.V2ToV3(Map.HexCellToWorldPosition(center)), 1);

    //        DrawHexagonInEditor(center, Color.black);

    //        Neighbors neigh = map.GetNeighbors(center);

    //        foreach (HexCell cell in neigh.GetNeighbors())
    //        {
    //            Gizmos.DrawSphere(Converter.V2ToV3(Map.HexCellToWorldPosition(cell)), 1);
    //            DrawHexagonInEditor(cell, Color.blue);
    //        }
    //    }
    //}


    //void DrawHexagonInEditor(HexCell cell, Color color)
    //{
    //    Gizmos.color = color;
    //    Vector2[] corners = cell.GetAllCorners();

    //    for (int i = 0; i < 6; i++)
    //    {
    //        Gizmos.DrawSphere(Converter.V2ToV3(corners[i]), 1);
    //        if (i == 5)
    //        {
    //            Gizmos.DrawLine(Converter.V2ToV3(corners[i]), corners[0]);
    //            break;
    //        }

    //        Gizmos.DrawLine(Converter.V2ToV3(corners[i]), corners[i + 1]);

    //    }
    //}

    


}


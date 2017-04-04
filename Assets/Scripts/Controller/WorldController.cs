using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;


public class WorldController : MonoBehaviour {

    #region Member Variables

 
    [Range(1,5)]
    public int EnemyVariatyPerBiome;

    [Range(1, 20)]
    public int ItemVariatyPerBiome;

    [Range(1, 5)]
    public  int ResourceVariatyPerBiome;

    public Map map;// = new Map(20);

    public Material testMat;
    public Material markMaterial;

    Area hoveroverArea;
    Area prevHoverOver;

    public static WorldController Instance { get; protected set; }

    Dictionary<HexCell, GameObject> HexCellToGameObjectDictionary;
    Dictionary<BiomeType, Material> BiomeTypeToMaterialDictionary;

    #endregion




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
        HexCellToGameObjectDictionary = new Dictionary<HexCell, GameObject>();
        LoadBiomeInfo();
        hoveroverArea = null;
        prevHoverOver = null;
        map = new Map(5);
        if (Instance == null)
        {
            Instance = this;
        }
    }

    //bool temp_once = false;
    // Update is called once per frame
    void Update()
    {

        Vector2 mouse = Converter.V3ToV2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        ////Debug.Log(mouse);

        //if (!temp_once)
        //{
        //    temp_once = true;
        //    //temp_once = true;
        //    //BiomeInfo info = new BiomeInfo(BiomeType.DickGrease,
        //    //    new List<EnemyType>() { EnemyType.Bat, EnemyType.Marshmellow, EnemyType.Nutts, EnemyType.Snickers },
        //    //    new List<ItemType>() { ItemType.Hands, ItemType.NotHands, ItemType.Feet },
        //    //    new List<ResourceType>() { ResourceType.Asshair, ResourceType.Blood, ResourceType.Guts, ResourceType.Iron },
        //    //    new Range(1.0f, 20.0f));
        //    //Debug.Log(info.ToString());

        //    //BiomeInfoSerializer.SaveBiomeInfoToDisk(info);


        //    BiomeInfo info = BiomeInfoSerializer.LoadBiomeInfoFromDisk(BiomeType.DickGrease);


        //    Debug.Log(info.ToString());

        //}

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
                        Debug.Log("Area Capacity: " + hoveroverArea.Cells.Capacity + " Actual Size: " + hoveroverArea.Cells.Count + "\nBiome: " + hoveroverArea.AreaBiome.ToString());
                    }
                    foreach (HexCell c in hoveroverArea.Cells)
                    {
                        if (HexCellToGameObjectDictionary.ContainsKey(c))
                        {
                            HexCellToGameObjectDictionary[c].GetComponent<MeshRenderer>().material.color = Color.blue;
                        }
                    }
                }
                else
                {
                    hoveroverArea = cell.ParentArea;
                }
            }

            //}
            //else
            //{
            //    // Debug.Log("MAP IS NULL WHAT IS GOING ON");
            //}


        }
    }

    /// <summary>
    /// Loads everything about the biomes from  material to info
    /// </summary>
    void LoadBiomeInfo()
    {
        BiomeInfoSerializer.LoadAllBiomeInfosFromDisk();
        LoadBiomeMaterials();
    }

    /// <summary>
    /// Loads the biome materials
    /// </summary>
    void LoadBiomeMaterials()
    {
        BiomeTypeToMaterialDictionary = new Dictionary<BiomeType, Material>();

        Material[] materials = Resources.LoadAll<Material>("Materials/BiomeMaterials/");

        foreach (Material mat in materials)
        {
            BiomeType type = (BiomeType)Enum.Parse(typeof(BiomeType), mat.name);
            Debug.Log("WorldController::LoadBiomeMaterials - Creating entry for " + type);
            if(!BiomeTypeToMaterialDictionary.ContainsKey(type))
            {
                BiomeTypeToMaterialDictionary.Add(type, mat);
            }
        }
    }


    public void OnHexCellCreated(HexCell cell)
    {
        GameObject hexGO = new GameObject();

        //set parent to mabye not clogg hirachy view
        //set name for identification
        //set center to hex cell Center

        hexGO.name = "HexCell : " + cell.Coords.ToString();
        hexGO.transform.position = Map.HexCellToWorldPosition(cell);
        hexGO.transform.SetParent(transform, true);
        hexGO.layer = LayerMask.NameToLayer("Map");
        //set layer


        MeshFilter meshFilter = hexGO.AddComponent<MeshFilter>();
        meshFilter.mesh = HexCellMeshGenerator.GenerateHexCellMesh(cell);
        MeshRenderer renderer = hexGO.AddComponent<MeshRenderer>();
        renderer.material = default(Material);

        if (HexCellToGameObjectDictionary.ContainsKey(cell))
        {
            Debug.LogError("WorldController::OnHexCellCreated - There already exists an entry for this hex cell in the <HexCell,GameObject> Dictionary");
            return;
        }
        HexCellToGameObjectDictionary.Add(cell, hexGO);

    }

    /// <summary>
    /// updates the material of a hexcell depending on biome type
    /// </summary>
    /// <param name="cell"></param>
    public void UpdateCellTexture(HexCell cell)
    {
        if(HexCellToGameObjectDictionary.ContainsKey(cell))
        {
           if(BiomeTypeToMaterialDictionary.ContainsKey(cell.ParentArea.AreaBiome.Type))
           {
                HexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = BiomeTypeToMaterialDictionary[cell.ParentArea.AreaBiome.Type];
           }
           else
           {
                Debug.Log("WorldController::UpdateCellTexture - There is no material for this biome type " + cell.ParentArea.AreaBiome.Type);
           }
        }
        else
        {
            Debug.Log("WorldController::UpdateCellTexture - This HexCell does not exist in the HexCell dictionary");
        }
    }


    /// <summary>
    /// Colors a ring of certain radius in the given color
    /// </summary>
    /// <param name="rad"></param>
    /// <param name="color"></param>
    void ColorRing(int rad, Color color)
    {
        List<HexCell> ring = map.GetRing(rad);

        foreach (HexCell cell in ring)
        {
            HexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = color;
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

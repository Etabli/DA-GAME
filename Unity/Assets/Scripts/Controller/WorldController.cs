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

    [Range(1, 20)]
    public int radius;

    public Map map;// = new Map(20);

    public Material testMat;
    public Material markMaterial;
    public GameObject hexCellPrefab;

    public static WorldController Instance { get; protected set; }

    Dictionary<HexCell, GameObject> hexCellToGameObjectDictionary;
    Dictionary<BiomeType, Material> biomeTypeToMaterialDictionary;

    HexCell prevHoverOver;

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
        hexCellToGameObjectDictionary = new Dictionary<HexCell, GameObject>();
        LoadBiomeInfo();
        map = new Map(radius);
        UpdateCellTextures();
    }

    //bool temp_once = false;
    // Update is called once per frame
    void Update()
    {
        if (prevHoverOver != null)
        {
            // revert changes to previous frame if new hoverover
            UnmarkAreaHexCellIsIn(prevHoverOver);
        }
        Vector2 mouse = Converter.V3ToV2(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (map != null)
        {
            HexCell cell = map.WorldPositionToHexCell(mouse);
            if(cell != null)
            {
                // not over same cell as prev frame
                MarkAreaHexCellIsIn(cell);
                prevHoverOver = cell;
                // else do nothing
            }// end if cell null
        }//end if map null
    }

    /// <summary>
    /// TEMP
    /// </summary>
    /// <param name="cell"></param>
    void UnmarkAreaHexCellIsIn(HexCell cell)
    {
        if(cell.ParentArea != null)
        {
            foreach (HexCell c in cell.ParentArea.Cells)
            {
                UpdateCellTexture(c);
            }
        }
    }
    /// <summary>
    /// TEMP
    /// </summary>
    /// <param name="cell"></param>
    void MarkAreaHexCellIsIn(HexCell cell)
    {
        if(cell.ParentArea != null)
        {
            //Debug.Log("Area: " + cell.ParentArea.ToString());
            foreach(HexCell c in cell.ParentArea.Cells)
            {
                if (hexCellToGameObjectDictionary.ContainsKey(c))
                {
                    hexCellToGameObjectDictionary[c].GetComponent<MeshRenderer>().material = markMaterial;
                }
            }
        }
    }

    /// <summary>
    /// Loads everything about the biomes from  material to info
    /// </summary>
    void LoadBiomeInfo()
    {
        Serializer.LoadAllBiomeInfosFromDisk();
        LoadBiomeMaterials();
    }

    /// <summary>
    /// Loads the biome materials
    /// </summary>
    void LoadBiomeMaterials()
    {
        biomeTypeToMaterialDictionary = new Dictionary<BiomeType, Material>();

        Material[] materials = Resources.LoadAll<Material>("Materials/BiomeMaterials/");

        foreach (Material mat in materials)
        {
            BiomeType type = (BiomeType)Enum.Parse(typeof(BiomeType), mat.name);
            //Debug.Log("WorldController::LoadBiomeMaterials - Creating entry for " + type);
            if(!biomeTypeToMaterialDictionary.ContainsKey(type))
            {
                biomeTypeToMaterialDictionary.Add(type, mat);
            }
        }
    }

    public void OnHexCellCreated(HexCell cell)
    {
        //GameObject hexGO = Instantiate(hexCellPrefab);
        GameObject hexGO = new GameObject
        {
            name = "HexCell : " + cell.Coords.ToString()
        };
        //set parent to mabye not clogg hirachy view
        //set name for identification
        //set center to hex cell Center

        hexGO.transform.position = Map.HexCellToWorldPosition(cell);
        hexGO.transform.SetParent(transform, true);
        hexGO.layer = LayerMask.NameToLayer("Map");
        //set layer

        MeshFilter meshFilter = hexGO.AddComponent<MeshFilter>();
        meshFilter.mesh = HexCellMeshGenerator.GenerateHexCellMesh(cell);
        MeshRenderer renderer = hexGO.AddComponent<MeshRenderer>();
        renderer.material = default(Material);

        if (hexCellToGameObjectDictionary.ContainsKey(cell))
        {
            Debug.LogError("WorldController::OnHexCellCreated - There already exists an entry for this hex cell in the <HexCell,GameObject> Dictionary");
            return;
        }
        hexCellToGameObjectDictionary.Add(cell, hexGO);

    }

    /// <summary>
    /// updates the material of all
    /// </summary>
    /// <param name="cell"></param>
    public void UpdateCellTextures()
    {
        foreach(HexCell cell in map.WorldMap.Values)
        {
            if(hexCellToGameObjectDictionary.ContainsKey(cell))
            {
               if(cell.ParentArea.AreaBiome != null)
               {
                   if(biomeTypeToMaterialDictionary.ContainsKey(cell.ParentArea.AreaBiome.Type))
                   {
                        hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = biomeTypeToMaterialDictionary[cell.ParentArea.AreaBiome.Type];
                   }// end if biome material map contains key
                   else
                        Debug.Log("WorldController::UpdateCellTexture - There is no material for this biome type " + cell.ParentArea.AreaBiome.Type);
               }
               else
               {
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = default(Material);
               }// end parent area has no biome
            }// end if contains key
            else
                Debug.Log("WorldController::UpdateCellTexture - This HexCell does not exist in the HexCell dictionary");
        }// ned foreach
    }

    /// <summary>
    /// Updates texture for single cell
    /// </summary>
    public void UpdateCellTexture(HexCell cell)
    {
        if (hexCellToGameObjectDictionary.ContainsKey(cell))
        {
            if (cell.ParentArea.AreaBiome != null)
            {
                if (biomeTypeToMaterialDictionary.ContainsKey(cell.ParentArea.AreaBiome.Type))
                {
                    hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = biomeTypeToMaterialDictionary[cell.ParentArea.AreaBiome.Type];
                }// end if biome material map contains key
                else
                    Debug.Log("WorldController::UpdateCellTexture - There is no material for this biome type " + cell.ParentArea.AreaBiome.Type);
            }
            else
            {
                hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material = default(Material);
            }// end parent area has no biome
        }// end if contains key
        else
            Debug.Log("WorldController::UpdateCellTexture - This HexCell does not exist in the HexCell dictionary");

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
            hexCellToGameObjectDictionary[cell].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void OnDrawGizmos()
    {
        if (map != null)
        {
            if (map.AreaGraph != null)
            {
                DrawGraph(map.AreaGraph);
            }// end if ar graph not null
        }// end if map not null
    }

    /// <summary>
    /// TEMP
    /// </summary>
    /// <param name="g"></param>
    void DrawGraph(AreaGraph g)
    {
        foreach (AreaNode node in g.GetNodes())
        {
            Gizmos.color = biomeTypeToMaterialDictionary[node.GetColor()].color;
            HexCell centerCell = map.GetAreaByID(node.NodeID).Cells[0];
            Vector3 nodePos = Converter.V2ToV3(Map.HexCellToWorldPosition(centerCell));
            Gizmos.DrawSphere(nodePos, 1);

            Gizmos.color = Color.cyan;
            foreach (uint node2 in node.Edges)
            {
                HexCell  edgeNode =  map.GetAreaByID(node2).Cells[0];
                Vector3 nodePos2 = Converter.V2ToV3(Map.HexCellToWorldPosition(edgeNode));
                Gizmos.DrawLine(nodePos, nodePos2);
            }
        }// end i
    }

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

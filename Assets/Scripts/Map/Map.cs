using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Map
{
    #region Member variables
    
    public int Radius { get; protected set; }

    HexCell[,] WorldMap;

    int WorldMapLength;

    #endregion

    #region Ctor

    public Map(int radius)
    {
        Radius = radius;

        WorldMapLength = 2 * radius + 1;

        WorldMap = new HexCell[WorldMapLength, WorldMapLength];

        HexCell center = new HexCell(0, 0, null, null);

        //set center in world map
        this[center.Coords] = center;

        Queue<HexCell> outerRing = new Queue<HexCell>();
        outerRing.Enqueue(center);

        //for testing
        //there are no bases and areas assigned yet
        for (int i = 0; i < radius; i++)
        {
            //Debug.Log("Hi why am i in here");
            Queue<HexCell> oldOuterRing = new Queue<HexCell>(outerRing);
            outerRing.Clear();
            while (oldOuterRing.Count > 0)
            {
                HexCell currCenter = oldOuterRing.Dequeue();

                Coords[] neighborCoords = Coords.GetAllNeighborCoords(currCenter.Coords);

                for (int j = 0; j < 6; j++)
                {
                    //Debug.Log("X:" + neighborCoords[j].X + " Y: " + neighborCoords[j].Y);
                    if(this[neighborCoords[j]] == null)
                    {
                        HexCell neighbor = new HexCell(neighborCoords[j], null, null);
                        this[neighborCoords[j]] = neighbor;
                        outerRing.Enqueue(neighbor);
                    }
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Generates the map, and areas as well as the texture for the center cell and maybe it sourounding
    /// cells
    /// </summary>
    public void Generate()
    {
        Debug.LogError("Map::Generate - WARNING: NOT IMPLEMENTED");
    }


    public HexCell this[Coords coord]
    {
        get
        {
            return WorldMap[coord.Y + Radius, coord.X + Radius + Mathf.Min(0, coord.Y)];
        }
        set
        {
            WorldMap[coord.Y + Radius, coord.X + Radius + Mathf.Min(0, coord.Y)] = value;
        }

    }


    public HexCell this[int x, int y]
    {
        get
        {
            return WorldMap[y + Radius, x + Radius + Mathf.Min(0, y)];
        }
        set
        {
            WorldMap[y + Radius, x + Radius + Mathf.Min(0, y)] = value;
        }

    }



    /// <summary>
    /// Returns the manahten distance between two cells
    /// </summary>
    /// <param name="a">the first cell</param>
    /// <param name="b">the second cell</param>
    /// <returns>the distance between tem</returns>
    public int DistanceBetweenCells(HexCell a, HexCell b)
    {
        return Coords.Distance(a.Coords, b.Coords);
    }

    /// <summary>
    /// Returns all the neighbor cells of a given cell
    /// </summary>
    /// <param name="center">the center cell of which we want the neigbors</param>
    /// <returns>A neighbors struct containint an array with all the neighbors and the center</returns>
    public Neighbors GetNeighbors(HexCell center)
    {
        Coords[] neighborCoords = Coords.GetAllNeighborCoords(center.Coords);
        HexCell[] neighbors = new HexCell[6];

        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = this[neighborCoords[i]];
        }

        return new Neighbors(center, neighbors);

    }


    /// <summary>
    /// Returns the HexCell that contains this world position
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public HexCell WorldPositionToHexCell(Vector2 position)
    {
        //Debug.LogError("Map::WorldPositionToHexCell - WARNING NOT IMPLEMENTED RIGHT NOW");
        Vector2 hexCoordsFractional = new Vector2(position.x * (2.0f / 3.0f) / HexCell.HEXCELL_SIZE,
                                                  ((-position.x) / 3.0f + Mathf.Sqrt(3.0f) / 3.0f * position.y) / HexCell.HEXCELL_SIZE);

        Coords hexCoords = HexCell.RoundToNearestHexCell(hexCoordsFractional);

        return this[hexCoords];
    }


    /// <summary>
    /// Returns the world position coords of the cell center point
    /// </summary>
    /// <param name="cell">The cell in question</param>
    /// <returns>The WorldPosition of the center of the cell</returns>
    public static Vector2 HexCellToWorldPosition(HexCell cell)
    {
        return new Vector2(HexCell.HEXCELL_SIZE * Mathf.Sqrt(3) * ((float)cell.Coords.X + (float)cell.Coords.Y / 2f),
                            HexCell.HEXCELL_SIZE * 3.0f / 2.0f * cell.Coords.Y);
    }


}

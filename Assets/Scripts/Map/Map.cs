using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


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

        Generate();
        Debug.Log("Finished generating");
    }

    #endregion

    /// <summary>
    /// Generates the map, and areas as well as the texture for the center cell and maybe it sourounding
    /// cells
    /// </summary>
    void Generate()
    {
        System.Random rng = new System.Random();
        Debug.Log("Map::Generate - WARNING: NOT Finsihed IMPLEMENTING");
        HexCell center = new HexCell(0, 0,new Area(0,rng.Next(1,4),BiomeType.Ice));
        center.ParentArea.AddToCellList(center);

        //generate start area

        //set center in world map
        this[center.Coords] = center;

        for (int i = 1; i <= Radius; i++)
        {
            Coords coord = new Coords(-i, 0);

            //walk a long a ring with radius i
            for (int j = 0; j < 6; j++)
            {
                for (int k = 0; k < i; k++)
                {
                    this[coord] = new HexCell(coord);
                    HandleAreaForCell(this[coord]);
                    coord = Coords.GetNeighborCoords(coord, (HexDirection)j);
                }
            }

        }

    }

    void HandleAreaForCell(HexCell cell)
    {
        List<HexCell> existingNeighbors = GetNeighbors(cell).GetExistingNeighbors();

        foreach (HexCell neighbor in existingNeighbors)
        {
            if(neighbor.ParentArea != null)
            {
                if(!neighbor.ParentArea.AreaIsFull())
                {
                    Lottery<int> lottery = new Lottery<int>();

                    lottery.Enter(1, 55 + 10 * DistanceBetweenCells(cell,neighbor) + 2 * (neighbor.ParentArea.PossibleAreaSize - neighbor.ParentArea.Cells.Count));
                    lottery.Enter(2, 35 + 5 * (1 - DistanceBetweenCells(cell,neighbor)) - 2 * (neighbor.ParentArea.PossibleAreaSize - neighbor.ParentArea.Cells.Count));

                    if (lottery.GetWinner() == 1)
                    {
                        cell.ParentArea = neighbor.ParentArea;
                        cell.ParentArea.AddToCellList(cell);
                    }
                }
            }
        }
        
        if(cell.ParentArea == null)
        {
            Lottery<int> lottery = new Lottery<int>();

            lottery.Enter(1, 30);
            lottery.Enter(2, 50);
            lottery.Enter(3, 42);
            lottery.Enter(4, 17);


            cell.ParentArea = new Area(DistanceBetweenCells(cell, this[0, 0]), lottery.GetWinner() , Biome.GetRandomBiomeType());
            cell.ParentArea.AddToCellList(cell);
        }
    }



    List<HexCell> GetNeighborsWithoutArea(HexCell cell)
    {
        List<HexCell> neighors = new List<HexCell>();

        Neighbors neigh = GetNeighbors(cell);

        foreach(HexCell c in neigh.GetNeighbors())
        {
            if(c != null && c.ParentArea == null)
            {
                neighors.Add(c);
            }
        }
        return neighors;
    }

    /// <summary>
    /// Returns a ring of hexcell with a certain distance from the center
    /// </summary>
    /// <param name="radius">The radius of the ring</param>
    /// <returns>List of the HexCells in the ring</returns>
    public List<HexCell> GetRing(int radius)
    {

        List<HexCell> ring = new List<HexCell>();

        if(radius == 0)
        {
            ring.Add(this[0,0]);
            return ring;
        }


        HexCell cube = this[-radius, 0];

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                ring.Add(cube);
                cube = this[Coords.GetNeighborCoords(cube.Coords, (HexDirection)i)];
            }
        }
        return ring;
    }



    public int GetTotalNumberOfHexCellsInMap()
    {
        // (2*radius +1) ^ 2 - (radius * (radius +1)) 
        return 1 + (3 * (Radius * (Radius + 1)));
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
            if (CheckCoordsInRange(neighborCoords[i]))
            {
               neighbors[i] = this[neighborCoords[i]];
            }
        }

        return new Neighbors(center, neighbors);

    }

    /// <summary>
    /// Check if coord is outside of the map or not
    /// </summary>
    /// <param name="coord">the coords to check</param>
    /// <returns>Flase when outside of map, true when inside</returns>
    public bool CheckCoordsInRange(Coords coord)
    {
        return Coords.Distance(coord, new Coords(0, 0)) <= Radius;
    }

    /// <summary>
    /// Returns the HexCell that contains this world position
    /// </summary>
    /// <param name="position">The World position</param>
    /// <returns>The HexCell in which the  world position is contained. Null if position is outside of map</returns>
    public  HexCell WorldPositionToHexCell(Vector2 position)
    {
        //Debug.LogError("Map::WorldPositionToHexCell - WARNING NOT IMPLEMENTED RIGHT NOW");

        //float x = (position.x * Mathf.Sqrt(3) / 3 - position.y / 3) / HexCell.HEXCELL_SIZE;
        //float y = position.y * 2.0f / 3.0f / HexCell.HEXCELL_SIZE;

        Vector2 hexCoordsFractional = 
                new Vector2((position.x * Mathf.Sqrt(3) / 3 - position.y / 3) / HexCell.HEXCELL_SIZE,
                             position.y * 2.0f / 3.0f / HexCell.HEXCELL_SIZE);

        Coords hexCoords = HexCell.RoundToNearestHexCell(hexCoordsFractional);

        if (CheckCoordsInRange(hexCoords))
            return this[hexCoords];
        return null;
    }


    /// <summary>
    /// Returns the world position coords of the cell center point
    /// </summary>
    /// <param name="cell">The cell in question</param>
    /// <returns>The WorldPosition of the center of the cell</returns>
    public  static Vector2 HexCellToWorldPosition(HexCell cell)
    {
        return new Vector2(HexCell.HEXCELL_SIZE * Mathf.Sqrt(3) * ((float)cell.Coords.X + (float)cell.Coords.Y / 2f),
                            HexCell.HEXCELL_SIZE * (3.0f / 2.0f) * cell.Coords.Y);
    }


}

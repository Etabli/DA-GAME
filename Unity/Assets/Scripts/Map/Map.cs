using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Map
{
    #region Member variables
    
    public int Radius { get; protected set; }

    //HexCell[,] WorldMap;
    public Dictionary<Coords, HexCell> WorldMap { get; protected set; }
    public Dictionary<uint, Area> Areas { get; protected set; }

    //temporary
    public AreaGraph AreaGraph { get; protected set; }

    #endregion

    #region Ctor

    public Map(int radius)
    {
        Radius = radius;

        Areas = new Dictionary<uint, Area>();

        //WorldMap = new HexCell[WorldMapLength, WorldMapLength];
        WorldMap = new Dictionary<Coords, HexCell>();

        Generate();

        #region testing cell duplicate
        //foreach(Area a in Areas)
        //{
        //    foreach(HexCell c in a.Cells)
        //    {
        //        foreach (Area ar in Areas)
        //        {
        //            if (!a.Equals(ar))
        //            {
        //                foreach (HexCell cr in ar.Cells)
        //                {
        //                    //Debug.Log("Checking agians:");
        //                    //Debug.Log(i++ +"Area1: " +a);
        //                    //Debug.Log(j++ +"Area2:"+ ar);
        //                    //Debug.Log("Cell1: " + c);
        //                    //Debug.Log("Cell2: " + cr);
        //                    if (c.Equals(cr))
        //                    {
        //                        Debug.Log("Found Areas that contain the same cell " + c);
        //                        Debug.Log("Area 1: " + a);
        //                        Debug.Log("Area 2: " + ar);
        //                    }// end if
        //                }// end foreach hexcell II
        //            }// end if areas equal
        //        }// end foreach areas II
        //    }// end for each hex cell

        //}// end for each areas
#endregion
    }

    #endregion

    /// <summary>
    /// Generates the map, and areas as well as the texture for the center cell and maybe it sourounding
    /// cells
    /// </summary>
    void Generate()
    {
        Debug.LogWarning("WARNING: Not(?) Finsihed IMPLEMENTING SUBJECT TO CHANGE");
        //Area centerArea = new Area(0, rng.Next(1, 4));
        HexCell center = new HexCell(0, 0);
        //Debug.Log(center.Coords.ToString() + " Has hash: " + center.Coords.GetHashCode());
        WorldMap.Add(center.Coords, center);

        //set center in world map
        this[center.Coords] = center;

        for (int i = 1; i <= Radius; i++)
        {
            GenerateRing(i);
        }

        GenerateAreas((area)=> { return HexCell.DistanceBetweenCells(area.Cells[0], this[0, 0]);});

        GenerateBiomes();

    }

    /// <summary>
    /// Generates A ring of new HexCells in a certain distance, and places them in the map.
    /// </summary>
    /// <param name="distanceOfRingFromCenter">The distance of the ring from the center</param>
    void GenerateRing(int distanceOfRingFromCenter)
    {
        Coords coord = new Coords(-distanceOfRingFromCenter, 0);

        //walk a long a ring with radius i
        for (int j = 0; j < 6; j++)
        {
            for (int k = 0; k < distanceOfRingFromCenter; k++)
            {
                //this[coord] = new HexCell(coord);
                //Debug.Log(coord.ToString() + " Has hash: " + coord.GetHashCode());
                WorldMap.Add(coord, new HexCell(coord));
                //HandleAreaForCell(this[coord]);
                coord = Coords.GetNeighborCoords(coord, (HexDirection)j);
            }
        }
    }

    /// <summary>
    /// Generates the areas for the map
    /// </summary>
    /// <param name="AreaTierCalculation"> 
    /// A Function that calculates the Tier for an area, gets called after the area
    /// has all cells set that will be in this area
    /// </param>
    void GenerateAreas(Func<Area,int> AreaTierCalculation)
    {
        System.Random rng = new System.Random((int)DateTime.Now.Ticks);
        // lottery for area size
        #region Area Size Lottery
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(1, 30);
        lottery.Enter(2, 50);
        lottery.Enter(3, 42);
        lottery.Enter(4, 17);
        #endregion

        List<HexCell> nextPossibleAreaCenters = new List<HexCell>{this[0, 0]};
        //int i= 0;
        while (nextPossibleAreaCenters.Count > 0)
        {

            HexCell AreaBegin = nextPossibleAreaCenters[rng.Next(0, nextPossibleAreaCenters.Count)];
            nextPossibleAreaCenters.Remove(AreaBegin);
            List<HexCell> possibleCellsForArea = new List<HexCell>(GetNeighborsWithoutArea(AreaBegin));

            Area newArea = new Area(lottery.Draw());
            Areas.Add(newArea.AreaID, newArea);
            newArea.TryEstablishRelationWithCell(AreaBegin);
            //Debug.Log("Map::GenerateAreas - Chose Cell: " + AreaBegin.ToString());
            #region Fill area
            while (!newArea.IsFull() &&
                  possibleCellsForArea.Count > 0)
            {
                // get random cell from possible entries
                HexCell cell = possibleCellsForArea[rng.Next(0, possibleCellsForArea.Count)];
                //Debug.Log("Map::GenerateAreas - Chose Cell: " + cell.ToString());
                // add to area

                newArea.TryEstablishRelationWithCell(cell);
       
                // if cell was added to area
                // remove slectred cell form possneighbors and 
                // if in nextPossibleAreaCenters remove it as well
                if(cell.ParentArea != null)
                {
                    possibleCellsForArea.Remove(cell);
                    if (nextPossibleAreaCenters.Contains(cell))
                    {
                        //Debug.Log("Map::GenerateAreas - Removing cell from next possible area centers list");
                        nextPossibleAreaCenters.Remove(cell);
                    }// end if next area begin contains
                }// end if cell parent area null

                // get all neigbors without areas from this recently added cell
                // add them to possible neighbors
                foreach(HexCell c in GetNeighborsWithoutArea(cell))
                {
                    if(!possibleCellsForArea.Contains(c))
                        possibleCellsForArea.Add(c);
                }
            }// end while area not full and poss neighbors count > 0
            #endregion

            newArea.SetTier(AreaTierCalculation(newArea));
            //area finsihed
            //Debug.Log("Map::GenerateAreas - Finiehsd Area: "+ newArea.ToString());
            // add all cells in poss neigbors that are not already in nextPossibleAreaCenters to the list of
            // next possible area Centers
            //Debug.Log(possibleCellsForArea.Count + " cells left in possible cells for area");
            #region Adding outline of cells adjacent to cell with area assigned
            foreach (HexCell c in possibleCellsForArea)
            {
                if (!nextPossibleAreaCenters.Contains(c))
                {
                    //Debug.Log("Adding cell");
                    nextPossibleAreaCenters.Add(c);
                }
            }
            #endregion
        }// end while next poss area begins  count > 0
    }

    /// <summary>
    /// Generates the biomes each area has
    /// </summary>
    void GenerateBiomes()
    {
        AreaGraph = new AreaGraph(Areas.Values.ToList(),this);
        AreaGraph.ColorGraph();

        MergeAreasWithSameBiome(AreaGraph);

        foreach (AreaNode node in AreaGraph.GetNodes())
        {
            if (Areas.ContainsKey(node.NodeID))
            {
                Area areaForNode = Areas[node.NodeID];
                areaForNode.GenerateBiomeForArea(node.GetColor());
            }
        }
    }

    /// <summary>
    /// Merges areas that have the same color in the area graph
    /// </summary>
    void MergeAreasWithSameBiome(AreaGraph g)
    {
        // FIND ALL AREAS THAT HAVE TO BE MERGED WITH THIS AREA
        // MERGE ALL AT ONCE

        List<List<uint>> listOfAreasToMerge = g.GetAreasToMerge();

        foreach (List<uint> areasToMerge in listOfAreasToMerge)
        {
            int size = 0;
            List<HexCell> cellsInNewArea = new List<HexCell>();
            int tier = 0;

            foreach(uint areaID in areasToMerge)
            {
                if (Areas.ContainsKey(areaID))
                {
                    Area mergeArea = Areas[areaID];

                    // use capacity if merge area not completely filled
                    // aka can add to this area if map is expanding
                    size += mergeArea.Cells.Capacity;
                    cellsInNewArea.AddRange(mergeArea.Cells);
                    tier = (tier < mergeArea.Tier) ? mergeArea.Tier : tier;
                    RemoveArea(areaID);
                }
                else
                {
                    Debug.LogError("trying to acces area that does not exist. ID: " + areaID);
                }
            }// end foreach uint areaID
            // accumulated all things about the merging ares from cell to capcaity to tier
            Area newMergedArea = new Area(size);
            newMergedArea.SetTier(tier);
            foreach (HexCell cellInNewArea in cellsInNewArea)
            {
                newMergedArea.TryEstablishRelationWithCell(cellInNewArea);
            }
            Areas.Add(newMergedArea.AreaID, newMergedArea);
            //Debug.Log("Created new Area  with ID: " + newMergedArea.AreaID);
            AreaGraph.MergeNodesIntoNewNode(areasToMerge, newMergedArea.AreaID);
        }// end foreach list uint
    }

    /// <summary>
    /// Removes an area by id, and sets parentArea for the hexcells in this area null
    /// <param name="areaId">the ID of the Area</param>
    void RemoveArea(uint areaId)
    {
        if (Areas.ContainsKey(areaId))
        {
            Area areaToRemove = Areas[areaId];
            foreach (HexCell cell in areaToRemove.Cells)
            {
                cell.ParentArea = null;
            }
            Areas.Remove(areaId);
        }
    }

    /// <summary>
    /// Returns all neighbors of a cell that don't have an area assigned
    /// </summary>
    /// <param name="cell">the cell from which we want the area less neighbors</param>
    /// <returns></returns>
    List<HexCell> GetNeighborsWithoutArea(HexCell cell)
    {
        List<HexCell> neighors = new List<HexCell>();

        Neighbors neigh = GetNeighborsForCell(cell);

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

    /// <summary>
    /// Returns an Area by a given area ID
    /// </summary>
    /// <param name="id">The ID of the area</param>
    /// <returns>The area wanted, if it exists otherwise null</returns>
    public Area GetAreaByID(uint id)
    {
        if (Areas.ContainsKey(id))
        {
            return Areas[id];
        }
        else
        {
            Debug.Log("Map::getAreaByID - There exists no area with this ID");
            return null;
        }
    }

    /// <summary>
    /// Calculates the total number of hexcells in the map
    /// </summary>
    /// <returns>the number of cells in the map</returns>
    public int GetTotalNumberOfHexCellsInMap()
    {
        // (2*radius +1) ^ 2 - (radius * (radius +1)) 
        return 1 + (3 * (Radius * (Radius + 1)));
    }

    public HexCell this[Coords coord]
    {
        get
        {
            if(WorldMap.ContainsKey(coord))
                return WorldMap[coord];
            //Debug.LogWarning("Map::this[Coords coord] - No HexCell in Map for these coords: " + coord.ToString());
            return null;
        }
        set
        {
            if (WorldMap.ContainsKey(coord))
            {
                WorldMap[coord] = value;
            }
            else
            {
                WorldMap.Add(coord,value);
            }
        }

    }

    public HexCell this[int x, int y]
    {
        get
        {
            //return WorldMap[y + Radius, x + Radius + Mathf.Min(0, y)];
            Coords c = new Coords(x, y);
            if (WorldMap.ContainsKey(c))
                return WorldMap[c];
            //Debug.LogWarning("Map::this[int x, int y] - No HexCell in Map for these coords: " + c.ToString());
            return null;
        }
        set
        {
            //WorldMap[y + Radius, x + Radius + Mathf.Min(0, y)] = value;
            Coords c = new Coords(x, y);
            if (WorldMap.ContainsKey(c))
            {
                WorldMap[c] = value;
            }
            else
            {
                WorldMap.Add(c, value);
            }
        }
    }


    /// <summary>
    /// Returns all the neighbor cells of a given cell
    /// </summary>
    /// <param name="center">the center cell of which we want the neigbors</param>
    /// <returns>A neighbors struct containint an array with all the neighbors and the center</returns>
    public Neighbors GetNeighborsForCell(HexCell center)
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
    /// Returns all neigboring areas
    /// </summary>
    /// <param name="area">The center area where we want the ngihbors from</param>
    /// <returns></returns>
    public List<Area> GetNeighboringAreas(Area area)
    {
        List<Area> neighboringAreas = new List<Area>();

        foreach (HexCell cell in area.Cells)
        {
            Neighbors neigh = GetNeighborsForCell(cell);
            foreach (HexCell c in neigh.GetExistingNeighbors())
            {
                if (c.ParentArea != cell.ParentArea)
                {
                    neighboringAreas.Add(c.ParentArea);
                }
            }

        }
        return neighboringAreas;
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
        {
            //if(dictMap.ContainsKey(hexCoords))
            //{
            //    Debug.Log("COORD:" + hexCoords.ToString() + " WOULD HAVE BEEN FOUND IN DICT MAP");
            //}
            return this[hexCoords];
        }
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

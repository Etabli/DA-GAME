using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    #region Member Varaiables
    public BiomeType BiomeType { get; protected set; }
    public List<HexCell> Cells { get; protected set; }
    public int Tier { get; protected set; }
    public int PossibleAreaSize { get; protected set; }
    #endregion

    #region ctors

    public Area(int tier,int size,BiomeType type)
    {
        this.Tier = Tier;
        this.PossibleAreaSize = size;
        BiomeType = type;
        Cells = new List<HexCell>();
    }

    #endregion
    

    /// <summary>
    /// Add an item to the cell list of the area
    /// </summary>
    /// <param name="cell">The cell to add</param>
    public void AddToCellList(HexCell cell)
    {
        Cells.Add(cell);
    }
    /// <summary>
    /// Test if there is still space for new cells in list
    /// </summary>
    /// <returns>True when it is full, false when there is still room left</returns>
    public bool AreaIsFull()
    {
        return PossibleAreaSize == Cells.Count;
    }


}

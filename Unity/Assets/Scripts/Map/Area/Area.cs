using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    #region static

    private static uint AreaIDGen = 0;

    #endregion
    #region Member Varaiables
    public  uint AreaID { get; protected set; }
    public Biome AreaBiome { get; protected set; }
    public int Tier { get; protected set; }
    public List<HexCell> Cells { get; protected set; }
    #endregion

    #region ctors

    public Area(int size)
    {
        AreaID = AreaIDGen++;
        Cells = new List<HexCell>(size);
    }

    #endregion

    /// <summary>
    /// Adds a cell to the list of cells 
    /// </summary>
    /// <param name="cell">The cell to add</param>
    /// <returns>True if Area was not already full, false if area was full</returns>
    private bool AddToCellList(HexCell cell)
    {
        if(!IsFull())
        {
            Cells.Add(cell);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Trys to add cell to Area, if not over capacity
    /// when successfully added to area sets cells parent area to this area
    /// otherwise it does not
    /// </summary>
    /// <returns>True if succesfully esablied relation</returns>
    public bool TryEstablishRelationWithCell(HexCell cell)
    {
        if(cell.ParentArea == null)
        {
            if (AddToCellList(cell))
            {
                cell.ParentArea = this;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Test if there is still space for new cells in list
    /// </summary>
    /// <returns>True when it is full, false when there is still room left</returns>
    public bool IsFull()
    {
        return Cells.Capacity == Cells.Count;
    }

    public void  SetTier(int tier)
    {
        Tier = tier;
    }

    #region BiomeRelaedStuff

    /// <summary>
    /// Generates a biome for this area
    /// </summary>
    /// <param name="type">the type of biome</param>
    public void GenerateBiomeForArea(BiomeType type)
    {
        AreaBiome = new Biome(BiomeInfo.GetBiomeInfo(type), Tier);
    }

    #endregion

    #region override
    public override string ToString()
    {
        return string.Format("Area {5}:\n Tier: {0}\n BiomeType: {1}\n Size: {2} out of {3} possible Cells\n Origin Cell: {4}",
                            Tier, AreaBiome.Type, Cells.Count, Cells.Capacity, Cells[0].Coords,AreaID);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        if (AreaID != ((Area)obj).AreaID)
            return false;
        return true;
        //Area obje = (Area)obj;
        //if (Cells.Capacity != obje.Cells.Capacity)
        //    return false;
        //if (AreaBiome.Type != obje.AreaBiome.Type)
        //    return false;
        //if (Tier != obje.Tier)
        //    return false;
        //if (Cells.Count != obje.Cells.Count)
        //    return false;
        //for (int i = 0; i < Cells.Count; i++)
        //{
        //    if (!Cells[i].Equals(obje.Cells[i]))
        //        return false;
        //}

        //return true;
    }

    public override int GetHashCode()
    {
        return ((((((79 + Cells[0].Coords.X) * 41) + Cells[0].Coords.Y) * 67) + (Tier << (int)AreaBiome.Type)) * 29) ^ Cells[0].GetHashCode();
    }

    #endregion
}

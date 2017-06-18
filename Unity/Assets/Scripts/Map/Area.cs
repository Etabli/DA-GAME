using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    #region Member Varaiables
    public Biome AreaBiome { get; protected set; }

    public List<HexCell> Cells { get; protected set; }
    #endregion

    #region ctors

    public Area(int tier,int size)
    {

        this.Cells = new List<HexCell>(size);
        GetBiomeForArea(tier);
    }

    #endregion

    /// <summary>
    /// Adds a cell to the list of cells 
    /// </summary>
    /// <param name="cell">The cell to add</param>
    /// <returns>True if Area was not already full, false if area was full</returns>
    private bool AddToCellList(HexCell cell)
    {
        if(!AreaIsFull())
        {
            Cells.Add(cell);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    public void EstablishAreaCellRelationship(HexCell cell)
    {
        if(AddToCellList(cell))
        {
            cell.ParentArea = this;
        }
    }



    /// <summary>
    /// Test if there is still space for new cells in list
    /// </summary>
    /// <returns>True when it is full, false when there is still room left</returns>
    public bool AreaIsFull()
    {
        return Cells.Capacity == Cells.Count;
    }


    #region BiomeRelaedStuff

    /// <summary>
    /// Gets a random BiomeType for this area, depending on tier.
    /// Only biomes that exist in the biome info dictioanry are possible to be chosen
    /// </summary>
    void GetBiomeForArea(int tier)
    {
        Lottery<BiomeType> biomeLottery  = new Lottery<BiomeType>();

        foreach(BiomeType type in BiomeInfo.GetPossibleBiomesForTier(tier))
        {
            biomeLottery.Enter(type, 20);
        }

        AreaBiome = new Biome(BiomeInfo.GetBiomeInfo(biomeLottery.GetWinner()),tier);

    }




    #endregion

}

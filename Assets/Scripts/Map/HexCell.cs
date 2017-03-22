using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell
{
    #region MemberVariables
    public Coords Coords { get; protected set; }

    public Area ParentArea { get; protected set; }

    public Base LocalBase { get; protected set; }
    #endregion
}

/// <summary>
/// Coords are 2D coordinates.
/// Currently used for an axial Coords representation of Hexagonal maps.
/// </summary>
public class Coords
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coords(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
}

/// <summary>
/// This struct contains the neighbors of a HexCell
/// </summary>
/// 
public struct Neighbors
{
    HexCell[] neighbors;
    HexCell center;

    public Neighbors(HexCell _center, HexCell[] _neighbors)
    {
        this.center = _center;
        this.neighbors = _neighbors;
    }
    /// <summary>
    /// This function returns the neighbor given by, direction of the hexagonal cell center.
    /// 
    /// </summary>
    /// <param name="direction">The direction of the neigbor as seen from the center cell</param>
    /// <returns>The corresponding neighbor</returns>
    public HexCell GetNeighbor(HexDirection direction)
    {
        //TODO: what if the center is an outskirt cell and there is no value in the array for this neighbor
        return neighbors[(int)direction];
    }

    /// <summary>
    /// Get the center of this neighbor struct
    /// </summary>
    /// <returns>The center</returns>
    public HexCell GetCenter()
    {
        return center;
    }

    /// <summary>
    /// This functions returns more than one neighbor.
    /// The order of the directions of the neighbor cell in the returned array is equivalent to the order of directions in the given list
    /// Without any directions given the whole neighbors array is returned.
    /// <param name="directions">The directions of the wanted neighbors </param>
    /// <returns>The neighbor cells wanted</returns>
    public HexCell[] GetNeighbors(List<HexDirection> directions = null)
    {
        if (directions == null || directions.Count == 6)
            return neighbors;


        //TODO: What if the neighbor array is not full

        HexCell[] wantedNeighbors = new HexCell[directions.Count];

        for (int i = 0; i < directions.Count; i++)
        {
            wantedNeighbors[i] = GetNeighbor(directions[i]);
        }
        return wantedNeighbors;
    }
}

/// <summary>
/// Used to identify the direction of a neighbor
/// </summary>
public enum HexDirection
{
    rightAbove,
    right,
    rightBelow,
    leftBelow,
    left,
    LeftAbove
}
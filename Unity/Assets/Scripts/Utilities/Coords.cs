using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    #region Convert

    /// <summary>
    /// converts a coordinat into string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return "(" + X + "," + Y + ")";
    }

    /// <summary>
    /// converts a coordinate into a vector 2
    /// </summary>
    /// <returns></returns>
    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }
    #endregion


    #region operators
    public static Coords operator +(Coords lhs, Coords rhs)
    {
        return new Coords(lhs.X + rhs.X, lhs.Y + rhs.Y);
    }

    public static Coords operator -(Coords lhs, Coords rhs)
    {
        return new Coords(lhs.X - rhs.X, lhs.Y - rhs.Y);
    }

    public static Coords operator *(Coords lhs, int scale)
    {
        return new Coords(lhs.X * scale, lhs.Y * scale);
    }


    #endregion

    #region Negihborhood
    /// <summary>
    /// Used as offset Reference for all the neighbors of a axial coords system
    /// ordered:
    ///     rightAbove, 
    ///     right, 
    ///     rightBelow, 
    ///     leftBelow, 
    ///     left, 
    ///     leftAbove
    /// The orderin is consitent with the HexDirection enum ordering
    /// </summary>
    public static readonly Coords[] HEX_DIRECTIONS_OFFSET = { new Coords(1, -1), new Coords(1, 0), new Coords(0, 1), new Coords(-1, 1), new Coords(-1, 0), new Coords(0, -1) };



    /// <summary>
    /// Returns the coords of a neighbor to the cell center with the direction direction
    /// </summary>
    /// <param name="center">the cell from which you want the neighbor</param>
    /// <param name="direction">the direction the neighbor wanted</param>
    /// <returns>the coords of the neighbor</returns>
    public static Coords GetNeighborCoords(HexCell center, HexDirection direction)
    {
        return center.Coords + HEX_DIRECTIONS_OFFSET[(int)direction];
    }

    /// <summary>
    /// This function calculates the coords of a neighbor in a specific direction
    /// </summary>
    /// <param name="center"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Coords GetNeighborCoords(Coords center, HexDirection direction)
    {
        return center + HEX_DIRECTIONS_OFFSET[(int)direction];
    }

    /// <summary>
    /// This function returns all the coordinates of the neighbors of a cell.
    /// The ordering is consitent with the enum HexDirection
    /// </summary>
    /// <param name="center">the center cell</param>
    /// <returns>An array of all neighbors</returns>
    public static Coords[] GetAllNeighborCoords(HexCell center)
    {
        return GetAllNeighborCoords(center.Coords);
    }

    /// <summary>
    /// Returns all neighbors of coordinates in an axial coords system
    /// The ordering of the array entries is consitent with the enum HexDirection.
    /// </summary>
    /// <param name="center">The center cell forw whom we get the neighbors</param>
    /// <returns>An array with all the neighbor coords</returns>
    public static Coords[] GetAllNeighborCoords(Coords center)
    {
        Coords[] neighbors = new Coords[6];

        for (int i = 0; i < 6; i++)
        {
            neighbors[i] = GetNeighborCoords(center, (HexDirection)i);
        }
        return neighbors;
    }
    #endregion

    #region distance

    /// <summary>
    /// Calculates the manhatten distance between two HexCells
    /// </summary>
    /// <param name="a">The first HexCell</param>
    /// <param name="b">The Second HexCell</param>
    /// <returns>The Manhatten distance as integer</returns>


    /// <summary>
    /// Calculates the manhatten distance between two axial Coords
    /// </summary>
    /// <param name="a">The first Coords</param>
    /// <param name="b">The Second Coords</param>
    /// <returns>The Manhatten distance as integer</returns>
    public static int Distance(Coords a, Coords b)
    {
        return ((Mathf.Abs(a.X - b.X) + Mathf.Abs((-a.X -a.Y) - (-b.X -b.Y))+ Mathf.Abs(a.Y - b.Y)) / 2);
    }

    #endregion
}

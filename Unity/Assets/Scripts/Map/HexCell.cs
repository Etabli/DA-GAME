using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    leftAbove
}

/// <summary>
/// The index of every cornern of a HexCell
/// Ordering:
///     Right below Center point, 
///     Directly below Center point, 
///     Left below center point, 
///     left above center point, 
///     directly above center point, 
///     right above center point
/// </summary>
public enum HexCorner
{
    rightBelowCenter,
    belowCenter,
    leftBelowCenter,
    leftAboveCenter,
    aboveCenter,
    rightAboveCenter
}

public class HexCell
{
    //size of one cell
    public static readonly float HEXCELL_SIZE = 3;
 
    #region MemberVariables
    public Coords Coords { get; protected set; }

    Area _parentArea;
    public Area ParentArea { get{ return _parentArea; }
                            set { _parentArea = value;} }

    public Base LocalBase { get; set; }
    #endregion

    #region ctor

    public HexCell(Coords coords)
    {
        Coords = coords;
        WorldController.Instance.OnHexCellCreated(this);
    }

    public HexCell(int x, int y)
    : this(new Coords(x, y))
    {}

    #endregion

    #region HelperFunction
    /// <summary>
    /// The height of a single cell
    /// </summary>
    public float Height
    { get { return HEXCELL_SIZE * 2; } }

    /// <summary>
    /// The vertical Distance between two HexCell center Points
    /// </summary>
    public float VerticalDistBetweenTwoHexCells
    {
        get
        {
            return this.Height * (3.0f / 4.0f);
        }
    }

    /// <summary>
    /// The width of a single Cell
    /// </summary>
    public float Width
    {
        get
        {
            return Mathf.Sqrt(3.0f) / 2.0f * this.Height;
        }
    }

    /// <summary>
    /// The horizontal distance between two HexCell CenterPoints
    /// </summary>
    public float HorizontalDistanceBetweenHexCells
    {
        get
        {
            return this.Width;
        }
    }

    /// <summary>
    /// Returns a given the world position of a given hex Cell
    /// </summary>
    /// <param name="corner">the corner wanted</param>
    /// <returns>The world position of the corner</returns>
    public Vector2 GetCorner(HexCorner corner)
    {
        // angle for corner in degree: 60° * corner + 30°
        float angleRad = Mathf.PI / 180.0f * (60.0f * (float)corner + 30.0f);
        Vector2 center = Map.HexCellToWorldPosition(this);
        return new Vector2(center.x + HEXCELL_SIZE * Mathf.Cos(angleRad), center.y + HEXCELL_SIZE * Mathf.Sin(angleRad));
    }

    /// <summary>
    /// Returns the world position coordinates of all corners
    /// ordering in array consitent with HexCorner Enum values
    /// </summary>
    /// <returns>All the cornerns in world position coordinates</returns>
    public Vector2[] GetAllCorners()
    {
        Vector2[] corners = new Vector2[6];
        for (int i = 0; i < 6; i++)
        {
            corners[i] = GetCorner((HexCorner)i);
        }
        return corners;
    }

    #endregion

    #region static hexCellFunctions

    /// <summary>
    /// Returns the coords of the nearest hex that contain these fractional coords
    /// </summary>
    /// <param name="fractionalCoords"></param>
    /// <returns></returns>
    public static Coords RoundToNearestHexCell(Vector2 fractionalCoords)
    {
        // for more reasons why rounding works this way visit:
        //http://www.redblobgames.com/grids/hexagons/#rounding

        //this is the cube coord representation
        //makes the rounding easier
        //cause x + y  + z = 0 has to be true always
        float x = fractionalCoords.x;
        float z = fractionalCoords.y;
        float y = -x - z;

        float roundedX = Mathf.Round(x);
        float roundedY = Mathf.Round(y);
        float roundedZ = Mathf.Round(z);

        float xDiff = Mathf.Abs(roundedX - x);
        float yDiff = Mathf.Abs(roundedY - y);
        float zDiff = Mathf.Abs(roundedZ - z);

        if (xDiff > yDiff && xDiff > zDiff)
            roundedX = -roundedY - roundedZ;
        else if (yDiff > zDiff)
            roundedY = -roundedX - roundedZ;
        else
            roundedZ = -roundedX - roundedY;

        return new Coords((int)roundedX, (int)roundedZ);
    }

    /// <summary>
    /// Returns the manhaten distance between two cells
    /// </summary>
    /// <param name="a">the first cell</param>
    /// <param name="b">the second cell</param>
    /// <returns>the distance between tem</returns>
    public static int DistanceBetweenCells(HexCell a, HexCell b)
    {
        return Coords.Distance(a.Coords, b.Coords);
    }

    #endregion

    #region overrides
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;
        return Coords.Equals(((HexCell)obj).Coords);
    }

    public override int GetHashCode()
    {
        return Coords.GetHashCode();
    }

    public override string ToString()
    {
        return string.Format("HexCell:\n Coords: {0}\n Area It belongs to:\t{1}\r\n Base in Cell: NOT IMPLEMENTED",
                            Coords.ToString(),(ParentArea != null ) ? ParentArea.ToString() : "Not in any area");
    }

    #endregion  

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
    /// Returned HexCell may be null
    /// </summary>
    /// <param name="direction">The direction of the neigbor as seen from the center cell</param>
    /// <returns>The corresponding neighbor</returns>
    public HexCell GetNeighbor(HexDirection direction)
    {
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
    /// Returned Array may contain null
    /// <param name="directions">The directions of the wanted neighbors </param>
    /// <returns>The neighbor cells wanted</returns>
    public HexCell[] GetNeighbors(List<HexDirection> directions = null)
    {
        if (directions == null || directions.Count == 6)
            return neighbors;

        HexCell[] wantedNeighbors = new HexCell[directions.Count];

        for (int i = 0; i < directions.Count; i++)
        {
            wantedNeighbors[i] = GetNeighbor(directions[i]);
        }
        return wantedNeighbors;
    }

    public List<HexCell> GetExistingNeighbors()
    {
        List<HexCell> existingNeighbors = new List<HexCell>();

        for (int i = 0; i < 6; i++)
        {
            if(neighbors[i] != null)
            {
                existingNeighbors.Add(neighbors[i]);
            }
        }

        return existingNeighbors;
    }
}


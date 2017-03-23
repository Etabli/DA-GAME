using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    #region Member Varaiables
    public BiomeType BiomeType { get; protected set; }
    public List<HexCell> Cells { get; protected set; }
    public int Tier { get; protected set; }
    #endregion




}

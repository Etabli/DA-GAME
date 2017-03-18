using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttributeType
{
    Health,
    PhysDmgFlat,
    FireRate
}

public abstract class Attribute
{ 
    public AttributeType Type { get; protected set; }
}

public class SingleValueAttribute : Attribute
{
    public float Value { get; protected set; }
}

public class RangeAttribute : Attribute
{
    public Range ValueRange { get; protected set; }
}

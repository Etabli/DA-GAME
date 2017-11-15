using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Stores affixes by reference. Can be linked together in a tree hierarchy
/// </summary>
public class AffixContainer
{
    #region Member Variables
    protected Dictionary<AffixType, List<Affix>> affixTypeMap = new Dictionary<AffixType, List<Affix>>();
    protected AffixContainer parent = null;
    protected List<AffixContainer> children = new List<AffixContainer>();
    #endregion

    #region Constructors
    public AffixContainer()
    { }

    public AffixContainer(Affix[] affixes)
    {
        foreach(var affix in affixes)
        {
            Add(affix);
        }
    }

    public AffixContainer(List<Affix> affixes) : this(affixes.ToArray())
    { }
    #endregion

    #region Container Functionality
    /// <summary>
    /// Adds an affix to the container.
    /// </summary>
    public void Add(Affix affix)
    {
        if (affixTypeMap.ContainsKey(affix.Type))
        {
            affixTypeMap[affix.Type].Add(affix);
        }
        else
        {
            affixTypeMap[affix.Type] = new List<Affix>();
            affixTypeMap[affix.Type].Add(affix);
        }
    }

    /// <summary>
    /// Removes an affix from the container
    /// </summary>
    public void Remove(Affix affix)
    {
        if (!affixTypeMap.ContainsKey(affix.Type))
        {
            Debug.LogError($"Could not find affix of type {affix.Type}");
            return;
        }

        if (!affixTypeMap[affix.Type].Remove(affix))
        {
            Debug.LogError($"Could not find affix {affix}");
        }
    }

    /// <summary>
    /// Removes all affixes from the container
    /// </summary>
    public void Clear()
    {
        affixTypeMap.Clear();
    }
    #endregion

    #region Graph Functionality
    /// <summary>
    /// Appends a child to this node
    /// </summary>
    /// <param name="child"></param>
    public void AppendChild(AffixContainer child)
    {
        if (child.parent != null)
        {
            Debug.LogError("Child already has a parent!");
            return;
        }

        if (child == this)
        {
            Debug.LogError("Can't append self as child!");
            return;
        }

        if (CheckChildProducesLoop(child))
        {
            Debug.LogError("Adding this child would produce a loop!");
            return;
        }

        children.Add(child);
        child.parent = this;
    }

    /// <summary>
    /// Appends multiple children to this node
    /// </summary>
    /// <param name="children"></param>
    public void AppendChildren(List<AffixContainer> children)
    {
        foreach(var child in children)
        {
            AppendChild(child);
        }
    }

    /// <summary>
    /// Checks if adding a node as a child of this node would produce a loop.
    /// </summary>
    /// <param name="child">The new child</param>
    /// <returns>Whether or not a loop would be created</returns>
    protected bool CheckChildProducesLoop(AffixContainer child)
    {
        for (AffixContainer current = this;  current != null; current = current.parent)
        {
            if (current == child)
                return true;
        }
        return false;
    }
    #endregion

    public override string ToString()
    {
        string result = "";

        foreach(var affix in affixTypeMap.Values.SelectMany(a => a))
        {
            result += affix.ToString() + "\n";
        }

        return result.TrimEnd();
    }
}

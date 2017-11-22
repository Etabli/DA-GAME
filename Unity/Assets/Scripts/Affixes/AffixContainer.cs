using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Stores affixes by reference. Can be linked together in a tree hierarchy
/// </summary>
public class AffixContainer
{
    #region Member Variables
    protected Dictionary<AffixType, HashSet<Affix>> affixTypeMap = new Dictionary<AffixType, HashSet<Affix>>();
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
            if (affixTypeMap[affix.Type].Contains(affix))
            {
                throw new ArgumentException($"Container already contains this affix! {affix}", nameof(affix));
            }
            affixTypeMap[affix.Type].Add(affix);
        }
        else
        {
            affixTypeMap[affix.Type] = new HashSet<Affix>();
            affixTypeMap[affix.Type].Add(affix);
        }
        Propagate(affix);
    }

    /// <summary>
    /// Removes an affix from the container
    /// </summary>
    public void Remove(Affix affix)
    {
        if (!affixTypeMap.ContainsKey(affix.Type))
        {
            throw new ArgumentException($"Could not find affix of type {affix.Type}", nameof(affix));
        }

        // This tries to remove and returns whether or not it was successful
        if (!affixTypeMap[affix.Type].Remove(affix))
        {
            throw new ArgumentException($"Could not find affix \"{affix}\"", nameof(affix));
        }

        PropagateRemove(affix);
    }

    /// <summary>
    /// Removes all affixes from the container
    /// </summary>
    public void Clear()
    {
        PropagateRemoveAll();
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
            throw new InvalidOperationException("Child already has a parent!");
        }

        if (child == this)
        {
            throw new InvalidOperationException("Can't append self as child!");
        }

        if (CheckChildProducesLoop(child))
        {
            throw new InvalidOperationException("Adding this child would produce a loop!");
        }

        children.Add(child);
        child.parent = this;
        child.PropagateAll();
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
    /// Attaches this node to a parent.
    /// </summary>
    /// <param name="parent"></param>
    public void AttachToParent(AffixContainer parent)
    {
        parent.AppendChild(this);
    }

    /// <summary>
    /// Disconnects this code from its parent
    /// </summary>
    public void DisconnectFromParent()
    {
        // Since we already contain all the information from our children, this is sufficient
        PropagateRemoveAll();
        parent.children.Remove(this);
        parent = null;
    }

    /// <summary>
    /// Removes a child from this node
    /// </summary>
    /// <param name="child"></param>
    public void RemoveChild(AffixContainer child)
    {
        child.DisconnectFromParent();
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

    #region Propagation
    /// <summary>
    /// Propagates a single affix upwards through the hierarchy.
    /// </summary>
    /// <param name="affix">The affix to be propagated</param>
    protected void Propagate(Affix affix)
    {
        parent?.Add(affix);
    }

    /// <summary>
    /// Propagates the removal of an affix upwards.
    /// </summary>
    /// <param name="affix"></param>
    protected void PropagateRemove(Affix affix)
    {
        parent?.Remove(affix);
    }

    /// <summary>
    /// Propagates all affixes in this container upwards.
    /// </summary>
    protected void PropagateAll()
    {
        foreach (var affix in affixTypeMap.Values.SelectMany(a => a ))
        {
            Propagate(affix);
        }
    }

    /// <summary>
    /// Propagates the removal of all affixes upwards
    /// </summary>
    protected void PropagateRemoveAll()
    {
        foreach (var affix in affixTypeMap.Values.SelectMany(a => a))
        {
            PropagateRemove(affix);
        }
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

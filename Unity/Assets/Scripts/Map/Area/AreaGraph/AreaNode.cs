using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class AreaNode
{
    List<BiomeType> possibleBiomes;
    public uint NodeID { get; protected set; }
    public HashSet<uint> Edges { get; protected set; }
    int color;
#region Ctors
    public AreaNode(uint id, HashSet<uint> edges, int color = -1)
    {
        NodeID = id;
        if (edges != null)
            Edges = edges;
        else
            Edges = new HashSet<uint>();

        this.color = color;
        if (color != -1)
            possibleBiomes = new List<BiomeType>() { (BiomeType)color };
    }

    public AreaNode(uint id, int color)
    {
        Edges = new HashSet<uint>();
        NodeID = id;
        this.color = color;
        possibleBiomes = new List<BiomeType>() { (BiomeType)color };
    }

    public AreaNode(Area area)
    {
        NodeID = area.AreaID;
        Edges = new HashSet<uint>();
        color = -1;
        possibleBiomes = BiomeInfo.GetPossibleBiomesForTier(area.Tier);

    }

#endregion

    /// <summary>
    /// Addes a single edge to this node
    /// </summary>
    /// <param name="edgeID">the id of the edge</param>
    public void AddEdge(uint edgeID)
    {
        Edges.Add(edgeID);
    }

    /// <summary>
    /// Adds multiple edges to this node
    /// </summary>
    /// <param name="edges">all the edges to be added</param>
    public void AddEdges(List<uint> edges)
    {
        foreach (uint edge in edges)
        {
            Edges.Add(edge);
        }
    }

    /// <summary>
    /// removes an edge from the node if contained
    /// </summary>
    /// <param name="edge">the edge to be removed</param>
    /// <returns>ture if succesfully removed, false if edge did not exist</returns>
    public bool RemoveEdge(uint edge)
    {
        if (Edges.Contains(edge))
        {
            Edges.Remove(edge);
            return true;
        }
        return false;
    }

    /// <summary>
    /// colors the node
    /// </summary>
    /// <param name="color">the color for the node</param>
    public void ColorNode(int color)
    {
        this.color = color;
    }

    /// <summary>
    /// Checks if a node is already colored
    /// </summary>
    /// <returns></returns>
    public bool IsColored()
    {
        return color != -1;
    }

    /// <summary>
    /// Returns the color of the node as an integer value
    /// </summary>
    /// <returns></returns>
    public int GetColorAsInt()
    {
        return color;
    }

    /// <summary>
    /// adds edges to a node, taking the area id
    /// </summary>
    /// <param name="neighborAreas"></param>
    public void AddEdges(List<Area> neighborAreas)
    {
        AddEdges(neighborAreas.ConvertAll(a => a.AreaID));
    }

    /// <summary>
    /// Updates the possible biomes for a node, if this node is not already colored
    /// </summary>
    /// <param name="notPossibleBiome">the biome that is not possible to for this node</param>
    public void UpdatePossibleBiomes(BiomeType notPossibleBiome)
    {
        if (!IsColored() && possibleBiomes.Count > 1)
        {
            if (possibleBiomes.Contains(notPossibleBiome))
            {
                possibleBiomes.Remove(notPossibleBiome);
            }// end if contains

            // check if only one poss biome left
            // if so color node
            if (possibleBiomes.Count == 1)
            {
                color = (int)possibleBiomes[0];
            }// end if count == 1
        }// end if node colored
        else if(!IsColored() && possibleBiomes.Count == 1)
        {
            color = (int)possibleBiomes[0];
        }
    }

    /// <summary>
    /// Removes all biomes from possible biomes if this node is not already colored
    /// </summary>
    /// <param name="biomesToRemove">allo the biomes to remove</param>
    public void  UpdatePossibleBiomes(List<BiomeType> biomesToRemove)
    {
        foreach(BiomeType biome in biomesToRemove)
        {
            UpdatePossibleBiomes(biome);
        }// end for each biome forme biomes to remove
    }

    /// <summary>
    /// Colors a node by choosing a random possible biometype and making it the only possibility
    /// </summary>
    public void ColorNode()
    {
        System.Random rng = new System.Random((int)System.DateTime.Now.Ticks);
        BiomeType type = possibleBiomes[rng.Next(possibleBiomes.Count)];
        // set bases color
        color = (int)type;
    }

    /// <summary>
    /// returns the color of this node
    /// </summary>
    /// <returns></returns>
    public  BiomeType GetColor()
    {
        return (BiomeType)color;
    }
}

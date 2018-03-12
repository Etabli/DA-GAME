using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AreaGraph
{

    Dictionary<uint, AreaNode> Graph;
    
    public AreaGraph(List<Area> areas, Map map)
    {
        Graph = new Dictionary<uint, AreaNode>();

        foreach (Area ar in areas)
        {
            Graph.Add(ar.AreaID, new AreaNode(ar));
            List<Area> neigh = map.GetNeighboringAreas(ar);
            Graph[ar.AreaID].AddEdges(neigh);
        }// end foreach area in areas

    }

    /// <summary>
    /// Colors a graph
    /// </summary>
    public void ColorGraph()
    {
        // loop throug every node
        // if node is colored -> update the possible colors for neighbors  ->next node
        // we have to update the neighboring nodes of an already colored one, becuse it was "accidentally"
        // because only one option was left, and we never had the chance to tell its neighbors
        // if not -> choose one possible color -> update neighbors possible colors and continue
        foreach(AreaNode node in Graph.Values)
        {
            if (!node.IsColored())
            {
                // node is not colored -> color it
                node.ColorNode();
            }
            // update neighbor possible colors
            foreach(uint neighNode in node.Edges)
            {
                Graph[neighNode].UpdatePossibleBiomes(node.GetColor());
            }
        }// end foreach node
    }

    /// <summary>
    /// returns nodes in graph
    /// </summary>
    /// <returns>nodes in graph</returns>
    public List<AreaNode> GetNodes()
    {
        return Graph.Values.ToList();
    }

    /// <summary>
    /// Creates a list of all neighboring areas with the same color that need to be merged into one area
    /// </summary>
    /// <returns>a list of lists of area ids of areas that need to be merged</returns>
    public List<List<uint>> GetAreasToMerge()
    {
        List<List<uint>> mergingLists = new List<List<uint>>();
        HashSet<uint> nodesInAnyMergingList = new HashSet<uint>();

        foreach (AreaNode node in Graph.Values)
        {
            // cehck not alread in a list
            if (nodesInAnyMergingList.Contains(node.NodeID))
                continue;

            bool noNeighWithSameColor = true;
            foreach (uint neighID in node.Edges)
            {
                if (Graph.ContainsKey(neighID))
                {
                    if (node.GetColor() == Graph[neighID].GetColor())
                    {
                        noNeighWithSameColor = false;
                        break;
                    }
                }
            }// end foreach neighID
             // no neighbor with same color
             //continue
            if (noNeighWithSameColor)
                continue;

            // loop over all nodes with same color
            // and see if their neighnodes have same color
            // if so add to list if not already contained
            // extends loop until no more new edge found
            List<uint> nodesToMerge = new List<uint>() { node.NodeID};
            nodesInAnyMergingList.Add(node.NodeID);
            for(int i = 0; i < nodesToMerge.Count; i++)
            {
                uint nodeID = nodesToMerge[i];
                if (Graph.ContainsKey(nodeID))
                {
                    foreach(uint neighborID  in Graph[nodeID].Edges)
                    {
                        if (Graph.ContainsKey(neighborID))
                        {
                            if(Graph[nodeID].GetColor() == Graph[neighborID].GetColor())
                            {
                                if (!nodesToMerge.Contains(neighborID))
                                {
                                    nodesToMerge.Add(neighborID);
                                    nodesInAnyMergingList.Add(neighborID);
                                }// end if nodesToMerge contains neighID
                            }
                        }// end if nodes contains neighID
                    }// end foreach neighide
                }// end if nodes contains nodeID
            }// end for i = 0 nodesToMerge.Count
            
            // adding completed merging list
            mergingLists.Add(nodesToMerge);
        }// end foreach ndoe
        return mergingLists;
    }

    /// <summary>
    /// Merges Nodes int the graph into a new node
    /// </summary>
    /// <param name="mergeNodeIDs">the nodes to be merged</param>
    /// <param name="newAreaTheyCreated">the area they craeated in the map, wiht the new id for the new node</param>
    public void MergeNodesIntoNewNode(List<uint> mergeNodeIDs, uint newNodeID)
    {
        // remove all edges that link to the nodes that need to be merged
        // and store nodes that have edges to this node
        // when they are not to be removed themself
        HashSet<uint> NodesThatHadEdgeToMergingNode = new HashSet<uint>();
        foreach(AreaNode nod in Graph.Values)
        {
           foreach(uint nodeToMergeID in mergeNodeIDs)
           {
                // remove edge and add to nodes that had edge if not already contained
                // or contained in nodes to merge
                if(nod.RemoveEdge(nodeToMergeID) &&
                    !mergeNodeIDs.Contains(nod.NodeID))
                {
                    NodesThatHadEdgeToMergingNode.Add(nod.NodeID);
                }
           }
        }
        int color = (int)Graph[mergeNodeIDs[0]].GetColor();
        AreaNode newMergedNode = new AreaNode(newNodeID, NodesThatHadEdgeToMergingNode, color);
        // remove all nodes that need to be merged
        foreach (uint nodeToMergeID in mergeNodeIDs)
        {
            Graph.Remove(nodeToMergeID);
        }
        Graph.Add(newNodeID, newMergedNode);
    }
}

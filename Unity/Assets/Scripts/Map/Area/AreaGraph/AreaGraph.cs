using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AreaGraph{

    Dictionary<uint, AreaNode> nodes;
    
    public AreaGraph(List<Area> areas, Map map)
    {
        nodes = new Dictionary<uint, AreaNode>();

        foreach (Area ar in areas)
        {
            nodes.Add(ar.AreaID, new AreaNode(ar));
            List<Area> neigh = map.GetNeighboringAreas(ar);
            nodes[ar.AreaID].AddEdges(neigh);
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
        Debug.Log("AreaGraph::ColorGraph - Coloring Graph");
        foreach(AreaNode node in nodes.Values)
        {
            if (!node.IsColored())
            {
                // node is not colored -> color it
                node.ColorNode();
            }
            // update neighbor possible colors
            foreach(uint neighNode in node.Edges)
            {
                nodes[neighNode].UpdatePossibleBiomes(node.GetColor());
            }
        }// end foreach node
        Debug.Log("AreaGraph::ColorGraph - Finished Coloring Graph");
    }

    /// <summary>
    /// returns nodes in graph
    /// </summary>
    /// <returns>nodes in graph</returns>
    public List<AreaNode> GetNodes()
    {
        return nodes.Values.ToList();
    }

    /// <summary>
    /// Creates a list of all neighboring areas with the same color that need to be merged into one area
    /// </summary>
    /// <returns>a list of lists of area ids of areas that need to be merged</returns>
    public List<List<uint>> GetAreasToMerge()
    {
        List<List<uint>> mergingLists = new List<List<uint>>();

        foreach (AreaNode node in nodes.Values)
        {
            // cehck not alread in a list
            bool alreadyInList = false;
            foreach (List<uint> mergingList in mergingLists)
            {
                if (mergingList.Contains(node.NodeID))
                    alreadyInList = true;
            }
            if (alreadyInList)
                continue;

            // no neighbor with same color
            //continue
            bool noNeighWithSameColor = true;
            foreach (uint neighID in node.Edges)
            {
                if (nodes.ContainsKey(neighID))
                {
                    if (node.GetColor() == nodes[neighID].GetColor())
                        noNeighWithSameColor = false;
                }
            }// end foreach neighID
            if (noNeighWithSameColor)
                continue;
            
            // loop over all nodes with same color
            // and see if their neighnodes have same color
            // if so add to list if not already contained
            // extends loop until no more new edge found
            List<uint> nodesToMerge = new List<uint>() { node.NodeID};
            for(int i = 0; i < nodesToMerge.Count; i++)
            {
                uint nodeID = nodesToMerge[i];
                if (nodes.ContainsKey(nodeID))
                {
                    foreach(uint neighID  in nodes[nodeID].Edges)
                    {
                        if (nodes.ContainsKey(neighID))
                        {
                            if(nodes[nodeID].GetColor() == nodes[neighID].GetColor())
                            {
                                if (!nodesToMerge.Contains(neighID))
                                {
                                    nodesToMerge.Add(neighID);
                                }// end if nodesToMerge contains neighID
                            }
                        }// end if nodes contains neighID
                    }// end foreach neighide
                }// end if nodes contains nodeID
            }// end for i = 0 nodesToMerge.Count
            mergingLists.Add(nodesToMerge);

        }// end foreach ndoe


        #region debug about areas to merge
        Debug.Log("All areas to Merge: ");
        string mergerstring = "";
        foreach (List<uint> merger in mergingLists)
        {
            mergerstring += nodes[merger[0]].GetColor().ToString() + " Areas to merge: ";
            foreach (uint id in merger)
            {
                mergerstring += id.ToString() + ", ";
            }
            mergerstring += "\n";
        }
        Debug.Log(mergerstring);
        Debug.Log("--------------------------------------------------");
        #endregion
        return mergingLists;
    }

    /// <summary>
    /// Merges Nodes int the graph into a new node
    /// </summary>
    /// <param name="mergeNodeIDs">the nodes to be merged</param>
    /// <param name="newAreaTheyCreated">the area they craeated in the map, wiht the new id for the new node</param>
    public void MergeNodes(List<uint> mergeNodeIDs, uint newNodeID)
    {
        // remove all edges that link to the nodes that need to be merged
        // and store nodes that have edges to this node
        // when they are not to be removed themself
        List<uint> nodesThatHadEdge = new List<uint>();
        foreach(AreaNode nod in nodes.Values)
        {
           foreach(uint nodeToMergeID in mergeNodeIDs)
           {
                // remove edge and add to nodes that had edge if not already contained
                // or contained in nodes to merge
                if(nod.RemoveEdge(nodeToMergeID) &&
                    !nodesThatHadEdge.Contains(nod.NodeID) &&
                    !mergeNodeIDs.Contains(nod.NodeID))
                {
                    nodesThatHadEdge.Add(nod.NodeID);
                }
           }
        }
        int color = (int)nodes[mergeNodeIDs[0]].GetColor();
        AreaNode newMergedNode = new AreaNode(newNodeID, nodesThatHadEdge, color);
        // remove all nodes that need to be merged
        foreach (uint nodeToMergeID in mergeNodeIDs)
        {
            nodes.Remove(nodeToMergeID);
        }
        nodes.Add(newNodeID, newMergedNode);
    }
}

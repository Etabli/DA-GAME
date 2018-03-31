using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Linq;

using Random = System.Random;

/// <summary>
/// A general Exception from mishandling a Lottery
/// </summary>
public class LotteryException : Exception
{
    public LotteryException()
    { }

    public LotteryException(string message) : base(message)
    { }

    public LotteryException(string message, Exception inner) : base(message, inner)
    { }
}

/// <summary>
/// A lottery for randomly drawing from a pool of objects. Fast for adding
/// entrants and drawing but slow for batch drawing and removing entrants
/// </summary>
/// <typeparam name="T">The type of object the lottery is for</typeparam>
[DataContract]
public class Lottery<T> : ILottery<T> where T : IEquatable<T>
{
    #region Block
    /// <summary>
    /// Stores starting index and size of a continuous block belonging to a single entrant
    /// </summary>
    private class Block
    {
        public int StartingIndex;
        public int Size;

        public int LastIndex { get { return StartingIndex + Size; } }

        public Block(int startingIndex, int size)
        {
            if (size < 1)
                throw new ArgumentException($"Cannot create block with size {size}!");

            StartingIndex = startingIndex;
            Size = size;
        }
    }

    private class Blocks 
    {
        List<Block> blocks;
        public int TotalSize;

        public Blocks(Block block)
        {
            blocks = new List<Block>(1) { block };
            TotalSize = block.Size;
        }

        public Blocks(List<Block> blocks)
        {
            this.blocks = blocks;
            TotalSize = blocks.Aggregate(0, (n, block) => n + block.Size);
        }

        public int Count { get { return blocks.Count; } }
        public int LastIndex
        {
            get
            {
                var last = blocks.Last();
                return last.StartingIndex + last.Size;
            }
        }

        public void Add(Block block)
        {
            blocks.Add(block);
            TotalSize += block.Size;
        }

        public void AddToLast(int value)
        {
            if (value < 0)
                throw new ArgumentException("Can't add negative values to a block!", nameof(value));

            blocks.Last().Size += value;
            TotalSize += value;
        }

        /// <summary>
        /// Cuts off all contents after a given index (exclusive)
        /// </summary>
        public void CutAfter(int index)
        {
            while (blocks.Count > 0 && blocks.Last().StartingIndex > index)
                RemoveAt(Count - 1);

            if (blocks.Count > 0)
            {
                var block = blocks.Last();
                if (block.LastIndex > index)
                {
                    TotalSize -= block.Size;
                    block.Size = index - block.StartingIndex + 1;
                    TotalSize += block.Size;
                }
            }
        }

        /// <summary>
        /// Decreases the total size by the given value
        /// </summary>
        /// <param name="value"></param>
        public void CutBy(int value)
        {
            while (blocks.Count > 0 && blocks.Last().Size <= value)
            {
                value -= blocks.Last().Size;
                RemoveAt(blocks.Count - 1);
            }

            if (value > 0 && blocks.Count > 0)
            {
                var block = blocks.Last();
                block.Size -= value;
                TotalSize -= value;
            }
        }

        public void Remove(Block block)
        {
            TotalSize -= block.Size;
            blocks.Remove(block);
        }

        public void RemoveAt(int index)
        {
            TotalSize -= blocks[index].Size;
            blocks.RemoveAt(index);
        }

        public Block Last() => blocks.Last();

        public Block this[int index]
        {
            get { return blocks[index]; }
            set { blocks[index] = value; }
        }
    }
    #endregion

    // List which contains one entry for each ticket
    [DataMember]
    List<T> entrants;

    // Only used to store total number of tickets in each block of entries
    // Actual drawing only happens fromt he entrants list
    Dictionary<T, Blocks> entrantBlocks = new Dictionary<T, Blocks>();

    // Our own random generator to make seeding possible
    Random rng;

    #region Constructors
    public Lottery() : this(2)
    { }

    public Lottery(int capacity) : this(capacity, (int)DateTime.Now.Ticks)
    { }

    public Lottery(int capacity, int seed)
    {
        entrants = new List<T>(capacity);
        rng = new Random(seed);
    }

    public Lottery(Lottery<T> src) : this()
    {
        src.CombineInto(this);
    }
    #endregion

    #region Enter
    /// <summary>
    /// Enters an object into the lottery
    /// </summary>
    public void Enter(T entrant, int tickets)
    {
        if (tickets < 1)
            throw new LotteryException($"Cannot enter with {tickets} ticktes!");

        // Adjust capacity if necessary
        if (entrants.Capacity < entrants.Count + tickets)
            entrants.Capacity = entrants.Count + tickets;
        
        // Update blocks
        if (entrantBlocks.ContainsKey(entrant))
        {
            if (entrants.Last().Equals(entrant))
            {
                entrantBlocks[entrant].AddToLast(tickets);
            }
            else
            {
                entrantBlocks[entrant].Add(new Block(entrants.Count, tickets));
            }
        }
        else
        {
            entrantBlocks.Add(entrant, new Blocks(new Block(entrants.Count, tickets)));
        }

        // Add entries themselves
        for (int i = 0; i < tickets; i++)
        {
            entrants.Add(entrant);
        }
    }

    /// <summary>
    /// Enters an object into the lottery
    /// </summary>
    /// <param name="entrant"></param>
    public void Enter(T entrant) => Enter(entrant, 1);
    #endregion

    #region Remove
    /// <summary>
    /// Removes an entrant from the lottery
    /// </summary>
    public void Remove(T entrant) => Remove(entrant, entrantBlocks[entrant].TotalSize);

    /// <summary>
    /// Removes a given amount of tickets from an entrant
    /// </summary>
    public void Remove(T entrant, int tickets)
    {
        if (!entrantBlocks.ContainsKey(entrant))
            return;
        if (tickets < 1)
            return;

        int lastIndex = entrants.Count;
        int remaining = tickets;
        var blocks = entrantBlocks[entrant];
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            var block = blocks[i];
            if (block.Size <= remaining)
            {
                lastIndex = block.StartingIndex;
                entrants.RemoveRange(block.StartingIndex, block.Size);
                remaining -= block.Size;
            }
            else
            {
                int diff = block.Size - remaining;
                int start = block.StartingIndex + diff;
                lastIndex = start;
                entrants.RemoveRange(start, block.Size - diff);
                break;
            }
        }

        blocks.CutBy(tickets);
        if (blocks.TotalSize == 0)
            entrantBlocks.Remove(entrant);

        RebuildBlocks(lastIndex);
    }
    #endregion

    #region Draw
    public T Draw()
    {
        throw new NotImplementedException();
    }

    public List<T> Draw(int n)
    {
        throw new NotImplementedException();
    }

    public T Draw(HashSet<T> blacklist)
    {
        throw new NotImplementedException();
    }
    #endregion

    #region Batch Draw
    public void StartBatchDraw()
    {
        throw new NotImplementedException();
    }

    public void StartBatchDraw(HashSet<T> blacklist)
    {
        throw new NotImplementedException();
    }

    public void EndBatchDraw()
    {
        throw new NotImplementedException();
    }
    #endregion

    #region GetTickets
    public int GetTickets(T entrant)
    {
        if (!entrantBlocks.ContainsKey(entrant))
            return 0;

        return entrantBlocks[entrant].TotalSize;
    }
    #endregion

    #region ChangeSeed
    /// <summary>
    /// Changes the current seed of the lottery
    /// </summary>
    /// <param name="seed">The new seed</param>
    public void ChangeSeed(int seed)
    {
        rng = new System.Random(seed);
    }

    public void ChangeSeed()
    {
        ChangeSeed((int)DateTime.Now.Ticks);
    }
    #endregion

    #region Combining
    /// <summary>
    /// Enters all entrants of this lottery into the given lottery
    /// </summary>
    public void CombineInto(ILottery<T> lottery)
    {
        foreach (var entrant in entrantBlocks.Keys)
        {
            lottery.Enter(entrant, entrantBlocks[entrant].TotalSize);
        }
    }

    /// <summary>
    /// Returns a combined lottery between this lottery and the given lottery
    /// </summary>
    /// <param name="lottery"></param>
    /// <returns></returns>
    public ILottery<T> CombineWith(ILottery<T> lottery)
    {
        Lottery<T> result = new Lottery<T>();
        lottery.CombineInto(result);
        this.CombineInto(result);
        return result;
    }

    public static Lottery<T> Combine(Lottery<T> lottery1, Lottery<T> lottery2)
    {
        return lottery1.CombineWith(lottery2) as Lottery<T>;
    }
    #endregion

    #region Private Helper Functions
    /// <summary>
    /// Rebuilds the blocks of all entrants starting from a given index
    /// </summary>
    /// <param name="startingIndex"></param>
    private void RebuildBlocks(int startingIndex)
    {
        if (startingIndex >= entrants.Count)
            return;

        // First cut off all blocks that are after the starting index
        foreach (var blocks in entrantBlocks.Values)
            blocks.CutAfter(startingIndex);

        // Check if we're starting in the middle of a block
        if (startingIndex > 0 && ReferenceEquals(entrants[startingIndex - 1], entrants[startingIndex]))
        {
            T entrant = entrants[startingIndex];

            int start = startingIndex;
            int end = FindEndOfBlock(start);
            entrantBlocks[entrant].AddToLast(end - start);
            startingIndex = end;
        }

        // Finally scan over all remaining entrants and add new blocks as needed
        for (int i = startingIndex; i < entrants.Count;)
        {
            T entrant = entrants[i];

            // Scan until end of block
            int start = i;
            i = FindEndOfBlock(start);

            entrantBlocks[entrant].Add(new Block(start, i - start));
        }
    }

    /// <summary>
    /// Scans through the entrants to find the end of a block
    /// </summary>
    /// <returns>The first index after the block.</returns>
    private int FindEndOfBlock(int start)
    {
        T entrant = entrants[start];

        int end = start;
        for (; end < entrants.Count && entrants[end].Equals(entrant); end++) ;

        return end;
    }
    #endregion
}

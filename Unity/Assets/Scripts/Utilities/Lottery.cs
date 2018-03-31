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
/// A lottery for randomly drawing from a pool of objects. Very fast for drawing
/// ,reasonably fast for adding entrants, and slow for removing and drawing with blacklist. For 
/// large lotteries specifying a capacity (total number of expected tickets) vastly
/// improves adding time.
/// </summary>
/// <typeparam name="T">The type of object the lottery is for</typeparam>
[DataContract]
public class Lottery<T> : ILottery<T> where T : IEquatable<T>
{
    #region Block
    /// <summary>
    /// Stores starting index and size of a continuous block belonging to a single entrant
    /// </summary>
    private class Block : IComparable<Block>
    {
        /// <summary>
        /// Called when the block's size changes. Argument is the difference in size
        /// </summary>
        public event Action<int> OnSizeChanged;

        public T Entrant;
        public int StartingIndex;

        private int _size;
        public int Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    int diff = value - _size;
                    _size = value;
                    OnSizeChanged?.Invoke(diff);
                }
            }
        }

        public int LastIndex { get { return StartingIndex + Size; } }

        public Block(T entrant, int startingIndex, int size)
        {
            if (size < 1)
                throw new ArgumentException($"Cannot create block with size {size}!");

            Entrant = entrant;
            StartingIndex = startingIndex;
            Size = size;
        }

        public int CompareTo(Block block) => StartingIndex.CompareTo(block.StartingIndex);
    }

    private class Blocks : IEnumerable<Block>
    {
        List<Block> blocks;
        public int TotalSize;

        public Blocks(Block block)
        {
            blocks = new List<Block>(1) { block };
            TotalSize = block.Size;
            block.OnSizeChanged += diff => TotalSize += diff;
        }

        public Blocks(List<Block> blocks)
        {
            this.blocks = blocks;
            TotalSize = blocks.Aggregate(0, (n, block) => n + block.Size);
            foreach (var block in blocks)
            {
                block.OnSizeChanged += diff => TotalSize += diff;
            }
        }

        public int Count { get { return blocks.Count; } }
        public int StartingIndex
        {
            get
            {
                if (blocks.Count == 0)
                    return -1;

                return blocks[0].StartingIndex;
            }
        }
        public int LastIndex
        {
            get
            {
                if (blocks.Count == 0)
                    return -1;

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
                    block.Size = index - block.StartingIndex + 1;
                }
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

        public IEnumerator<Block> GetEnumerator()
        {
            foreach (var block in blocks)
            {
                yield return block;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    #endregion

    // List which contains one entry for each ticket
    [DataMember]
    List<T> entrants;

    // Only used to store total number of tickets in each block of entries
    // Actual drawing only happens fromt he entrants list
    Dictionary<T, Blocks> entrantBlocks = new Dictionary<T, Blocks>();
    List<Block> sortedBlocks = new List<Block>();

    // Our own random generator to make seeding possible
    Random rng;

    // Used for batch drawing
    Lottery<T> batchLottery;

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

    public Lottery(Lottery<T> src) : this(src.entrants.Capacity)
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
                Block newBlock = new Block(entrant, entrants.Count, tickets);
                entrantBlocks[entrant].Add(newBlock);
                sortedBlocks.Add(newBlock);
            }
        }
        else
        {
            Block newBlock = new Block(entrant, entrants.Count, tickets);
            entrantBlocks.Add(entrant, new Blocks(newBlock));
            sortedBlocks.Add(newBlock);
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
    public void Remove(T entrant)
    {
        if (!entrantBlocks.ContainsKey(entrant))
            return;

        Remove(entrant, entrantBlocks[entrant].TotalSize);
    }

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
                blocks.RemoveAt(i);
                sortedBlocks.RemoveAt(sortedBlocks.BinarySearch(block));
            }
            else
            {
                block.Size -= remaining;
                lastIndex = block.LastIndex;
                entrants.RemoveRange(block.StartingIndex, remaining);
                break;
            }
        }

        if (blocks.TotalSize == 0)
            entrantBlocks.Remove(entrant);

        RebuildBlocks(lastIndex);
    }
    #endregion

    #region Draw
    /// <summary>
    /// Draws a random entrant from this lottery
    /// </summary>
    /// <returns></returns>
    public T Draw()
    {
        if (entrants.Count == 0)
            throw new LotteryException("Can't draw from empty lottery!");

        if (batchLottery == null)
            return entrants[rng.Next(entrants.Count)];

        // else draw from the batch lottery
        try
        {
            T result = batchLottery.Draw();
            batchLottery.Remove(result);
            return result;
        }
        catch (LotteryException)
        {
            return default(T);
        }
    }

    /// <summary>
    /// Draws n random entrants from this lottery
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public List<T> Draw(int n)
    {
        List<T> winners = new List<T>(n);
        for (int i = 0; i < n; i++)
        {
            winners.Add(Draw());
        }
        return winners;
    }

    /// <summary>
    /// Draws a random entrant from this lottery that is not on the blacklist
    /// </summary>
    public T Draw(HashSet<T> blacklist)
    {
        if (entrants.Count == 0)
            throw new LotteryException("Can't draw from empty lottery!");

        // First get a sorted list of all blocks that aren't on the blacklist
        List<Block> blocks = entrantBlocks.Keys
            .Where(key => !blacklist.Contains(key))
            .Select(key => entrantBlocks[key])
            .SelectMany(list => list)
            .ToList();
        blocks.Sort((lhs, rhs) => lhs.StartingIndex.CompareTo(rhs.StartingIndex));

        // Then create filtered list of entrants based on those blocks
        List<T> filtered = new List<T>(blocks.Aggregate(0, (sum, block) => sum + block.Size));
        foreach (var block in blocks)
        {
            for (int i = 0; i < block.Size; i++)
            {
                filtered.Add(block.Entrant);
            }
        }

        if (filtered.Count == 0)
            return default(T);
        return filtered[rng.Next(filtered.Count)];
    }
    #endregion

    #region Batch Draw
    /// <summary>
    /// Puts this lottery into batch drawing mode. This means each entrant can only be drawn once
    /// </summary>
    public void StartBatchDraw()
    {
        batchLottery = new Lottery<T>(this);
    }

    /// <summary>
    /// Puts this lottery into batch drawing mode with a preexisting blacklist.
    /// </summary>
    public void StartBatchDraw(HashSet<T> blacklist)
    {
        batchLottery = new Lottery<T>(entrants.Count);

        foreach (var e in entrantBlocks.Keys.Where(key => !blacklist.Contains(key)))
        {
            batchLottery.Enter(e, entrantBlocks[e].TotalSize);
        }
    }

    public void EndBatchDraw()
    {
        batchLottery = null;
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
        foreach (var e in entrantBlocks.Keys)
        {
            lottery.Enter(e, entrantBlocks[e].TotalSize);
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

        int index = sortedBlocks.BinarySearch(new Block(default(T), startingIndex, 1));
        if (index < 0)
            index = ~index;

        for (int i = index; i < sortedBlocks.Count; i++)
        {
            int newStart = i > 0 ? sortedBlocks[i - 1].LastIndex : 0;
            sortedBlocks[i].StartingIndex = newStart;
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

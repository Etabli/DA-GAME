using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System.Linq;


/// <summary>
/// Options for how to handle duplicates when entering a Lottery
/// </summary>
public enum EntryOptions
{
    NoChecks,       // Don't perform any checks
    Aggregate,      // Aggregate duplicate entries into a single entry
    Overwrite,      // Overwrite old entries with the new one
    Discard         // Discard entry if one of the same type is already in the lottery
}

/// <summary>
/// A general Exception from mishandling a Lottery
/// </summary>
public class LotteryException : Exception
{
    // Only throw this if it's something seriously dumb, otherwise just log error
    public LotteryException()
    {

    }

    public LotteryException(string message) : base(message)
    { }

    public LotteryException(string message, Exception inner) : base(message, inner)
    { }
}

/// <summary>
/// A lottery for randomly drawing from a pool of objects.
/// </summary>
/// <typeparam name="T">The type of object the lottery is for</typeparam>
[DataContract]
public class Lottery<T>
{
    [DataMember]
    List<Tuple<T, int>> entrants;

    // Our own random generator to make seeding possible
    System.Random rng;
    int maxRoll = 0;

    List<Tuple<T, int>> batchDrawEntrants;
    int batchDrawMaxRoll = 0;

    public Lottery() : this((int)DateTime.Now.Ticks)
    { }

    public Lottery(int seed)
    {
        rng = new System.Random(seed);
        entrants = new List<Tuple<T, int>>();
    }

    public Lottery(Lottery<T> src)
    {
        rng = new System.Random((int)DateTime.Now.Ticks);
        src.CombineInto(this);
    }

    #region BatchDraw
    /// <summary>
    /// Initializes a batch draw with a preexisting blacklist
    /// </summary>
    /// <param name="blacklist"></param>
    public void StartBatchDraw(HashSet<T> blacklist)
    {
        batchDrawEntrants = new List<Tuple<T, int>>(from Tuple<T, int> e in entrants where !blacklist.Contains(e.Item1) select e);
        batchDrawMaxRoll = batchDrawEntrants.Aggregate(0, (sum, e) => sum + e.Item2);
    }

    /// <summary>
    /// Initializes a batch draw
    /// </summary>
    public void StartBatchDraw()
    {
        batchDrawEntrants = new List<Tuple<T, int>>(entrants);
        batchDrawMaxRoll = maxRoll;
    }

    /// <summary>
    /// Draws a batch of winners.
    /// </summary>
    /// <param name="n">The number of winners to draw</param>
    /// <returns>The list of winners. Null if there were no entrants left.</returns>
    public T[] DrawBatch(int n)
    {
        if (batchDrawEntrants == null)
        {
            throw new LotteryException("Trying to BatchDraw without initializing!");
        }

        List<T> batch = new List<T>();
        // Draw n times
        for (; n > 0; n--)
        {
            // Break if we've exhausted all entrants
            if (batchDrawEntrants.Count == 0)
            {
                break;
            }

            int roll = rng.Next(batchDrawMaxRoll);
            foreach (Tuple<T, int> e in batchDrawEntrants)
            {
                if (roll < e.Item2)
                {
                    batch.Add(e.Item1);
                    batchDrawEntrants.Remove(e);
                    batchDrawMaxRoll -= e.Item2;
                    break;
                }
                roll -= e.Item2;
            }
        }

        if (batch.Count > 0)
        {
            return batch.ToArray();
        }
        return null;
    }

    /// <summary>
    /// Draws a single winner from the batch-drawing list.
    /// </summary>
    /// <returns></returns>
    public T DrawBatch()
    {
        T[] batch = DrawBatch(1);
        if (batch == null)
            return default(T);
        return batch[0];
    }

    /// <summary>
    /// Ends a batch draw, so that accidental draws without restarting throw an error
    /// </summary>
    public void EndBatchDraw()
    {
        batchDrawEntrants = null;
        batchDrawMaxRoll = 0;
    }
    #endregion

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

    /// <summary>
    /// Enters all entrants of this lottery into another lottery
    /// </summary>
    /// <param name="lottery">The lottery to be entered into</param>
    /// <param name="options">Strategy to use for duplicate entries</param>
    public void CombineInto(Lottery<T> lottery, EntryOptions options)
    {
        foreach (Tuple<T, int> e in entrants)
        {
            lottery.Enter(e.Item1, e.Item2, options);
        }
    }

    /// <summary>
    /// Enters all entrants of this lottery into another lottery. Uses Aggregate option
    /// </summary>
    /// <param name="lottery"></param>
    public void CombineInto(Lottery<T> lottery)
    {
        CombineInto(lottery, EntryOptions.Aggregate);
    }

    /// <summary>
    /// Combines this lottery with another one and returns the result
    /// </summary>
    /// <param name="lottery">The lottery to combine this one with</param>
    /// <returns>The combined lottery</returns>
    public Lottery<T> CombineWith(Lottery<T> lottery, EntryOptions options)
    {
        Lottery<T> result = new Lottery<T>();
        CombineInto(result, options);
        lottery.CombineInto(result, options);
        return result;
    }

    public Lottery<T> CombineWith(Lottery<T> lottery)
    {
        return CombineWith(lottery, EntryOptions.Aggregate);
    }

    public static Lottery<T> Combine(Lottery<T> lottery1, Lottery<T> lottery2, EntryOptions options)
    {
        return lottery1.CombineWith(lottery2, options);
    }

    public static Lottery<T> Combine(Lottery<T> lottery1, Lottery<T> lottery2)
    {
        return Combine(lottery1, lottery2, EntryOptions.Aggregate);
    }

    /// <summary>
    /// Enters an object into the lottery
    /// </summary>
    /// <param name="entrant">The object to be entered into the lottery</param>
    /// <param name="nTickets">The number of tickets to assign to the object</param>
    /// <param name="options">Options regarding what to do when a duplicate is found</param>
    public void Enter(T entrant, int nTickets, EntryOptions options)
    {
        // Check for duplicates if we care about them
        if (options != EntryOptions.NoChecks)
        {
            Tuple<T, int> duplicate = GetEntrant(entrant);

            // We found a duplicate
            if (duplicate != null)
            {
                if (options == EntryOptions.Discard)
                {
                    // Do nothing
                    return;
                }
                else if (options == EntryOptions.Aggregate)
                {
                    // Remove duplicate and update ticket amount
                    nTickets += duplicate.Item2;
                    Remove(entrant);
                }
                else if (options == EntryOptions.Overwrite)
                {
                    // Simply remove duplicate
                    Remove(entrant);
                }
            }
        }             

        maxRoll += nTickets;
        entrants.Add(new Tuple<T, int>(entrant, nTickets));
    }

    public void Enter(T entrant, int nTickets)
    {
        Enter(entrant, nTickets, EntryOptions.Aggregate);
    }

    public void Enter(T entrant)
    {
        Enter(entrant, 1, EntryOptions.Aggregate);
    }

    public T GetWinner(HashSet<T> blacklist)
    {
        Tuple<T, int>[] entrants_copy = new Tuple<T, int>[entrants.Count];
        entrants.CopyTo(entrants_copy);

        var filtered_entrants = entrants.ToArray();
        int max = maxRoll;

        // Only filter if we actually have a blacklist
        if (blacklist.Count > 0)
        {
            filtered_entrants = (from Tuple<T, int> e in entrants where !blacklist.Contains(e.Item1) select e).ToArray();
            max = filtered_entrants.Aggregate(0, (sum, e) => sum + e.Item2);
        }    

        T winner = default(T);
        int roll = rng.Next(max);
        foreach (Tuple<T, int> entrant in filtered_entrants)
        {
            if (roll < entrant.Item2)
            {
                winner = entrant.Item1;
                break;
            }
            roll -= entrant.Item2;
        }
        return winner;
    }

    public T GetWinner()
    {
        return GetWinner(new HashSet<T>());
    }

    public T Draw()
    {
        return GetWinner();
    }

    public T Draw(HashSet<T> blacklist)
    {
        return GetWinner(blacklist);
    }

    public void Remove(T entrant)
    {
        entrants.RemoveAll(t => {
            if (t.Item1.Equals(entrant))
            {
                maxRoll -= t.Item2;
                return true;
            }
            return false;
        });
    }

    public override string ToString()
    {
        string result = string.Format("Lottery of type '{0}' with {1} entrants:\n", typeof(T), entrants.Count);
        foreach(Tuple<T, int> e in entrants)
        {
            result += string.Format("{0}: {1}\n", e.Item1, e.Item2);
        }
        return result;
    }

    /// <summary>
    /// Returns a proper entrant for a given object, if there is one
    /// </summary>
    private Tuple<T, int> GetEntrant(T value)
    {
        Tuple<T, int> result = null;
        foreach(Tuple<T, int> e in entrants)
        {
            if (e.Item1.Equals(value))
            {
                result = e;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// Returns all entrants for a given object, if there are any
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private Tuple<T, int>[] GetEntrants(T value)
    {
        List<Tuple<T, int>> result = new List<Tuple<T, int>>();

        foreach (Tuple<T, int> e in entrants)
        {
            if (e.Item1.Equals(value))
            {
                result.Add(e);
            }
        }

        return result.ToArray();
    }
}

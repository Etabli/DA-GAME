using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

/// <summary>
/// Options for how to handle duplicates when entering a Lottery
/// </summary>
public enum EntryOptions
{
    NoChecks,
    Aggregate,
    Overwrite,
    Discard
}

/// <summary>
/// A lottery for randomly drawing from a pool of objects.
/// </summary>
/// <typeparam name="T">The type of object the lottery is for</typeparam>
[DataContract]
public class Lottery<T>
{
    [DataMember]
    List<Tuple<T, int>> entrees;

    // Our own random generator to make seeding possible
    System.Random rng;
    int maxRoll = 0;

    public Lottery() : this((int)DateTime.Now.Ticks)
    { }

    public Lottery(int seed)
    {
        rng = new System.Random(seed);
        entrees = new List<Tuple<T, int>>();
    }

    public Lottery(Lottery<T> src)
    {
        rng = new System.Random((int)DateTime.Now.Ticks);
        src.CombineInto(this);
    }

    public void ChangeSeed(int seed)
    {
        rng = new System.Random(seed);
    }

    /// <summary>
    /// Enters all entrees of this lottery into another lottery
    /// </summary>
    /// <param name="lottery">The lottery to be entered into</param>
    public void CombineInto(Lottery<T> lottery, EntryOptions options)
    {
        foreach (Tuple<T, int> e in entrees)
        {
            lottery.Enter(e.Item1, e.Item2, options);
        }
    }

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
    /// <param name="entree">The object to be entered into the lottery</param>
    /// <param name="nTickets">The number of tickets to assign to the object</param>
    /// <param name="options">Options regarding what to do when a duplicate is found</param>
    public void Enter(T entree, int nTickets, EntryOptions options)
    {
        // Check for duplicates if we care about them
        if (options != EntryOptions.NoChecks)
        {
            Tuple<T, int> duplicate = GetEntree(entree);

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
                    Remove(entree);
                }
                else if (options == EntryOptions.Overwrite)
                {
                    // Simply remove duplicate
                    Remove(entree);
                }
            }
        }             

        maxRoll += nTickets;
        entrees.Add(new Tuple<T, int>(entree, nTickets));
    }

    public void Enter(T entree, int nTickets)
    {
        Enter(entree, nTickets, EntryOptions.Aggregate);
    }

    public void Enter(T entree)
    {
        Enter(entree, 1, EntryOptions.Aggregate);
    }

    public T GetWinner()
    {
        T winner = default(T);
        int roll = rng.Next(maxRoll);
        foreach (Tuple<T, int> entree in entrees)
        {
            if (roll < entree.Item2)
            {
                winner = entree.Item1;
                break;
            }
            roll -= entree.Item2;
        }
        return winner;
    }

    public void Remove(T entree)
    {
        entrees.RemoveAll(t => {
            if (t.Item1.Equals(entree))
            {
                maxRoll -= t.Item2;
                return true;
            }
            return false;
        });
    }

    public override string ToString()
    {
        string result = string.Format("Lottery of type '{0}':\n", typeof(T));
        foreach(Tuple<T, int> e in entrees)
        {
            result += string.Format("{0}: {1}\n", e.Item1, e.Item2);
        }
        return result;
    }

    /// <summary>
    /// Returns a proper entree for a given object, if there is one
    /// </summary>
    private Tuple<T, int> GetEntree(T value)
    {
        Tuple<T, int> result = null;
        foreach(Tuple<T, int> e in entrees)
        {
            if (e.Item1.Equals(value))
            {
                result = e;
                break;
            }
        }
        return result;
    }
}

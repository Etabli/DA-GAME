using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

[DataContract]
public class Lottery<T>
{
    [DataMember]
    List<Tuple<T, int>> entrees;
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

    public void CombineInto(Lottery<T> lottery)
    {
        foreach (Tuple<T, int> e in entrees)
        {
            lottery.Enter(e.Item1, e.Item2);
        }
        lottery.maxRoll += maxRoll;
    }

    public Lottery<T> CombineWith(Lottery<T> lottery)
    {
        Lottery<T> result = new Lottery<T>();
        CombineInto(result);
        lottery.CombineInto(result);
        return result;
    }

    public void Enter(T entree, int nTickets)
    {
        maxRoll += nTickets;
        entrees.Add(new Tuple<T, int>(entree, nTickets));
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
}

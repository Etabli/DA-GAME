using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lottery<T>
{
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
}

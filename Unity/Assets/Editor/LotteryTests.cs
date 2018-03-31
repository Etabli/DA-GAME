using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;

using Random = System.Random;

public class LotteryTests
{
    private class TestReference { }

    [Test]
    public void LotterySimpleDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(3);

        Assert.That(lottery.Draw() == 3);
    }

    [Test]
    public void LotterySimpleEnterTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(1);
        Assert.That(lottery.GetTickets(1) == 1);
    }

    [Test]
    public void LotteryEnterAggregateTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(3, 2);
        lottery.Enter(3, 3);
        lottery.Enter(3, 1);
        Assert.That(lottery.GetTickets(3) == 6);
    }

    [Test]
    public void LotteryRandom10000EnterTest()
    {
        Lottery<int> lottery = new Lottery<int>(10000);
        Random rng = new Random();

        Dictionary<int, int> tickets = new Dictionary<int, int>(10000);
        for (int i = 0; i < 10000; i++)
        {
            int entrant = rng.Next();
            int n = rng.Next() % 99 + 1;
            lottery.Enter(entrant, n);

            if (tickets.ContainsKey(entrant))
                tickets[entrant] += n;
            else
                tickets.Add(entrant, n);
        }

        foreach (var entrant in tickets.Keys)
        {
            Assert.That(tickets[entrant] == lottery.GetTickets(entrant));
        }
    }

    [Test]
    public void LotterySimpleRemoveTest()
    {
        Lottery<int> lottery = new Lottery<int>(1);
        lottery.Enter(2, 5);
        lottery.Remove(2, 2);
        Assert.That(lottery.GetTickets(2) == 3);
    }

    [Test]
    public void LotteryRemoveTest()
    {
        Lottery<int> lottery = new Lottery<int>(3);
        lottery.Enter(1, 5);
        lottery.Enter(2, 10);
        lottery.Enter(3, 3);

        lottery.Remove(2, 4);
        Assert.That(lottery.GetTickets(2) == 6);
        lottery.Remove(2);
        Assert.That(lottery.GetTickets(2) == 0);
        lottery.Remove(1, 2);
        Assert.That(lottery.GetTickets(1) == 3);
        Assert.AreEqual(3, lottery.GetTickets(3));
    }
}

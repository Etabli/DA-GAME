using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;

using Random = System.Random;

public class LotteryTests
{
    private class TestReference  : IEquatable<TestReference>
    {
        public bool Equals(TestReference obj)
        {
            return ReferenceEquals(this, obj);
        }
    }

    [Test]
    public void LotterySimpleDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(3);

        Assert.That(lottery.Draw() == 3);
    }

    [Test]
    public void LotteryReferenceDrawTest()
    {
        Lottery<TestReference> lottery = new Lottery<TestReference>();
        var reference = new TestReference();
        lottery.Enter(reference, 10);

        Assert.That(ReferenceEquals(reference, lottery.Draw()));
    }

    [Test]
    public void LotteryRemoveDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(1, 2);
        lottery.Enter(2, 10);
        lottery.Remove(1, 1);
        lottery.Remove(2);
        Assert.AreEqual(1, lottery.Draw());
    }

    [Test]
    public void LotteryBlacklistDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>();
        lottery.Enter(1);
        lottery.Enter(2);
        lottery.Enter(3);
        lottery.Enter(4);

        int result = lottery.Draw(new HashSet<int> { 1, 3, 4 });
        Assert.AreEqual(2, result);
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
    public void Lottery10000EnterTest()
    {
        Lottery<int> lottery = new Lottery<int>(1000000);
        Random rng = new Random();

        Dictionary<int, int> tickets = new Dictionary<int, int>(10000);
        for (int i = 0; i < 10000; i++)
        {
            int entrant = rng.Next();
            int n = rng.Next(100) + 1;
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
    public void Lottery10000AggregateEnterTest()
    {
        Lottery<int> lottery = new Lottery<int>(1000000);
        Random rng = new Random();

        Dictionary<int, int> tickets = new Dictionary<int, int>(10000);
        for (int i = 0; i < 10000; i++)
        {
            int entrant = rng.Next(100);
            int n = rng.Next(100) + 1;
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
        Assert.AreEqual(3, lottery.GetTickets(2));
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

    [Test]
    public void LotterySimpleBatchDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>(1);
        lottery.Enter(5, 20);
        lottery.Enter(3, 10);

        lottery.StartBatchDraw();
        var result = lottery.Draw(3);

        Assert.That(result[0] == 5 && result[1] == 3
                    || result[0] == 3 && result[1] == 5);
        Assert.AreEqual(default(int), result[2]);
    }

    [Test]
    public void LotteryRandom10000BatchDrawTest()
    {
        Lottery<int> lottery = new Lottery<int>(1000000);
        for (int i = 0; i < 10000; i++)
        {
            lottery.Enter(i);
        }

        lottery.StartBatchDraw();
        var result = lottery.Draw(10000);

        HashSet<int> set = new HashSet<int>();
        foreach (var item in result)
        {
            Assert.That(!set.Contains(item));
            set.Add(item);
        }
    }
}

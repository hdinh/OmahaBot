namespace OmahaBot.Tests.Analysis
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using NUnit.Framework;
    using OmahaBot.Analysis;
    using OmahaBot.Core;
    using OmahaBot.Utilities;
    using UnitTestUtil;
    using System.Diagnostics;

    [TestFixture]
    public class HandFutureOddsTests
    {
        [Test]
        public void Calculate_PlayerHaveHighNuts_OddsShouldReturn100HighWinPercentage()
        {
            Card[] hands = CardHelper.CreateHandFromString("7c 8c 9c 10c");
            Card[] common = CardHelper.CreateHandFromString("2c 3c 4c 5c 6c");
            double[][] odds = HandFutoreOdds.Calculate(hands, common, 2, 100);

            Assert.AreEqual(1.0, odds[0][0]);
        }

        [Test]
        public void Calculate_PlayerHaveWheel_OddsShouldReturn0LowLosePercentage()
        {
            Card[] hands = CardHelper.CreateHandFromString("As 3s 4s 5s");
            Card[] common = CardHelper.CreateHandFromString("2c 3c 4c 5c 6c");
            double[][] odds = HandFutoreOdds.Calculate(hands, common, 3);

            Assert.AreEqual(0, odds[1][1]);
        }

        [Test]
        public void Calculate_NutHighNutLow_DeclareCorrectWinner()
        {
            Card[][] hands = new Card[3][];

            hands[0] = CardHelper.CreateHandFromString("8s 9s 10s Js");
            hands[1] = CardHelper.CreateHandFromString("8c 9c 10c Jc");
            hands[2] = CardHelper.CreateHandFromString("As 2s 3s 4s");

            Card[] common = CardHelper.CreateHandFromString("2c 3c 4c 5c 6c");
            double[][][] odds = HandFutoreOdds.Calculate(hands, common);

            Assert.AreEqual(odds[0][0][0], 0);
            Assert.AreEqual(odds[0][1][1], 1);
            Assert.AreEqual(odds[1][0][0], 1);
            Assert.AreEqual(odds[1][1][1], 1);
            Assert.AreEqual(odds[1][1][1], 1);
            Assert.AreEqual(odds[2][0][0], 0);
            Assert.AreEqual(odds[2][1][0], 1);
        }

        [Test]
        public void Calculate_NutHighNutLow_DeclareCorrecadsfasdftWinner()
        {
            Card[][] hands = new Card[2][];

            hands[0] = CardHelper.CreateHandFromString("As 2s 7s Qh");
            hands[1] = CardHelper.CreateHandFromString("Ad 3d 6s 7s");

            Card[] common = CardHelper.CreateHandFromString("4c 5c Qc");
            double[][][] odds = HandFutoreOdds.Calculate(hands, common);
        }

        [Test]
        public void Calculate_FourEqualHands_ShouldAllBeIdentical()
        {
            Card[][] hands = new Card[4][];

            hands[0] = CardHelper.CreateHandFromString("As 2c 3h 4d");
            hands[1] = CardHelper.CreateHandFromString("Ac 2d 3s 4h");
            hands[2] = CardHelper.CreateHandFromString("Ad 2h 3c 4s");
            hands[3] = CardHelper.CreateHandFromString("Ah 2s 3d 4c");

            Card[] common = CardHelper.CreateHandFromString(string.Empty);
            double[][][] odds = HandFutoreOdds.Calculate(hands, common);

            for (int playerA = 0; playerA < 4; playerA++)
            {
                for (int playerB = 0; playerB < 4; playerB++)
                {
                    for (int highOrLow = 0; highOrLow < 2; highOrLow++)
                    {
                        for (int winLoseDraw = 0; winLoseDraw < 3; winLoseDraw++)
                        {
                            double aodd = odds[playerA][highOrLow][winLoseDraw];
                            double bodd = odds[playerA][highOrLow][winLoseDraw];

                            Assert.AreEqual(aodd, bodd);
                        }
                    }
                }
            }
        }

        [Test]
        [Ignore]
        public void Futures_SetAPerceivedHandStregthProperty()
        {
            ////double[][][] odds = HandFutoreOdds.Calculate(hands, new double[] {0.35, 0.88, 0.77}, common);
        }

        [Test]
        [Ignore]
        public void Futures_GetAValueRankFromASetOfCards()
        {
            ////HandFutoreOdds.GetValue(Card.CreateHandFromString("As 2s 3s 4s"), 8);
        }

        [Test]
        [Ignore]
        public void Futures_SimulateAHandInTheFuture()
        {
            ////Hand hand = new OmahaHand();
            ////double averageAmountOfValue = hand.Simulate(100);
        }

        [Test]
        [Explicit]
        [CoverageExclude]
        public void PrintTheBestHands()
        {
            int numIterations = 1500;

            Semaphore maxThreads = new Semaphore(0, Environment.ProcessorCount);
            maxThreads.Release(Environment.ProcessorCount);

            for (int numPlayers = 3; numPlayers < 9; numPlayers++)
            {
                Dictionary<ulong, double> caculations = new Dictionary<ulong, double>();

                Random r = new Random();

                List<Thread> threads = new List<Thread>();

                Stopwatch sw = new Stopwatch();
                sw.Start();

                for (int a = 0; a < 49; a++)
                {
                    int aa = a;
                    maxThreads.WaitOne();

                    Thread executeThread = new Thread(
                        delegate()
                        {
                            for (int b = aa + 1; b < 50; b++)
                            {
                                for (int c = b + 1; c < 51; c++)
                                {
                                    for (int d = c + 1; d < 52; d++)
                                    {
                                        Card[] hand = new Card[4];
                                        hand[0] = new Card(aa);
                                        hand[1] = new Card(b);
                                        hand[2] = new Card(c);
                                        hand[3] = new Card(d);

                                        ulong hash = OmahaHandHash.GetHashCode(hand);

                                        if (!caculations.ContainsKey(hash))
                                        {
                                            double[][] odds = HandFutoreOdds.Calculate(hand, new Card[0], numPlayers, numIterations);

                                            double hi = odds[0][0] + odds[0][2];
                                            double lo = odds[1][0] + odds[1][2];
                                            double wintage = hi + lo;

                                            lock (caculations)
                                            {
                                                if (!caculations.ContainsKey(hash))
                                                {
                                                    caculations.Add(hash, wintage);
                                                }

                                                if (caculations.Count % 400 == 0)
                                                {
                                                    Console.WriteLine("{0} : {1:F3} ({2:F3}, {3:F3})  - {4:F4} {5}", OmahaHandHash.ToString(hash), wintage, hi, lo, caculations.Count / 16718.0, sw.Elapsed.TotalSeconds);
                                                    Console.WriteLine("Estimated: {0}", DateTime.Now.AddMilliseconds(((1 / (caculations.Count / 16718.0)) - 1) * sw.Elapsed.TotalMilliseconds));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            maxThreads.Release();
                        });
                    threads.Add(executeThread);
                    executeThread.Start();
                }

                foreach (Thread thread in threads)
                {
                    thread.Join();
                }

                sw.Stop();

                List<Pair<ulong, double>> calculationsInList = new List<Pair<ulong, double>>();
                foreach (ulong key in caculations.Keys)
                {
                    calculationsInList.Add(new Pair<ulong, double>(key, caculations[key]));
                }

                calculationsInList.Sort(
                    delegate(Pair<ulong, double> a, Pair<ulong, double> b)
                    {
                        if (a.Second > b.Second)
                        {
                            return -1;
                        }
                        else if (b.Second > a.Second)
                        {
                            return 1;
                        }

                        return 0;
                    });

                for (int i = 0; i < 10; i++)
                {
                    Console.WriteLine();
                }

                Console.WriteLine("=============================================================\n");
                Console.WriteLine(" Total time   : " + sw.Elapsed.TotalSeconds);
                Console.WriteLine(" DateTime.Now : " + DateTime.Now);

                for (int i = 0; i < calculationsInList.Count; i++)
                {
                    Console.WriteLine("{0:D4}: {1:F3} : {2}", i, calculationsInList[i].Second, OmahaHandHash.ToString(calculationsInList[i].First));
                }

                File.WriteAllBytes(string.Format("HAND_OODS_{0}_{1}.bin", numPlayers, numIterations), BinaryCloneable.ToByte(caculations));
            }
        }
    }
}


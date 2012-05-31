namespace OmahaBot.Analysis
{
    using OmahaBot.Core;
    using System.Collections.Generic;
    using System;

    public static class HandFutoreOdds
    {
        private static readonly int DefaultNumberOfIterations = 1000;
        private static readonly int NumCardsInOmaha = 4;

        public static double[][] Calculate(Card[] hands, Card[] common, int numOpponents)
        {
            return Calculate(hands, common, numOpponents, DefaultNumberOfIterations);
        }

        public static double[][] Calculate(Card[] hands, Card[] common, int numOpponents, int numIterations)
        {
            Card[][] handsNew = new Card[numOpponents + 1][];
            handsNew[0] = hands;
            double[][][] odds = Calculate(handsNew, common, numOpponents + 1, numIterations);
            return odds[0];
        }

        public static double[][][] Calculate(Card[][] hands, Card[] common)
        {
            return Calculate(hands, common, hands.Length);
        }

        public static double[][][] Calculate(Card[][] hands, Card[] common, int numOpponents)
        {
            return Calculate(hands, common, numOpponents, DefaultNumberOfIterations);
        }

        public static double[][][] Calculate(Card[][] hands, Card[] common, int numOpponents, int numIterations)
        {
            double[] numWinsHigh = new double[numOpponents];
            double[] numTiesHigh = new double[numOpponents];

            double[] numWinsLow = new double[numOpponents];
            double[] numTiesLow = new double[numOpponents];
            int numNonLow = 0; //// used in caculating loses

            Random random = new Random();

            Deck original = new Deck();

            for (int i = 0; i < hands.Length; i++)
            {
                if (hands[i] != null)
                {
                    original.SetDead(hands[i]);
                }
                else
                {
                    hands[i] = new Card[NumCardsInOmaha];
                }
            }
            original.SetDead(common);

            for (int i = 0; i < numIterations; i++)
            {
                Deck deck = (Deck)original.Clone();
                deck.Seed = random.Next();
                Card[][] playerCards = new Card[numOpponents][];

                // Copy common
                Card[] commonCopy = new Card[5];
                Array.Copy(common, commonCopy, common.Length);

                // Copy all player hands
                for (int j = 0; j < numOpponents; j++)
                {
                    playerCards[j] = new Card[NumCardsInOmaha];

                    if (j < hands.Length)
                    {
                        Array.Copy(hands[j], playerCards[j], NumCardsInOmaha);
                    }

                    // Finish the player hand if necessary
                    for (int k = 0; k < NumCardsInOmaha; k++)
                    {
                        if (playerCards[j][k] == null)
                        {
                            playerCards[j][k] = deck.Deal();
                        }
                    }
                }

                // Finish the common cards if necessary
                for (int j = 0; j < 5; j++)
                {
                    if (commonCopy[j] == null)
                    {
                        commonCopy[j] = deck.Deal();
                    }
                }

                uint bestHighValue = 0;
                uint bestLowValue = 0;
                List<int> bestHighIndex = new List<int>();
                List<int> bestLowIndex = new List<int>();

                for (int j = 0; j < numOpponents; j++)
                {
                    uint oppHighValue = OmahaHandHighEvaluator.Evaluate(playerCards[j], commonCopy);
                    uint oppLowValue = OmahaHandLowEvaluator.Evaluate(playerCards[j], commonCopy);

                    if (oppHighValue > bestHighValue)
                    {
                        bestHighValue = oppHighValue;
                        bestHighIndex.Clear();
                        bestHighIndex.Add(j);
                    }
                    else if (oppHighValue == bestHighValue)
                    {
                        bestHighIndex.Add(j);
                    }

                    if (oppLowValue > bestLowValue)
                    {
                        bestLowValue = oppLowValue;
                        bestLowIndex.Clear();
                        bestLowIndex.Add(j);
                    }
                    else if (oppLowValue == bestLowValue && oppLowValue > 0)
                    {
                        bestLowIndex.Add(j);
                    }
                }

                double lowExistsMultiple = 1.0;
                bool normalized = false;
                if (normalized)
                {
                    if (bestLowValue == 0)
                    {
                        lowExistsMultiple = 0.5;
                    }
                }

                for (int tiedIndex = 0; tiedIndex < bestHighIndex.Count; tiedIndex++)
                {
                    if (normalized)
                    {
                        numTiesHigh[bestHighIndex[tiedIndex]] += lowExistsMultiple / bestHighIndex.Count;
                    }
                    else
                    {
                        numTiesHigh[bestHighIndex[tiedIndex]] += lowExistsMultiple;
                    }
                }

                if (bestLowValue == 0)
                {
                    numNonLow++;
                }
                else
                {
                    for (int tiedIndex = 0; tiedIndex < bestLowIndex.Count; tiedIndex++)
                    {
                        if (normalized)
                        {
                            numTiesLow[bestLowIndex[tiedIndex]] += lowExistsMultiple / bestLowIndex.Count;
                        }
                        else
                        {
                            numTiesLow[bestLowIndex[tiedIndex]] += lowExistsMultiple;
                        }
                    }
                }
            }

            double numIterationsCache = (double)numIterations;
            double[][][] results = new double[numOpponents][][];

            for (int i=0; i<numOpponents; i++)
            {
                results[i] = new double[][]
                    {
                        new double[]
                        {
                            numWinsHigh[i] / numIterationsCache,
                            1.0 - ((numWinsHigh[i] + numTiesHigh[i]) / numIterationsCache),
                            numTiesHigh[i] / numIterationsCache
                        },
                        new double[]
                        {
                            numWinsLow[i] / numIterationsCache,
                            1.0 - ((numWinsLow[i] + numTiesLow[i]) / (numIterationsCache )) - (numNonLow / numIterationsCache),
                            numTiesLow[i] / numIterationsCache
                        }
                    };
            }

            return results;
        }
    }
}

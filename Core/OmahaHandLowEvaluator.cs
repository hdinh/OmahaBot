namespace OmahaBot.Core
{
    using System;

    public static class OmahaHandLowEvaluator
    {
        [CLSCompliant(false)]
        public static uint Evaluate(Card[] hand, Card[] common)
        {
            uint best = 0;

            for (int h0 = 0; h0 < hand.Length; h0++)
            {
                for (int h1 = h0 + 1; h1 < hand.Length; h1++)
                {
                    for (int c0 = 0; c0 < common.Length; c0++)
                    {
                        for (int c1 = c0 + 1; c1 < common.Length; c1++)
                        {
                            for (int c2 = c1 + 1; c2 < common.Length; c2++)
                            {
                                int[] dups = new int[] { hand[h0].Rank, hand[h1].Rank, common[c0].Rank, common[c1].Rank, common[c2].Rank };
                                Array.Sort(dups);

                                if (dups[4] <= HoldemHand.Hand.Rank8 || (dups[3] <= HoldemHand.Hand.Rank8 && dups[4] == HoldemHand.Hand.RankAce))
                                {
                                    bool duplicate = false;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        if (dups[i] == dups[i + 1])
                                        {
                                            duplicate = true;
                                        }
                                    }

                                    if (!duplicate)
                                    {
                                        uint value = 0;

                                        for (int i = 0; i < 5; i++)
                                        {
                                            switch (dups[i])
                                            {
                                                case 12:
                                                    value |= 1 << 8;
                                                    break;
                                                case 0:
                                                    value |= 1 << 7;
                                                    break;
                                                case 1:
                                                    value |= 1 << 6;
                                                    break;
                                                case 2:
                                                    value |= 1 << 5;
                                                    break;
                                                case 3:
                                                    value |= 1 << 4;
                                                    break;
                                                case 4:
                                                    value |= 1 << 3;
                                                    break;
                                                case 5:
                                                    value |= 1 << 2;
                                                    break;
                                                case 6:
                                                    value |= 1 << 1;
                                                    break;
                                                case 7:
                                                    break;
                                            }
                                        }

                                        if (value > best)
                                        {
                                            best = value;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return best;
        }
    }
}

namespace OmahaBot.Core
{
    using System;

    [CLSCompliant(false)]
    public static class OmahaHandHighEvaluator
    {
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
                                ulong mask = 0;
                                mask |= 1UL << ToMask(hand[h0]);
                                mask |= 1UL << ToMask(hand[h1]);
                                mask |= 1UL << ToMask(common[c0]);
                                mask |= 1UL << ToMask(common[c1]);
                                mask |= 1UL << ToMask(common[c2]);
                                uint eval = HoldemHand.Hand.Evaluate(mask);

                                if (eval > best)
                                {
                                    best = eval;
                                }
                            }
                        }
                    }
                }
            }

            return best;
        }

        private static int ToMask(Card card)
        {
            return (card.Suit * 13) + card.Rank;
        }
    }
}

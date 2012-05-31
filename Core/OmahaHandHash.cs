namespace OmahaBot.Core
{
    using System;
    using System.Collections.Generic;

    public static class OmahaHandHash
    {
        private static readonly char[] RankChar =
        {
            '2', '3', '4', '5', '6', '7', '8', '9', 'T', 'J', 'Q', 'K', 'A',
        };

        [CLSCompliant(false)]
        public static ulong GetHashCode(Card[] hand)
        {
            List<Card> hcopy = new List<Card>(hand);
            ulong hash = 0;

            hcopy.Sort(delegate(Card a, Card b)
            {
                if (a.Rank > b.Rank)
                {
                    return -1;
                }
                else if (b.Rank > a.Rank)
                {
                    return 1;
                }
                else
                {
                    if (a.Suit > b.Suit)
                    {
                        return -1;
                    }
                    else if (b.Suit > a.Suit)
                    {
                        return 1;
                    }

                    return 0;
                }
            });

            short[] numSuit = new short[4];

            for (int j = 0; j < 4; j++)
            {
                numSuit[hcopy[j].Suit]++;
            }

            int hashedQuirk = 1;

            // Number of repeated suits (going backware, 4-3-2-1)
            for (int i = 4; i > 0; i--)
            {
                for (int q = 0; q < hcopy.Count; q++)
                {
                    // Check if the top rank has that number of suits
                    if (numSuit[hcopy[q].Suit] == i)
                    {
                        int suit = hcopy[q].Suit;

                        List<Card> toRemove = new List<Card>();

                        // Get all the cards that have this suit
                        for (int k = 0; k < hcopy.Count; k++)
                        {
                            if (hcopy[k].Suit == hcopy[q].Suit)
                            {
                                // Push that rank into the lowest quirk
                                hash |= 1UL << (((hashedQuirk - 1) * 13) + hcopy[k].Rank);
                                toRemove.Add(hcopy[k]);
                            }
                        }

                        for (int p = 0; p < toRemove.Count; p++)
                        {
                            hcopy.Remove(toRemove[p]);
                        }

                        numSuit[suit] = 0; //// Ensure this doesn't get used again
                        hashedQuirk++; //// The next suit will use the next hashedQuirk

                        i++;
                        break; // We break here because q is now compromised, just revisit i again
                    }
                }
            }

            return hash;
        }

        [CLSCompliant(false)]
        public static string ToString(ulong omahaHandHash)
        {
            List<string> tostringed = new List<string>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    if ((omahaHandHash & (1UL << ((i * 13) + j))) > 0)
                    {
                        string temp = string.Empty;
                        temp += RankChar[j];
                        temp += (char)('a' + i);
                        tostringed.Add(temp);
                    }
                }
            }

            string result = string.Empty;
            for (int i = 0; i < 3; i++)
            {
                result += tostringed[i] + " ";
            }

            result += tostringed[3];

            return result;
        }
    }
}

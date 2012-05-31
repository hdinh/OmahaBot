namespace UnitTestUtil
{
    using System.Collections.Generic;
    using OmahaBot.Core;

    public static class CardHelper
    {
        public static Card[] CreateHandFromString(string handStr)
        {
            string[] handStrs = handStr.Split();
            List<Card> hand = new List<Card>();

            for (int i = 0; i < handStrs.Length; i++)
            {
                if (handStrs[i].Length != 0)
                {
                    hand.Add(new Card(handStrs[i]));
                }
            }

            return hand.ToArray();
        }
    }
}

namespace UnitTestUtil
{
    using NUnit.Framework;
    using OmahaBot.Core;

    public static class OmahaEvalAssert
    {
        public static void AreEqual(string hand, string common, string expected)
        {
            Card[] handCards = CardHelper.CreateHandFromString(hand);
            Card[] commonCards = CardHelper.CreateHandFromString(common);

            uint exp = HoldemHand.Hand.Evaluate(expected);
            uint best5 = OmahaHandHighEvaluator.Evaluate(handCards, commonCards);

            Assert.AreEqual(exp, best5);
        }
    }
}

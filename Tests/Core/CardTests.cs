namespace OmahaBot.Tests.Core.Core
{
    using NUnit.Framework;
    using OmahaBot.Core;

    [TestFixture]
    public class CardTests
    {
        private readonly int[,] ExpectedSuitAndRank = new int[,]
        {
            { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 }, { 4, 0 }, { 5, 0 }, { 6, 0 }, { 7, 0 }, { 8, 0 }, { 9, 0 }, { 10, 0 }, { 11, 0 }, { 12, 0 },
            { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 1 }, { 8, 1 }, { 9, 1 }, { 10, 1 }, { 11, 1 }, { 12, 1 },
            { 0, 2 }, { 1, 2 }, { 2, 2 }, { 3, 2 }, { 4, 2 }, { 5, 2 }, { 6, 2 }, { 7, 2 }, { 8, 2 }, { 9, 2 }, { 10, 2 }, { 11, 2 }, { 12, 2 },
            { 0, 3 }, { 1, 3 }, { 2, 3 }, { 3, 3 }, { 4, 3 }, { 5, 3 }, { 6, 3 }, { 7, 3 }, { 8, 3 }, { 9, 3 }, { 10, 3 }, { 11, 3 }, { 12, 3 }
        };

        private static readonly string[] ExpectedString =
        {
            "2c", "3c", "4c", "5c", "6c", "7c", "8c", "9c", "Tc", "Jc", "Qc", "Kc", "Ac",
            "2d", "3d", "4d", "5d", "6d", "7d", "8d", "9d", "Td", "Jd", "Qd", "Kd", "Ad",
            "2h", "3h", "4h", "5h", "6h", "7h", "8h", "9h", "Th", "Jh", "Qh", "Kh", "Ah",
            "2s", "3s", "4s", "5s", "6s", "7s", "8s", "9s", "Ts", "Js", "Qs", "Ks", "As",
        };

        [Test]
        public void Card_FromIndexConstructor_VerifyRanksAndSuits()
        {
            for (int i = 0; i < 52; i++)
            {
                Card card = new Card(i);

                Assert.AreEqual(ExpectedSuitAndRank[i, 0], card.Rank);
                Assert.AreEqual(ExpectedSuitAndRank[i, 1], card.Suit);
            }
        }

        [Test]
        public void Card_FromCardRankConstructor_VerifyRanksAndSuits()
        {
            for (int i = 0; i < 52; i++)
            {
                Card card = new Card(10, 2);

                Assert.AreEqual(10, card.Rank);
                Assert.AreEqual(2, card.Suit);
            }
        }

        [Test]
        public void Card_FromConstructor_VerifyToString()
        {
            for (int i = 0; i < 52; i++)
            {
                Card card = new Card(i);

                Assert.AreEqual(ExpectedString[i], card.ToString());
            }
        }
    }
}

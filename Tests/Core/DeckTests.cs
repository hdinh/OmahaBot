namespace OmahaBot.Tests.Core
{
    using System.Collections.Generic;
    using NUnit.Framework;
    using OmahaBot.Core;
    using UnitTestUtil;

    [TestFixture]
    public class DeckTests
    {
        [Test]
        public void Remove_2sFromFullDeck_DeckCanDeal51CardsExcluding2s()
        {
            Deck deck = new Deck();

            List<Card> cards = new List<Card>();
            Card twoSpades = new Card("2s");
            deck.Remove(twoSpades);

            for (int i = 0; i < 51; i++)
            {
                Card next = deck.Deal();
                Assert.AreNotEqual(next, twoSpades);

                cards.Add(next);
            }
        }

        [Test]
        public void Deal_FromStart_All52CardsDifferent()
        {
            Deck deck = new Deck();

            List<Card> cards = new List<Card>();

            for (int i = 0; i < 52; i++)
            {
                Card next = deck.Deal();
                Assert.IsFalse(cards.Exists(c => next == c));

                cards.Add(next);
            }
        }

        [Test]
        public void Deal_FromStart_VerifySuitsAndRanksWereAllDealt()
        {
            Deck deck = new Deck();

            bool[,] wasDealt = new bool[13, 4];

            for (int i = 0; i < 52; i++)
            {
                Card next = deck.Deal();
                wasDealt[next.Rank, next.Suit] = true;
            }

            for (int r = 0; r < 13; r++)
            {
                for (int c = 0; c < 4; c++)
                {
                    Assert.IsTrue(wasDealt[r, c]);
                }
            }
        }

        [Test]
        public void Deal_TwoSameConstructors_VerifySeeedConstructorWillCreateIdenticalDecks()
        {
            Deck deck1 = new Deck(123);
            Deck deck2 = new Deck(123);

            for (int i = 0; i < 52; i++)
            {
                Assert.AreEqual(deck1.Deal(), deck2.Deal());
            }
        }

        [Test]
        public void Seed_TwoDifferentDeckConstructorsSeedSameNumber_IdenticalDecks()
        {
            Deck deck1 = new Deck(123);
            Deck deck2 = new Deck(456);

            deck1.Seed = 100;
            deck2.Seed = 100;

            for (int i = 0; i < 52; i++)
            {
                Assert.AreEqual(deck1.Deal(), deck2.Deal());
            }
        }

        [Test]
        public void SetDead_RemoveAllTens_WhenDealingShouldHaveNoTens()
        {
            Deck deck = new Deck();
            deck.SetDead(CardHelper.CreateHandFromString("10s 10c 10d 10h"));

            for (int i = 0; i < 48; i++)
            {
                Card card = deck.Deal();

                Assert.AreNotEqual(HoldemHand.Hand.RankTen, card.Rank);
            }
        }
    }
}

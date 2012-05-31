namespace OmahaBot.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [Serializable]
    public class Card
    {
        private readonly int _card;

        public Card(int card)
        {
            _card = card;
        }

        [SuppressMessage(
            "Microsoft.Usage",
            "CA2233:OperationsShouldNotOverflow",
            Justification = "Constructor is seam for tests, unlikely to overflow")]
        public Card(int rank, int suit)
            : this((suit * 13) + rank)
        {
        }

        public Card(string card)
            : this(HoldemHand.Hand.ParseCard(card))
        {
        }

        public int Rank
        {
            get { return HoldemHand.Hand.CardRank(_card); }
        }

        public int Suit
        {
            get { return HoldemHand.Hand.CardSuit(_card); }
        }

        public override bool Equals(object obj)
        {
            Card card = obj as Card;

            if (card != null)
            {
                return Rank == card.Rank && Suit == card.Suit;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return HoldemHand.Hand.CardTable[_card];
        }
    }
}

namespace OmahaBot.Core
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class Deck : BinaryCloneable
    {
        private const int NumCardsInDeck = 52;

        private List<Card> _cards = new List<Card>();
        private int _seed;

        private Random _random;

        public Deck() : this((int)DateTime.Now.Ticks)
        {
        }

        public Deck(int seed)
        {
            _random = new Random(seed);
            _seed = seed;

            for (int i = 0; i < NumCardsInDeck; i++)
            {
                _cards.Add(new Card(i));
            }
        }

        public int Seed
        {
            get
            {
                return _seed;
            }

            set
            {
                _random = new Random(value);
                _seed = value;
            }
        }

        public void Remove(Card card)
        {
            Card temp = _cards.Find(c => c.Equals(card));

            if (temp != null)
            {
                _cards.Remove(temp);
            }
        }

        public Card Deal()
        {
            int next = _random.Next(_cards.Count);
            Card nextCard = _cards[next];

            _cards.RemoveAt(next);

            return nextCard;
        }

        public void SetDead(Card[] dead)
        {
            for (int i = 0; i < dead.Length; i++)
            {
                _cards.Remove(dead[i]);
            }
        }
    }
}

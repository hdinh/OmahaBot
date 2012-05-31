namespace OmahaBot.Core
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public abstract class OmahaPlayer : IGameObserver
    {
        private static EmptyPlayer _empty = new EmptyPlayer();

        private PlayerId _id = new PlayerId();
        private string _name;
        private Card[] _holeCards = new Card[0];

        public static OmahaPlayer Empty
        {
            get { return _empty; }
        }

        public PlayerId Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "It's easier to expose HoleCards as an array.")]
        public Card[] HoleCards
        {
            get
            {
                return _holeCards;
            }

            set
            {
                _holeCards = value;
            }
        }

        public abstract PokerAction Act();

        public abstract void ActionEvent(int pos, PokerAction act);

        public abstract void DealHoleCardsEvent();

        public abstract void GameOverEvent();

        public abstract void GameStartEvent(IGameInfo gi);

        public abstract void GameStateChanged();

        public abstract void ShowdownEvent(int pos, Card[] cards);

        public abstract void RoundChanged(int round);

        public abstract void WinEvent(int pos, long amount);

        private class EmptyPlayer : OmahaPlayer
        {
            public EmptyPlayer()
            {
            }

            public override PokerAction Act()
            {
                return PokerAction.CreateFoldAction();
            }

            public override void ActionEvent(int pos, PokerAction act)
            {
            }

            public override void DealHoleCardsEvent()
            {
            }

            public override void GameOverEvent()
            {
            }

            public override void GameStartEvent(IGameInfo gi)
            {
            }

            public override void GameStateChanged()
            {
            }

            public override void ShowdownEvent(int pos, Card[] cards)
            {
            }

            public override void RoundChanged(int round)
            {
            }

            public override void WinEvent(int pos, long amount)
            {
            }
        }
    }
}

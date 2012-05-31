namespace OmahaBot.Core
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using OmahaBot.Utilities;

    public class HandHistory
    {
        private PokerSite _site;
        private HandHistoryPlayerCollection _players;
        private Card[] _flopCards = new Card[0];
        private Card _turnCard;
        private Card _riverCard;
        private int _buttonSeat;
        private int _smallBlindSeat;
        private int _bigBlindSeat;
        private long _smallBlindAmount;
        private long _bigBlindAmount;
        private IList<HandHistoryPokerAction> _actions = new List<HandHistoryPokerAction>();
        private List<Pair<HandHistoryPlayer, long>> _winners = new List<Pair<HandHistoryPlayer, long>>();

        public HandHistory(HandHistoryPlayerCollection players)
        {
            _players = players;
        }

        public HandHistory()
        {
            _players = new HandHistoryPlayerCollection();
        }

        public PokerSite Site
        {
            get { return _site; }
            set { _site = value; }
        }

        public HandHistoryPlayerCollection Players
        {
            get { return _players; }
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1819:PropertiesShouldNotReturnArrays",
            Justification = "HandHistory objects doesn't need to be ultra efficient.")]
        public Card[] FlopCards
        {
            get { return _flopCards; }
            set { _flopCards = value; }
        }

        public Card TurnCard
        {
            get { return _turnCard; }
            set { _turnCard = value; }
        }

        public Card RiverCard
        {
            get { return _riverCard; }
            set { _riverCard = value; }
        }

        public int ButtonSeat
        {
            get { return _buttonSeat; }
            set { _buttonSeat = value; }
        }

        public int SmallBlindSeat
        {
            get { return _smallBlindSeat; }
            set { _smallBlindSeat = value; }
        }

        public int BigBlindSeat
        {
            get { return _bigBlindSeat; }
            set { _bigBlindSeat = value; }
        }

        public long SmallBlindAmount
        {
            get { return _smallBlindAmount; }
            set { _smallBlindAmount = value; }
        }

        public long BigBlindAmount
        {
            get { return _bigBlindAmount; }
            set { _bigBlindAmount = value; }
        }

        public IList<HandHistoryPokerAction> Actions
        {
            get { return _actions; }
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Fine. To really fix we can create a custom collection later.")]
        public IList<Pair<HandHistoryPlayer, long>> Winners
        {
            get { return _winners; }
        }
    }
}

namespace OmahaBot.Core
{
    using System.Diagnostics.CodeAnalysis;

    public class HandHistoryPlayer : OmahaPlayer
    {
        private static HandHistoryPlayer _empty = new HandHistoryPlayer(string.Empty, 0);

        private int _seat;
        private long _bankroll;

        private OmahaPlayer _engine;

        public HandHistoryPlayer(string name, long bankroll)
            : this(name, bankroll, OmahaPlayer.Empty)
        {
        }

        public HandHistoryPlayer(string name, long bankroll, OmahaPlayer engine)
        {
            Name = name;
            _bankroll = bankroll;
            _engine = engine;
        }

        public static new HandHistoryPlayer Empty
        {
            get { return _empty; }
        }

        public int Seat
        {
            get { return _seat; }
            internal set { _seat = value; }
        }

        public long Bankroll
        {
            get { return _bankroll; }
        }

        public override PokerAction Act()
        {
            return _engine.Act();
        }

        public override void ActionEvent(int pos, PokerAction act)
        {
            _engine.ActionEvent(pos, act);
        }

        public override void DealHoleCardsEvent()
        {
            _engine.DealHoleCardsEvent();
        }

        public override void GameOverEvent()
        {
            _engine.GameOverEvent();
        }

        public override void GameStartEvent(IGameInfo gi)
        {
            _engine.GameStartEvent(gi);
        }

        public override void GameStateChanged()
        {
            _engine.GameStateChanged();
        }

        public override void ShowdownEvent(int pos, Card[] cards)
        {
            _engine.ShowdownEvent(pos, cards);
        }

        public override void RoundChanged(int round)
        {
            _engine.RoundChanged(round);
        }

        public override void WinEvent(int pos, long amount)
        {
            _engine.WinEvent(pos, amount);
        }
    }
}

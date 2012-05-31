namespace OmahaBot.Core
{
    using System.Diagnostics.CodeAnalysis;

    public class PokerAction
    {
        private ActionType _action;
        private long _bet;

        internal PokerAction(ActionType action)
        {
            _action = action;
        }

        internal PokerAction(ActionType action, long bet)
        {
            _action = action;
            _bet = bet;
        }

        [SuppressMessage(
            "Microsoft.Naming",
            "CA1721:PropertyNamesShouldNotMatchGetMethods",
            Justification = "Type is fine for now.")]
        public ActionType Type
        {
            get { return _action; }
        }

        public long Bet
        {
            get { return _bet; }
        }

        public static PokerAction CreateFoldAction()
        {
            return new PokerAction(ActionType.Fold);
        }

        public static PokerAction CreateCallAction()
        {
            return new PokerAction(ActionType.Call);
        }

        public static PokerAction CreateRaiseAction(long bet)
        {
            return new PokerAction(ActionType.Raise, bet);
        }
    }
}

namespace OmahaBot.Core
{
    using System;

    public class HandHistoryPokerAction
    {
        private PokerAction _action;
        private HandHistoryPlayer _player;

        public HandHistoryPokerAction(HandHistoryPlayer player, PokerAction action)
        {
            _player = player;
            _action = action;
        }

        public PokerAction Action
        {
            get { return _action; }
        }

        public HandHistoryPlayer Player
        {
            get { return _player; }
        }

        internal static PokerAction Create(string action, long amount)
        {
            ActionType type;

            switch (action)
            {
                case "folds":
                    type = ActionType.Fold;
                    break;
                case "checks":
                case "calls":
                    type = ActionType.Call;
                    break;
                case "bets":
                case "raises":
                    type = ActionType.Raise;
                    break;
                default:
                    throw new ArgumentException("Action type is unknown " + action);
            }

            return new PokerAction(type, amount);
        }
    }
}

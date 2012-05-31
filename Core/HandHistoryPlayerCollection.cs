namespace OmahaBot.Core
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage(
        "Microsoft.Naming",
        "CA1711:IdentifiersShouldNotHaveIncorrectSuffix",
        Justification = "Collection suffix is fine here, this is likely to inherit ICollection later if neede.")]
    public class HandHistoryPlayerCollection
    {
        private Dictionary<int, HandHistoryPlayer> _players = new Dictionary<int, HandHistoryPlayer>();

        public int Count
        {
            get { return _players.Count; }
        }

        public HandHistoryPlayer this[int seat]
        {
            get
            {
                if (_players.ContainsKey(seat))
                {
                    return _players[seat];
                }
                else
                {
                    return HandHistoryPlayer.Empty;
                }
            }

            set
            {
                _players[seat] = value;
                _players[seat].Seat = seat;
            }
        }

        public HandHistoryPlayer this[string playerName]
        {
            get
            {
                foreach (HandHistoryPlayer player in _players.Values)
                {
                    if (player.Name == playerName)
                    {
                        return player;
                    }
                }

                return HandHistoryPlayer.Empty;
            }
        }
    }
}

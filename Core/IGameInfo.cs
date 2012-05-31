namespace OmahaBot.Core
{
    using System.Diagnostics;

    public interface IGameInfo
    {
        int ActiveSeat { get; }

        long CurrentBet { get; }

        int NumberOfLivePlayersRound { get; }

        int NumberOfLivePlayersHand { get; }

        int ButtonSeat { get; }

        int CurrentRound { get; }

        long GetAmountToCall(int seat);

        bool GetDidCallCurrentRound(int seat);
    }
}

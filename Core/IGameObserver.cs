namespace OmahaBot.Core
{
    public interface IGameObserver
    {
        void ActionEvent(int pos, PokerAction act);

        void DealHoleCardsEvent();

        void GameOverEvent();
        
        void GameStartEvent(IGameInfo gi);

        void GameStateChanged();

        void ShowdownEvent(int pos, Card[] cards);

        void RoundChanged(int round);

        void WinEvent(int pos, long amount);
    }
}

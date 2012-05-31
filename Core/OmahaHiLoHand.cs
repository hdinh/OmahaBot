namespace OmahaBot.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    public delegate long SetBankrollServiceHandler(int seat, long bankroll);

    [SuppressMessage(
            "Microsoft.Naming",
            "CA1709:IdentifiersShouldBeCasedCorrectly",
            Justification = "HiLo is fine")]
    [Serializable]
    public class OmahaHiLoHand : BinaryCloneable, IDeserializationCallback
    {
        private const int MaxPlayers = 10;
        private const int NumberOfRounds = 4;
        private const int NumberOfCardsForOmaha = 4;

        [NonSerialized]
        private OmahaPlayer[] _players = new OmahaPlayer[MaxPlayers];

        [NonSerialized]
        private List<IGameObserver> _observers = new List<IGameObserver>();

        private long[] _bankrolls = new long[MaxPlayers];
        private Card[][] _hands = new Card[MaxPlayers][];
        private List<Card> _commonHand = new List<Card>();
        private HandHistoryState _historyState;
        private Deck _deck = new Deck();
        private int _seed;

        private HandPlayingState _state;

        private List<Card> _commonForcedBuffer = new List<Card>();

        public OmahaHiLoHand(HandHistory script)
            : this()
        {
            _historyState = new HandHistoryState();

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (script.Players[i] != HandHistoryPlayer.Empty)
                {
                    Sit(script.Players[i], i, script.Players[i].Bankroll);
                }
            }

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (script.Players[i] != HandHistoryPlayer.Empty)
                {
                    _players[i] = script.Players[i];
                    _bankrolls[i] = script.Players[i].Bankroll;
                    _historyState.SetSeatDealt(i);

                    if (script.Players[i].HoleCards.Length > 0)
                    {
                        _hands[i] = new Card[NumberOfCardsForOmaha];
                        script.Players[i].HoleCards.CopyTo(_hands[i], 0);

                        Array.ForEach(_hands[i], c => _deck.Remove(c));
                    }
                    else
                    {
                        _hands[i] = new Card[0];
                    }
                }
            }

            _historyState.ButtonSeat = script.ButtonSeat;

            _historyState.ActiveSeat = script.SmallBlindSeat;
            _historyState.PostBlind(script.SmallBlindSeat, script.SmallBlindAmount);
            _historyState.PostBlind(script.BigBlindSeat, script.BigBlindAmount);

            _state = HandPlayingState.Preflop;

            // Buffer the common cards
            if (script.FlopCards != null)
            {
                _commonForcedBuffer.AddRange(script.FlopCards);
            }

            if (script.TurnCard != null)
            {
                _commonForcedBuffer.Add(script.TurnCard);
            }

            if (script.RiverCard != null)
            {
                _commonForcedBuffer.Add(script.RiverCard);
            }

            foreach (HandHistoryPokerAction action in script.Actions)
            {
                SimulateNext(action.Action);
            }
        }

        public OmahaHiLoHand()
        {
            HandHistoryState.SetSeatBankrollServiceCallback(SetSeatBankroll);

            _state = HandPlayingState.InitHand;
        }

        private enum HandPlayingState
        {
            InitHand,
            DealCards,
            PostSmallBlind,
            PostBigBlind,
            Preflop,
            Flop,
            Turn,
            River,
            ChangeRound,
            Showdown
        }

        public int PlayerCount
        {
            get
            {
                int count = 0;

                for (int i = 0; i < MaxPlayers; i++)
                {
                    if (IsSeated(i))
                    {
                        count++;
                    }
                }

                return count;
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
                _seed = value;
                _deck.Seed = value;
            }
        }

        public void Sit(OmahaPlayer player, int seat, long bankroll)
        {
            if (_players[seat] == null)
            {
                _players[seat] = player;
                _bankrolls[seat] = bankroll;

                if (!_observers.Contains(player))
                {
                    _observers.Add(player);
                }
            }
            else
            {
                throw new OmahaException("That seat is already taken!");
            }
        }

        public void Stand(OmahaPlayer player)
        {
            for (int seat = 0; seat < _players.Length; seat++)
            {
                if (_players[seat] != null && _players[seat] == player)
                {
                    _players[seat] = null;

                    if (_observers.Contains(player))
                    {
                        _observers.Add(player);
                    }

                    break;
                }
            }
        }

        public void SimulateNext()
        {
            SimulateNext(null);
        }

        public void SimulateNext(PokerAction force)
        {
            switch (_state)
            {
                case HandPlayingState.InitHand:
                    InitHand();
                    break;
                case HandPlayingState.DealCards:
                    DealCards();
                    break;
                case HandPlayingState.PostSmallBlind:
                    PostSmallBlind();
                    break;
                case HandPlayingState.PostBigBlind:
                    PostBigBlind();
                    break;
                case HandPlayingState.Preflop:
                case HandPlayingState.Flop:
                case HandPlayingState.Turn:
                case HandPlayingState.River:
                    PlayerAction(force);
                    break;
                case HandPlayingState.ChangeRound:
                    ChangeRound();
                    break;
            }
        }

        public HandResult RunToEnd()
        {
            while (_state < HandPlayingState.Showdown)
            {
                SimulateNext();
            }

            HandResult handResult = new HandResult();
            List<OmahaPlayer> winnersHi = new List<OmahaPlayer>();
            List<OmahaPlayer> winnersLo = new List<OmahaPlayer>();

            uint bestHiValue = 0;
            uint bestLoValue = 0;

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (_historyState.IsAliveHand(i))
                {
                    uint seatHiValue = OmahaHandHighEvaluator.Evaluate(_hands[i], _commonHand.ToArray());
                    uint seatLoValue = OmahaHandLowEvaluator.Evaluate(_hands[i], _commonHand.ToArray());

                    foreach (IGameObserver observer in _observers)
                    {
                        observer.ShowdownEvent(i, _hands[i]);
                    }

                    if (seatHiValue > bestHiValue)
                    {
                        bestHiValue = seatHiValue;

                        winnersHi.Clear();
                        winnersHi.Add(_players[i]);
                    }
                    else if (seatHiValue == bestHiValue)
                    {
                        winnersHi.Add(_players[i]);
                    }

                    if (seatLoValue > bestLoValue)
                    {
                        bestLoValue = seatLoValue;

                        winnersLo.Clear();
                        winnersLo.Add(_players[i]);
                    }
                    else if (seatLoValue > 0 && seatLoValue == bestLoValue)
                    {
                        winnersLo.Add(_players[i]);
                    }
                }
            }

            double highShare = 1.0;

            if (bestLoValue > 0)
            {
                winnersLo.ForEach(lw => handResult.Winners.Add(new HandWinner(lw, (long)((_historyState.CommonPot / winnersLo.Count) * 0.5))));

                // The high only wins half now
                highShare = 0.50;
            }

            foreach (OmahaPlayer highWinner in winnersHi)
            {
                int existIndex = handResult.Winners.FindIndex(p => p.Player == highWinner);
                
                if (existIndex == -1)
                {
                    handResult.Winners.Add(new HandWinner(highWinner, (long)((_historyState.CommonPot / winnersHi.Count) * highShare)));
                }
                else
                {
                    handResult.Winners[existIndex].Amount += (long)((_historyState.CommonPot / winnersHi.Count) * highShare);
                }
            }

            for (int i = 0; i < _players.Length; i++)
            {
                int winnerIndex = handResult.Winners.FindIndex(p => p.Player == _players[i]);

                if (winnerIndex != -1)
                {
                    foreach (IGameObserver observer in _observers)
                    {
                        observer.WinEvent(winnerIndex, handResult.Winners[winnerIndex].Amount);
                    }
                }
            }

            foreach (IGameObserver observer in _observers)
            {
                observer.GameOverEvent();
            }

            return handResult;
        }

        public override object Clone()
        {
            OmahaHiLoHand cloned = (OmahaHiLoHand)base.Clone();

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (_players[i] != null)
                {
                    cloned.Sit(_players[i], i, _bankrolls[i]);
                }
            }

            return cloned;
        }

        public void OnDeserialization(object sender)
        {
            HandHistoryState.SetSeatBankrollServiceCallback(SetSeatBankroll);

            _players = new OmahaPlayer[MaxPlayers];
            _observers = new List<IGameObserver>();
        }

        private void InitHand()
        {
            _historyState = new HandHistoryState();

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (IsSeated(i))
                {
                    _historyState.SetSeatDealt(i);
                    _hands[i] = new Card[NumberOfCardsForOmaha];
                }
            }

            _historyState.AdvanceButtonSeat();

            foreach (IGameObserver observer in _observers)
            {
                observer.GameStartEvent(_historyState);
            }

            _state++;
        }

        private void DealCards()
        {
            // Deal
            for (int i = 0; i < NumberOfCardsForOmaha; i++)
            {
                for (int j = 0; j < _historyState.NumberOfLivePlayersHand; j++)
                {
                    _hands[_historyState.ActiveSeat][i] = _deck.Deal();
                    _historyState.AdvanceActiveSeat();
                }
            }

            foreach (IGameObserver observer in _observers)
            {
                observer.DealHoleCardsEvent();
            }

            for (int i = 0; i < MaxPlayers; i++)
            {
                if (_hands[i] != null && _hands[i].Length > 0)
                {
                    _players[i].HoleCards = _hands[i];
                }
            }

            _state++;
        }

        private void PostSmallBlind()
        {
            // Preflop forced action
            long smallBet = 40;
            long ante = 0;

            int openingSeat = _historyState.ActiveSeat;

            do
            {
                _historyState.PostBlind(_historyState.ActiveSeat, ante);
            }
            while (_historyState.ActiveSeat != openingSeat);

            // Take Small blind
            _historyState.PostBlind(_historyState.ActiveSeat, smallBet);

            _state++;
        }

        private void PostBigBlind()
        {
            // Take Big blind
            long bigBet = 20;
            _historyState.PostBlind(_historyState.ActiveSeat, bigBet);
            _state++;
        }

        private void PlayerAction(PokerAction forced)
        {
            int actingSeat = _historyState.ActiveSeat;

            PokerAction action;

            if (forced == null)
            {
                action = _players[actingSeat].Act();
            }
            else
            {
                action = forced;
            }

            foreach (IGameObserver observer in _observers)
            {
                observer.ActionEvent(actingSeat, action);
            }

            if (action.Type == ActionType.Fold)
            {
                _historyState.SetDead(actingSeat);

                // We have to manual advance this seat manually
                _historyState.AdvanceActiveSeat();
            }
            else if (action.Type == ActionType.Call)
            {
                _historyState.CommitBet(actingSeat, _historyState.GetAmountToCall(actingSeat));
                _historyState.SetCallCurrentRound(actingSeat);
            }
            else
            {
                Debug.Assert(action.Bet > _historyState.CurrentBet, "Bet needs to be larger than the currentBet " + _historyState.CurrentBet);
                _historyState.CommitBet(actingSeat, action.Bet);

                _historyState.RegeneratePlayersInRound(actingSeat);
            }

            if (_historyState.NumberOfLivePlayersHand < 2)
            {
                _state = HandPlayingState.Showdown;
            }
            else if (_historyState.NumberOfLivePlayersRound == 0)
            {
                if (_state == HandPlayingState.River)
                {
                    _state = HandPlayingState.Showdown;
                }
                else
                {
                    _state = HandPlayingState.ChangeRound;
                }
            }

            foreach (IGameObserver observer in _observers)
            {
                observer.GameStateChanged();
            }
        }

        private void ChangeRound()
        {
            _historyState.AdvanceRound();

            switch (_historyState.CurrentRound)
            {
                case 1:
                    if (_commonForcedBuffer.Count > 2)
                    {
                        _commonHand.Add(_commonForcedBuffer[0]);
                        _commonHand.Add(_commonForcedBuffer[1]);
                        _commonHand.Add(_commonForcedBuffer[2]);

                        _deck.Remove(_commonForcedBuffer[0]);
                        _deck.Remove(_commonForcedBuffer[1]);
                        _deck.Remove(_commonForcedBuffer[2]);
                    }
                    else
                    {
                        _commonHand.Add(_deck.Deal());
                        _commonHand.Add(_deck.Deal());
                        _commonHand.Add(_deck.Deal());
                    }

                    _state = HandPlayingState.Flop;
                    break;
                case 2:
                    if (_commonForcedBuffer.Count > 3)
                    {
                        _commonHand.Add(_commonForcedBuffer[3]);
                        _deck.Remove(_commonForcedBuffer[3]);
                    }
                    else
                    {
                        _commonHand.Add(_deck.Deal());
                    }

                    _state = HandPlayingState.Turn;
                    break;
                case 3:
                    if (_commonForcedBuffer.Count > 4)
                    {
                        _commonHand.Add(_commonForcedBuffer[4]);
                        _deck.Remove(_commonForcedBuffer[4]);
                    }
                    else
                    {
                        _commonHand.Add(_deck.Deal());
                    }

                    _state = HandPlayingState.River;
                    break;
            }

            foreach (IGameObserver observer in _observers)
            {
                observer.RoundChanged(_historyState.CurrentRound);
            }
        }

        private bool IsSeated(int seat)
        {
            return _players[seat] != null;
        }

        private long SetSeatBankroll(int seat, long change)
        {
            Debug.Assert(IsSeated(seat), "Seat should be seated.");

            long actualChange;

            if (_bankrolls[seat] + change < 0)
            {
                actualChange = _bankrolls[seat];
                _bankrolls[seat] = 0;
            }
            else
            {
                actualChange = change;
                _bankrolls[seat] += change;
            }

            return actualChange;
        }

        [Serializable]
        internal class HandHistoryState : IGameInfo
        {
            private static SetBankrollServiceHandler _setSeatBankroll;

            private int _buttonSeat;
            private int _activeSeat;

            private int _currentRound;

            private long _commonPot;
            private long _currentBet;
            private long _commonCurrentRoundPot;
            private long[] _amountPutInRound = new long[MaxPlayers];

            private bool[] _wasDealt = new bool[MaxPlayers];
            private bool[] _isAliveHand = new bool[MaxPlayers];
            private bool[][] _wasAliveRound = new bool[MaxPlayers][];
            private bool[][] _isAliveRound = new bool[MaxPlayers][];
            private bool[] _didCallCurrentRound = new bool[MaxPlayers];

            internal HandHistoryState()
            {
                for (int i = 0; i < MaxPlayers; i++)
                {
                    _isAliveRound[i] = new bool[NumberOfRounds];
                    _wasAliveRound[i] = new bool[NumberOfRounds];
                }
            }

            public int ActiveSeat
            {
                get { return _activeSeat; }
                set { _activeSeat = value; }
            }

            public long CurrentBet
            {
                get { return _currentBet; }
            }

            public long CommonPot
            {
                get { return _commonPot; }
            }

            public int NumberOfLivePlayersRound
            {
                get
                {
                    int numLive = 0;

                    for (int i = 0; i < MaxPlayers; i++)
                    {
                        if (_isAliveRound[i][_currentRound])
                        {
                            numLive++;
                        }
                    }

                    return numLive;
                }
            }

            public int NumberOfLivePlayersHand
            {
                get
                {
                    int numLive = 0;

                    for (int i = 0; i < MaxPlayers; i++)
                    {
                        if (_isAliveHand[i])
                        {
                            numLive++;
                        }
                    }

                    return numLive;
                }
            }

            public int ButtonSeat
            {
                get { return _buttonSeat; }
                set { _buttonSeat = value; }
            }

            public int CurrentRound
            {
                get { return _currentRound; }
            }

            public long GetAmountToCall(int seat)
            {
                return _currentBet - _amountPutInRound[seat];
            }

            public bool GetDidCallCurrentRound(int seat)
            {
                return _didCallCurrentRound[seat];
            }

            internal static void SetSeatBankrollServiceCallback(SetBankrollServiceHandler setSeatBankroll)
            {
                _setSeatBankroll = setSeatBankroll;
            }

            internal void AdvanceActiveSeat()
            {
                _activeSeat = GetNextLiveSeat(_activeSeat);
            }

            internal void AdvanceButtonSeat()
            {
                _buttonSeat = GetNextLiveSeat(_buttonSeat);
                _activeSeat = GetNextLiveSeat(_buttonSeat);
            }

            internal void CommitBet(int seat, long bet)
            {
                Debug.Assert(seat == _activeSeat, "ActiveSeat should be the only one that should be acting.");
                Debug.Assert(_isAliveRound[seat][_currentRound], "Seat has to be qualified to act.");

                long amountBet = -1 * _setSeatBankroll(seat, -bet);
                _commonCurrentRoundPot += amountBet;
                _amountPutInRound[seat] += amountBet;
                _commonPot += amountBet;

                AdvanceActiveSeat();

                if (bet > _currentBet)
                {
                    _currentBet = bet;
                }
                else
                {
                    SetRoundAliveStatus(seat, _currentRound, false);
                }
            }

            internal void PostBlind(int seat, long bet)
            {
                Debug.Assert(_currentRound == 0, "Should only post blind in preflop.");

                long amountBet = -1 * _setSeatBankroll(seat, -bet);
                _commonCurrentRoundPot += amountBet;
                _amountPutInRound[seat] = amountBet;
                _commonPot += amountBet;
                _currentBet = bet;

                AdvanceActiveSeat();
            }

            internal bool IsAliveHand(int seat)
            {
                return _isAliveHand[seat];
            }

            internal void SetSeatDealt(int seat)
            {
                _wasDealt[seat] = true;
                _isAliveHand[seat] = true;

                SetRoundAliveStatus(seat, 0, true);
            }

            internal void SetDead(int seat)
            {
                _isAliveHand[seat] = false;
                SetRoundAliveStatus(seat, _currentRound, false);
            }

            internal void AdvanceRound()
            {
                Debug.Assert(_currentRound < NumberOfRounds, "Current round is more than the possible number of rounds");

                _commonCurrentRoundPot = 0;
                _currentBet = 0;
                _currentRound++;

                for (int i = 0; i < MaxPlayers; i++)
                {
                    if (_isAliveHand[i])
                    {
                        SetRoundAliveStatus(i, _currentRound, true);
                    }

                    _amountPutInRound[i] = 0;
                }

                _activeSeat = GetNextLiveSeat(_buttonSeat);
            }

            internal void RegeneratePlayersInRound(int excludedPlayer)
            {
                // Everybody how had called this round is now alive again
                for (int j = 0; j < MaxPlayers; j++)
                {
                    if (j != excludedPlayer)
                    {
                        if (_wasAliveRound[j][_currentRound])
                        {
                            SetRoundAliveStatus(j, _currentRound, true);
                        }
                    }
                }
            }

            internal void SetCallCurrentRound(int seat)
            {
                _didCallCurrentRound[seat] = true;
            }

            private int GetNextLiveSeat(int seat)
            {
                int actualButtonSeat = -1;

                for (int seatAfter = seat + 1; seatAfter < MaxPlayers; seatAfter++)
                {
                    if (_isAliveRound[seatAfter][_currentRound])
                    {
                        actualButtonSeat = seatAfter;
                        break;
                    }
                }

                if (actualButtonSeat == -1)
                {
                    for (int seatBefore = 0; seatBefore <= _buttonSeat; seatBefore++)
                    {
                        if (_isAliveRound[seatBefore][_currentRound])
                        {
                            actualButtonSeat = seatBefore;
                            break;
                        }
                    }
                }

                return actualButtonSeat;
            }

            private void SetRoundAliveStatus(int seat, int round, bool status)
            {
                Debug.Assert(round == _currentRound, "Should not set a round status if its not the current round.");

                _isAliveRound[seat][round] = status;
                _wasAliveRound[seat][round] |= status;
            }
        }
    }
}

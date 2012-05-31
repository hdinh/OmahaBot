namespace OmahaBot.Core
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using OmahaBot.Utilities;

    public class HandReader
    {
        private static readonly CultureInfo EnUsCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

        private ParseState _state = ParseState.Site;
        private string _line;
        private bool _doNotConsumeNextLine;

        private PokerSite _site;
        private int _buttonSeat;
        private int _smallBlindSeat;
        private int _bigBlindSeat;
        private long _smallBlindAmount;
        private long _bigBlindAmount;
        private HandHistoryPlayerCollection _players = new HandHistoryPlayerCollection();
        private List<Card> _commonCards = new List<Card>();
        private List<HandHistoryPokerAction> _actions = new List<HandHistoryPokerAction>();
        private List<Pair<HandHistoryPlayer, long>> _winners = new List<Pair<HandHistoryPlayer, long>>();

        private enum ParseState
        {
            Site,
            TableId,
            Seats,
            Blinds,
            Cards,
            Preflop,
            FlopTransition,
            Flop,
            TurnTransition,
            Turn,
            RiverTransition,
            River,
            ShowdownTransition,
            Showdown,
            Summary
        }

        public HandHistory Parse(string handLog)
        {
            StringReader sr = new StringReader(handLog);

            while (_doNotConsumeNextLine || (_line = sr.ReadLine()) != null)
            {
                _doNotConsumeNextLine = false;

                switch (_state)
                {
                    case ParseState.Site:
                        StateSite();
                        break;
                    case ParseState.TableId:
                        StateTableId();
                        break;
                    case ParseState.Seats:
                        StateSeats();
                        break;
                    case ParseState.Blinds:
                        StateBlinds();
                        break;
                    case ParseState.Cards:
                        StateCards();
                        break;
                    case ParseState.Preflop:
                        StatePlayerAction();
                        break;
                    case ParseState.FlopTransition:
                        StateRoundTransition();
                        break;
                    case ParseState.Flop:
                        StatePlayerAction();
                        break;
                    case ParseState.TurnTransition:
                        StateRoundTransition();
                        break;
                    case ParseState.Turn:
                        StatePlayerAction();
                        break;
                    case ParseState.RiverTransition:
                        StateRoundTransition();
                        break;
                    case ParseState.River:
                        StatePlayerAction();
                        break;
                    case ParseState.ShowdownTransition:
                        StateShowdownTransition();
                        break;
                    case ParseState.Showdown:
                        StateShowdown();
                        break;
                    case ParseState.Summary:
                        break;
                }
            }

            HandHistory hand = new HandHistory(_players);
            hand.Site = _site;

            hand.ButtonSeat = _buttonSeat;
            hand.SmallBlindSeat = _smallBlindSeat;
            hand.BigBlindSeat = _bigBlindSeat;
            hand.SmallBlindAmount = _smallBlindAmount;
            hand.BigBlindAmount = _bigBlindAmount;

            if (_commonCards.Count > 2)
            {
                hand.FlopCards = new Card[3];

                for (int i = 0; i < 3; i++)
                {
                    hand.FlopCards[i] = _commonCards[i];
                }
            }

            if (_commonCards.Count > 3)
            {
                hand.TurnCard = _commonCards[3];
            }

            if (_commonCards.Count > 4)
            {
                hand.RiverCard = _commonCards[4];
            }

            foreach (HandHistoryPokerAction action in _actions)
            {
                hand.Actions.Add(action);
            }

            foreach (Pair<HandHistoryPlayer, long> winner in _winners)
            {
                hand.Winners.Add(winner);
            }

            return hand;
        }

        private void StateSite()
        {
            if (Regex.IsMatch(_line, "^PokerStars.*"))
            {
                _site = PokerSite.PokerStars;
                _state = ParseState.TableId;
            }
        }

        private void StateTableId()
        {
            Match match = Regex.Match(_line, @"^Table '[\d]*\s?[\d]*?' [A-Za-z\d\-\s]*#([\d]*).*");

            if (match.Success)
            {
                _buttonSeat = int.Parse(match.Groups[1].Value, EnUsCulture);
                _state = ParseState.Seats;
            }
        }

        private void StateSeats()
        {
            Match match = Regex.Match(_line, @"^Seat ([\d]*): ([^\(]*) \(([\d]*)[a-zA-Z\s]*\).*");

            if (match.Success)
            {
                int seat = int.Parse(match.Groups[1].Value, EnUsCulture);
                string playerName = match.Groups[2].Value;
                long bankroll = long.Parse(match.Groups[3].Value, EnUsCulture);

                _players[seat] = new HandHistoryPlayer(playerName, bankroll);
            }
            else
            {
                _doNotConsumeNextLine = true;
                _state = ParseState.Blinds;
            }
        }

        private void StateBlinds()
        {
            Match match = Regex.Match(_line, @"(^[^\:]*): posts ([a-zA-Z\s]*) ([\d]*)");

            if (match.Success)
            {
                string player = match.Groups[1].Value;
                long amount = long.Parse(match.Groups[3].Value, EnUsCulture);

                if (match.Groups[2].Value == "small blind")
                {
                    _smallBlindSeat = _players[player].Seat;
                    _smallBlindAmount = amount;
                }
                else if (match.Groups[2].Value == "big blind")
                {
                    _bigBlindSeat = _players[player].Seat;
                    _bigBlindAmount = amount;
                }
            }
            else if (Regex.IsMatch(_line, @"[*]{3} ([a-zA-Z\s]*) [*]{3}"))
            {
                _state = ParseState.Cards;
            }
        }

        private void StateCards()
        {
            Match match = Regex.Match(_line, @"^Dealt to ([^[]*) \[(([\dTJQKA][scdh]\s?)*)\].*");

            if (match.Success)
            {
                string name = match.Groups[1].Value;
                string[] cardsStr = match.Groups[2].Value.Split(' ');

                Card[] cards = new Card[cardsStr.Length];
                for (int i = 0; i < cards.Length; i++)
                {
                    cards[i] = new Card(cardsStr[i]);
                }

                _players[name].HoleCards = cards;
            }
            else
            {
                _state = ParseState.Preflop;
                _doNotConsumeNextLine = true;
            }
        }

        private void StateRoundTransition()
        {
            Match match = Regex.Match(_line, @"[*]{3} ([a-zA-Z\s]*) [*]{3} ((\s?\[(([\dTJQKA][scdh]\s?)*)\])*)");

            if (match.Success)
            {
                string[] cards = match.Groups[2].Value.Replace("[", string.Empty).Replace("]", string.Empty).Split(' ');

                for (int i = 0; i < cards.Length; i++)
                {
                    if (_commonCards.Count == i)
                    {
                        _commonCards.Add(new Card(cards[i]));
                    }
                }

                _state++;
            }
        }

        private void StatePlayerAction()
        {
            Match actionMatch = Regex.Match(_line, @"([^:]*): (checks|bets|calls|raises|folds) (\d*)( to (\d*))?");

            if (actionMatch.Success)
            {
                string player = actionMatch.Groups[1].Value;
                string action = actionMatch.Groups[2].Value;
                long amount = 0;

                if (actionMatch.Groups[3].Value.Length > 0)
                {
                    amount = long.Parse(actionMatch.Groups[3].Value, EnUsCulture);
                }

                if (actionMatch.Groups[4].Value.Length > 0)
                {
                    amount = long.Parse(actionMatch.Groups[5].Value, EnUsCulture);
                }

                _actions.Add(new HandHistoryPokerAction(_players[player], HandHistoryPokerAction.Create(action, amount)));
            }
            else
            {
                Match collectMatch = Regex.Match(_line, @"(([a-zA-Z\d])*)? collected \(?(\d*)\)?.*");

                if (collectMatch.Success)
                {
                    string player = collectMatch.Groups[1].Value;
                    long amount = long.Parse(collectMatch.Groups[3].Value, EnUsCulture);

                    Pair<HandHistoryPlayer, long> winner = _winners.Find(w => w.First.Name == player);
                    if (winner == null)
                    {
                        _winners.Add(new Pair<HandHistoryPlayer, long>(_players[player], amount));
                    }
                    else
                    {
                        winner.Second += amount;
                    }
                }
                else if (Regex.IsMatch(_line, @"[*]{3} ([a-zA-Z\s]*) [*]{3}.*"))
                {
                    _state++;
                    _doNotConsumeNextLine = true;
                }
            }
        }

        private void StateShowdownTransition()
        {
            if (Regex.IsMatch(_line, @"[*]{3} ([a-zA-Z\s]*) [*]{3}"))
            {
                _state++;
            }
        }

        private void StateShowdown()
        {
            Match showMatch = Regex.Match(_line, @"([^:]*): shows \[(([\dTJQKA][scdh]\s?)*)\] ([^\)]*).*");
            Match collectMatch = Regex.Match(_line, @"(([a-zA-Z\d])*)? collected \(?(\d*)\)?.*");

            if (showMatch.Success)
            {
                //// Let's ignore this for now
                ////string player = showMatch.Groups[1].Value;
                ////string[] cards = showMatch.Groups[2].Value.Split();
            }
            else if (collectMatch.Success)
            {
                string player = collectMatch.Groups[1].Value;
                long amount = long.Parse(collectMatch.Groups[3].Value, EnUsCulture);

                Pair<HandHistoryPlayer, long> winner = _winners.Find(w => w.First.Name == player);
                if (winner == null)
                {
                    _winners.Add(new Pair<HandHistoryPlayer, long>(_players[player], amount));
                }
                else
                {
                    winner.Second += amount;
                }
            }
            else
            {
                _state++;
                _doNotConsumeNextLine = true;
            }
        }
    }
}

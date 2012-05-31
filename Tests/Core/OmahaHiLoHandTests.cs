namespace OmahaBot.Tests.Core
{
    using System.Collections.Generic;
    using Moq;
    using NUnit.Framework;
    using OmahaBot.Core;
    using UnitTestUtil;

    [TestFixture]
    public class OmahaHiLoHandTests
    {
        [Test]
        public void Stand_TwoPlayersSeated_StandupTwoPlayersPlayerCountIsZero()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.Stand(mock1.Object);
            hand.Stand(mock2.Object);

            Assert.AreEqual(0, hand.PlayerCount);
        }

        [Test]
        [ExpectedException(typeof(OmahaException))]
        public void Sit_SitDifferentPlayersAtSameSeat_VerifiesThrowsException()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction());

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 0, 1000);
        }

        [Test]
        public void Sit_SitMaxPlayers_VerifyPlayerCoundEqualsMaxPlayers()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();
            int maxPlayers = 10;

            var mock = new Mock<OmahaPlayer>();
            mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            for (int i = 0; i < maxPlayers; i++)
            {
                hand.Sit(mock.Object, i, 1000);
            }

            Assert.AreEqual(maxPlayers, hand.PlayerCount);
        }

        [Test]
        public void RunToEnd_TwoPlayers_CanDeclareWinner()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();
            int numPlayers = 2;

            OmahaPlayer[] players = new OmahaPlayer[2];

            for (int i = 0; i < numPlayers; i++)
            {
                var mock = new Mock<OmahaPlayer>();
                mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
                players[i] = mock.Object;
                hand.Sit(mock.Object, i, 1000);
            }

            HandResult result = hand.RunToEnd();

            Assert.GreaterOrEqual(result.Winners.Count, 1);
        }

        [Test]
        public void RunToEnd_TwoPlayersCallPlayerAndFoldPlayer_VerifyCallPlayerIsWinner()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var call = new Mock<OmahaPlayer>();
            var fold = new Mock<OmahaPlayer>();

            call.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            fold.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();

            hand.Sit(call.Object, 0, 1000);
            hand.Sit(fold.Object, 1, 1000);

            HandResult result = hand.RunToEnd();

            Assert.AreEqual(1, result.Winners.Count);
            Assert.AreEqual(call.Object, result.Winners[0].Player);

            call.Verify();
            fold.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayersRaisePlayerAndFoldPlayer_VerifyRaisePlayerIsWinner()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            long amountToBet = 1000;

            var raise = new Mock<OmahaPlayer>();
            var fold = new Mock<OmahaPlayer>();

            raise.Setup(p => p.Act()).Returns(PokerAction.CreateRaiseAction(amountToBet)).Callback(() => amountToBet += 1000).Verifiable();
            fold.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();

            hand.Sit(raise.Object, 0, 1000);
            hand.Sit(fold.Object, 1, 1000);

            HandResult result = hand.RunToEnd();

            Assert.AreEqual(1, result.Winners.Count);
            Assert.AreEqual(raise.Object, result.Winners[0].Player);
        }

        [Test]
        public void RunToEnd_ThreeCallPlayers_RunThreeHands()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            int numPlayers = 3;
            OmahaPlayer[] players = new OmahaPlayer[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                var mock = new Mock<OmahaPlayer>();
                mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
                players[i] = mock.Object;
                hand.Sit(mock.Object, i, 1000);
            }

            List<HandResult> results = new List<HandResult>();

            for (int i = 0; i < 3; i++)
            {
                HandResult result = hand.RunToEnd();

                Assert.IsFalse(results.Contains(result));

                results.Add(result);
            }
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveGameInfoWithTwoPlayers()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.GameStartEvent(It.Is<IGameInfo>(g => g.NumberOfLivePlayersHand == 2))).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock2.Setup(p => p.GameStartEvent(It.Is<IGameInfo>(g => g.NumberOfLivePlayersHand == 2))).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveHoleCardsEvent()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.DealHoleCardsEvent()).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock2.Setup(p => p.DealHoleCardsEvent()).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveActionEvent()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.ActionEvent(0, It.Is<PokerAction>(a => a.Type == ActionType.Call))).Verifiable();
            mock1.Setup(p => p.ActionEvent(1, It.Is<PokerAction>(a => a.Type == ActionType.Fold))).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();
            mock2.Setup(p => p.ActionEvent(0, It.Is<PokerAction>(a => a.Type == ActionType.Call))).Verifiable();
            mock2.Setup(p => p.ActionEvent(1, It.Is<PokerAction>(a => a.Type == ActionType.Fold))).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveGameStateChanged()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.GameStateChanged()).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();
            mock2.Setup(p => p.GameStateChanged()).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveRoundChanged()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.RoundChanged(1)).Verifiable();
            mock1.Setup(p => p.RoundChanged(2)).Verifiable();
            mock1.Setup(p => p.RoundChanged(3)).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock2.Setup(p => p.RoundChanged(1)).Verifiable();
            mock2.Setup(p => p.RoundChanged(2)).Verifiable();
            mock2.Setup(p => p.RoundChanged(3)).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveGameOverEvent()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.GameOverEvent()).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();
            mock2.Setup(p => p.GameOverEvent()).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveShowdownEvent()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.ShowdownEvent(0, It.Is<Card[]>(c => IsValidHand(c)))).Verifiable();
            mock1.Setup(p => p.ShowdownEvent(1, It.Is<Card[]>(c => IsValidHand(c)))).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock2.Setup(p => p.ShowdownEvent(0, It.Is<Card[]>(c => IsValidHand(c)))).Verifiable();
            mock2.Setup(p => p.ShowdownEvent(1, It.Is<Card[]>(c => IsValidHand(c)))).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_TwoPlayers_PlayerShouldReceiveWinEvent()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            mock1.Setup(p => p.WinEvent(0, It.Is<long>(w => w > 0))).Verifiable();

            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateFoldAction()).Verifiable();
            mock2.Setup(p => p.WinEvent(0, It.Is<long>(w => w > 0))).Verifiable();

            hand.Sit(mock1.Object, 0, 1000);
            hand.Sit(mock2.Object, 1, 1000);

            hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_SetupHandHistoryScript_MockPlayerShouldNeverBeCalled()
        {
            HandHistory script = new HandHistory();

            var mock = new Mock<OmahaPlayer>();
            mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 5;
            script.BigBlindSeat = 2;
            script.BigBlindAmount = 10;

            script.Players[0] = new HandHistoryPlayer("John", 2300);
            script.Players[1] = new HandHistoryPlayer("Billy", 2800);
            script.Players[2] = new HandHistoryPlayer("Roger", 3000, mock.Object);
            script.Players[5] = new HandHistoryPlayer("Nick", 1000);

            script.Players[2].HoleCards = new Card[] { new Card("5s"), new Card("Js"), new Card("5h"), new Card("Qs") };

            script.Actions.Add(new HandHistoryPokerAction(script.Players[5], PokerAction.CreateFoldAction()));
            script.Actions.Add(new HandHistoryPokerAction(script.Players[0], PokerAction.CreateFoldAction()));
            script.Actions.Add(new HandHistoryPokerAction(script.Players[1], PokerAction.CreateFoldAction()));

            OmahaHiLoHand hand = new OmahaHiLoHand(script);

            Assert.AreEqual("Roger", hand.RunToEnd().Winners[0].Player.Name);

            mock.Verify();
        }

        [Test]
        public void RunToEnd_SetupHandHistoryScript_PlayersShouldGetCorrectCards()
        {
            HandHistory script = new HandHistory();

            var mock = new Mock<OmahaPlayer>();
            mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 5;
            script.BigBlindSeat = 0;
            script.BigBlindAmount = 10;

            script.Players[0] = new HandHistoryPlayer("John", 2300, mock.Object);
            script.Players[1] = new HandHistoryPlayer("Billy", 2800, mock.Object);

            Card[] expected0 = new Card[] { new Card("8s"), new Card("3s"), new Card("Ad"), new Card("Jc") };
            Card[] expected1 = new Card[] { new Card("5s"), new Card("Js"), new Card("5h"), new Card("Qs") };

            script.Players[0].HoleCards = expected0;
            script.Players[1].HoleCards = expected1;

            OmahaHiLoHand hand = new OmahaHiLoHand(script);

            hand.SimulateNext();

            Assert.AreEqual(expected0, script.Players[0].HoleCards);
            Assert.AreEqual(expected1, script.Players[1].HoleCards);
        }

        [Test]
        public void RunToEnd_SeededDeck_Players2ShouldWin()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();
            hand.Seed = 1;

            var player1 = new Mock<OmahaPlayer>(); //// AAT7 (AA669)
            var player2 = new Mock<OmahaPlayer>(); //// 73JQ (333JQ) -- should be winner
                                                   //// 69336 (common)

            player1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();
            player2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction()).Verifiable();

            hand.Sit(player1.Object, 0, 1000);
            hand.Sit(player2.Object, 1, 1000);

            HandResult result = hand.RunToEnd();

            Assert.AreEqual(1, result.Winners.Count);
            Assert.AreEqual(player2.Object, result.Winners[0].Player);

            player1.Verify();
            player2.Verify();
        }

        [Test]
        public void Clone_SingleHandWithSeed_ShouldReturnExactHand()
        {
            OmahaHiLoHand hand = new OmahaHiLoHand();
            hand.Seed = 1;

            var mock1 = new Mock<OmahaPlayer>();
            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            var mock2 = new Mock<OmahaPlayer>();
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            hand.Sit(mock1.Object, 3, 1000);
            hand.Sit(mock2.Object, 4, 1000);

            OmahaHiLoHand cloned = (OmahaHiLoHand)hand.Clone();

            HandResult hr1 = hand.RunToEnd();
            HandResult hr2 = cloned.RunToEnd();

            Assert.AreEqual(hr1.Winners[0].Player.HoleCards, hr2.Winners[0].Player.HoleCards);
        }

        [Test]
        public void Clone_HandHistoryHandWithThreePlayers_ShouldReturnExactHand()
        {
            HandHistory script = new HandHistory();

            var mock = new Mock<OmahaPlayer>();
            mock.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 5;
            script.BigBlindSeat = 2;
            script.BigBlindAmount = 10;

            script.Players[0] = new HandHistoryPlayer("Bob", 2300);
            script.Players[1] = new HandHistoryPlayer("Kevin", 2800);
            script.Players[2] = new HandHistoryPlayer("Burger", 2800);

            script.Players[0].HoleCards = new Card[] { new Card("5s"), new Card("Js"), new Card("7h"), new Card("Qs") };
            script.Players[1].HoleCards = new Card[] { new Card("3s"), new Card("3d"), new Card("7d"), new Card("Ah") };
            script.Players[2].HoleCards = new Card[] { new Card("2s"), new Card("Jh"), new Card("3h"), new Card("Ad") };

            script.Actions.Add(new HandHistoryPokerAction(script.Players[0], PokerAction.CreateCallAction()));
            script.Actions.Add(new HandHistoryPokerAction(script.Players[1], PokerAction.CreateCallAction()));
            script.Actions.Add(new HandHistoryPokerAction(script.Players[2], PokerAction.CreateCallAction()));

            OmahaHiLoHand hand = new OmahaHiLoHand(script);
            OmahaHiLoHand cloned = (OmahaHiLoHand)hand.Clone();

            HandResult hr1 = hand.RunToEnd();
            HandResult hr2 = cloned.RunToEnd();

            // This isn't perfect, but for now this is fine
            Assert.AreEqual(hr1.Winners[0].Player.HoleCards, hr2.Winners[0].Player.HoleCards);
        }

        [Test]
        public void RunToEnd_Player1HasHighPlayer2HasLow_ShouldBeTwoWinners()
        {
            HandHistory script = new HandHistory();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();
            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 5;
            script.BigBlindSeat = 0;
            script.BigBlindAmount = 10;

            script.Players[0] = new HandHistoryPlayer("John", 1000, mock1.Object);
            script.Players[1] = new HandHistoryPlayer("Bill", 1000, mock2.Object);

            script.Players[0].HoleCards = CardHelper.CreateHandFromString("Ah Ad 7h 8h");
            script.Players[1].HoleCards = CardHelper.CreateHandFromString("4h 6d Th Jh");

            script.FlopCards = CardHelper.CreateHandFromString("As 2s 3s");
            script.TurnCard = new Card(HoldemHand.Hand.Rank8, 0);
            script.RiverCard = new Card(HoldemHand.Hand.Rank9, 0);

            OmahaHiLoHand hand = new OmahaHiLoHand(script);

            HandResult hr = hand.RunToEnd();

            Assert.AreEqual(2, hr.Winners.Count);
        }

        [Test]
        public void RunToEnd_Player1HasHighAndLowPlayer2HasLow_Player1ShouldHave3TimesWinningAmoutAsPlayer2()
        {
            HandHistory script = new HandHistory();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();
            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock1.Setup(p => p.WinEvent(0, 300)).Verifiable();
            mock1.Setup(p => p.WinEvent(1, 100)).Verifiable();
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock2.Setup(p => p.WinEvent(0, 300)).Verifiable();
            mock2.Setup(p => p.WinEvent(1, 100)).Verifiable();

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 100;
            script.BigBlindSeat = 0;
            script.BigBlindAmount = 200;

            script.Players[0] = new HandHistoryPlayer("John", 1000, mock1.Object);
            script.Players[1] = new HandHistoryPlayer("Bill", 1000, mock2.Object);

            script.Players[0].HoleCards = CardHelper.CreateHandFromString("4s 5s 6h 7h"); //// Straight flush!
            script.Players[1].HoleCards = CardHelper.CreateHandFromString("4h 5d 6h 7h");

            script.FlopCards = CardHelper.CreateHandFromString("As 2s 3s");
            script.TurnCard = new Card(HoldemHand.Hand.Rank8, 0);
            script.RiverCard = new Card(HoldemHand.Hand.Rank9, 0);

            OmahaHiLoHand hand = new OmahaHiLoHand(script);

            HandResult hr = hand.RunToEnd();

            mock1.Verify();
            mock2.Verify();
        }

        [Test]
        public void RunToEnd_Player1HasHighPlayer2HasLow_AmountWonShouldBeEqual()
        {
            HandHistory script = new HandHistory();

            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();
            mock1.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());
            mock2.Setup(p => p.Act()).Returns(PokerAction.CreateCallAction());

            script.ButtonSeat = 0;
            script.SmallBlindSeat = 1;
            script.SmallBlindAmount = 5;
            script.BigBlindSeat = 0;
            script.BigBlindAmount = 10;

            script.Players[0] = new HandHistoryPlayer("John", 1000, mock1.Object);
            script.Players[1] = new HandHistoryPlayer("Bill", 1000, mock2.Object);

            script.Players[0].HoleCards = CardHelper.CreateHandFromString("Ah Ad 7h 8h");
            script.Players[1].HoleCards = CardHelper.CreateHandFromString("4h 6d Th Jh");

            script.FlopCards = CardHelper.CreateHandFromString("As 2s 3s");
            script.TurnCard = new Card(HoldemHand.Hand.Rank8, 0);
            script.RiverCard = new Card(HoldemHand.Hand.Rank9, 0);

            OmahaHiLoHand hand = new OmahaHiLoHand(script);

            HandResult hr = hand.RunToEnd();

            Assert.AreEqual(2, hr.Winners.Count);
            Assert.Greater(hr.Winners[0].Amount, 0);
            Assert.AreEqual(hr.Winners[0].Amount, hr.Winners[1].Amount);
        }

        private bool IsValidHand(Card[] hand)
        {
            if (hand.Length < 2)
            {
                return false;
            }

            foreach (Card card in hand)
            {
                if (card.Rank < 0 || card.Rank > 12 || card.Suit < 0 || card.Suit > 3)
                {
                    return false;
                }
            }

            return true;
        }
    }
}

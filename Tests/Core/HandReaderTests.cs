namespace OmahaBot.Tests.Core
{
    using NUnit.Framework;
    using OmahaBot.Core;
    using UnitTestUtil;

    [TestFixture]
    public class HandReaderTests
    {
        [Test]
        public void Parse_PokerStarsTournamentHand_ReturnsPokerStars()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Assert.AreEqual(PokerSite.PokerStars, handhistory.Site);
        }

        [Test]
        public void Parse_HandWithNinePlayers_ReturnsNinePlayers()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Assert.AreEqual(9, handhistory.Players.Count);
        }

        [Test]
        public void Parse_PSHand32738135666_GetsCorrectPlayerNames()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.AreEqual("jbyrne1949", handhistory.Players[1].Name);
            Assert.AreEqual("bsharp17", handhistory.Players[2].Name);
            Assert.AreEqual("Cajun 1986", handhistory.Players[3].Name);
            Assert.AreEqual("DimalASS", handhistory.Players[4].Name);
        }

        [Test]
        public void Parse_PSHand32738412212_GetsCorrectInitialBankroll()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738412212"));

            Assert.AreEqual(5670, handhistory.Players[1].Bankroll);
            Assert.AreEqual(8200, handhistory.Players[2].Bankroll);
            Assert.AreEqual(2920, handhistory.Players[4].Bankroll);
            Assert.AreEqual(3570, handhistory.Players[5].Bankroll);
            Assert.AreEqual(6350, handhistory.Players[6].Bankroll);
            Assert.AreEqual(6240, handhistory.Players[7].Bankroll);
            Assert.AreEqual(3130, handhistory.Players[8].Bankroll);
            Assert.AreEqual(2920, handhistory.Players[9].Bankroll);
        }

        [Test]
        public void Parse_PSHand32738135666_GetPlayerHoleCards()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Card[] cards = handhistory.Players[2].HoleCards;
            Assert.AreEqual("2h", cards[0].ToString());
            Assert.AreEqual("8d", cards[1].ToString());
            Assert.AreEqual("Qc", cards[2].ToString());
            Assert.AreEqual("7s", cards[3].ToString());
        }

        [Test]
        public void Parse_PSHand32616487533_GetFlopCards()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Card[] cards = handhistory.FlopCards;
            Assert.AreEqual("6c", cards[0].ToString());
            Assert.AreEqual("4h", cards[1].ToString());
            Assert.AreEqual("6h", cards[2].ToString());
        }

        [Test]
        public void Parse_PSHand32616487533_GetTurnCard()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Assert.AreEqual("8d", handhistory.TurnCard.ToString());
        }

        [Test]
        public void Parse_PSHand32616487533_GetRiverCard()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Assert.AreEqual("Qc", handhistory.RiverCard.ToString());
        }

        [Test]
        public void Parse_HandSmallBlindIsSeat3_Return3()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32616487533"));

            Assert.AreEqual(3, handhistory.SmallBlindSeat);
        }

        [Test]
        public void Parse_HandBigBlindIsSeat2_Return2()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.AreEqual(2, handhistory.BigBlindSeat);
        }

        [Test]
        public void Parse_HandButtonSeatIsSeat4_Return4()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.AreEqual(4, handhistory.ButtonSeat);
        }

        [Test]
        public void Parse_Hand32738135666_ReturnSmallBlindAmount()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.AreEqual(75, handhistory.SmallBlindAmount);
        }

        [Test]
        public void Parse_Hand32738135666_ReturnBigBlindAmount()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.AreEqual(150, handhistory.BigBlindAmount);
        }

        [Test]
        public void Parse_Hand32738135666_ReturnCajun1986Called()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            HandHistoryPokerAction action = handhistory.Actions[0];
            Assert.AreEqual(ActionType.Call, action.Action.Type);
            Assert.AreEqual(150, action.Action.Bet);
            Assert.AreEqual("Cajun 1986", action.Player.Name);
        }

        [Test]
        public void Parse_Hand32738135666_Returnjbyrne1949RaisedTo750()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            HandHistoryPokerAction action = handhistory.Actions[2];
            Assert.AreEqual(ActionType.Raise, action.Action.Type);
            Assert.AreEqual(750, action.Action.Bet);
            Assert.AreEqual("jbyrne1949", action.Player.Name);
        }

        [Test]
        public void Parse_AnyHand_ShowdownHasOneOrMoreWinner()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32738135666"));

            Assert.GreaterOrEqual(handhistory.Winners.Count, 1);
        }

        [Test]
        public void Parse_Hand32774709403_Declare4wheelin007WinnerPot3318()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32776254738"));

            Assert.AreEqual("4wheelin007", handhistory.Winners[0].First.Name);
            Assert.AreEqual(150, handhistory.Winners[0].Second);
        }

        [Test]
        public void Parse_HandWithSidePot_DeclareWhipp24WinnerPot6446()
        {
            HandReader reader = new HandReader();
            HandHistory handhistory = reader.Parse(HandHistoriesHelper.GetFile("PokerStars/32774709403"));

            Assert.AreEqual("whipp24", handhistory.Winners[0].First.Name);
            Assert.AreEqual(6446, handhistory.Winners[0].Second);
        }
    }
}

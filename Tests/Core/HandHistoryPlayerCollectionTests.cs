namespace OmahaBot.Tests.Core
{
    using NUnit.Framework;
    using OmahaBot.Core;

    [TestFixture]
    public class HandHistoryPlayerCollectionTests
    {
        [Test]
        public void BracketOperator_PlayerSeat_SetPlayerAtThatSeat()
        {
            HandHistoryPlayerCollection players = new HandHistoryPlayerCollection();
            HandHistoryPlayer player = new HandHistoryPlayer("Nothing", 100);
            players[3] = player;
            Assert.AreSame(player, players[3]);
        }

        [Test]
        public void BracketOperatorInt_DontSet_ReturnEmptyPlayer()
        {
            HandHistoryPlayerCollection players = new HandHistoryPlayerCollection();
            Assert.AreSame(HandHistoryPlayer.Empty, players[3]);
        }

        [Test]
        public void BracketOperatorString_DontSet_ReturnEmptyPlayer()
        {
            HandHistoryPlayerCollection players = new HandHistoryPlayerCollection();
            Assert.AreSame(HandHistoryPlayer.Empty, players["somebody"]);
        }
    }
}

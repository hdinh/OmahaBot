namespace OmahaBot.Tests.Core
{
    using Moq;
    using NUnit.Framework;
    using OmahaBot.Core;

    [TestFixture]
    public class OmahaPlayerTests
    {
        [Test]
        public void UniqueId_TwoDifferentPlayers_VerifyIdsAreDifferent()
        {
            var mock1 = new Mock<OmahaPlayer>();
            var mock2 = new Mock<OmahaPlayer>();

            Assert.AreNotEqual(mock1.Object.Id, mock2.Object.Id);
            Assert.AreNotEqual(mock1.Object.Id.Value, mock2.Object.Id.Value);
            Assert.AreNotEqual(mock1.Object.Id.ToString(), mock2.Object.Id.ToString());
        }
    }
}

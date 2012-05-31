namespace OmahaBot.Tests.Core
{
    using NUnit.Framework;
    using OmahaBot.Core;
    using UnitTestUtil;

    [TestFixture]
    public class OmahaHandHashTests
    {
        [Test]
        public void GetHashCode_TwoSameHandsWithDifferentSuits_ShouldReturnSameHashCode()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("As 2s 3s 4s");
            Card[] hand2 = CardHelper.CreateHandFromString("Ad 2d 3d 4d");

            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            ulong hash2 = OmahaHandHash.GetHashCode(hand2);

            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void GetHashCode_TwoHandsWithTwoSuits_ShouldReturnSameHashCode()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("Ks Kd Qs Qd");
            Card[] hand2 = CardHelper.CreateHandFromString("Kc Kh Qc Qh");

            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            ulong hash2 = OmahaHandHash.GetHashCode(hand2);

            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void GetHashCode_TwoHandsWithThreeSuits_ShouldReturnSameHashCode()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("As 2s 3s 4d");
            Card[] hand2 = CardHelper.CreateHandFromString("Ad 2d 3d 4h");

            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            ulong hash2 = OmahaHandHash.GetHashCode(hand2);

            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void GetHashCode_TwoHandsWithFourSuits_ShouldReturnSameHashCode()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("5s 8d 9h 2c");
            Card[] hand2 = CardHelper.CreateHandFromString("2h 8c 9d 5s");

            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            ulong hash2 = OmahaHandHash.GetHashCode(hand2);

            Assert.AreEqual(hash1, hash2);
        }

        [Test]
        public void ToString_FourDifferentSuits_ShouldReturnSameHand()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("5s 8d 9h 2c");
            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            Assert.AreEqual("9a 8b 5c 2d", OmahaHandHash.ToString(hash1));
        }

        [Test]
        public void ToString_TwoSuits_ShouldReturnSameHand()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("8s 9s Th Jh");
            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            Assert.AreEqual("Ta Ja 8b 9b", OmahaHandHash.ToString(hash1));
        }

        [Test]
        public void ToString_FourSuits_ShouldReturnSameHand()
        {
            Card[] hand1 = CardHelper.CreateHandFromString("3s 4s 5s 6s");
            ulong hash1 = OmahaHandHash.GetHashCode(hand1);
            Assert.AreEqual("3a 4a 5a 6a", OmahaHandHash.ToString(hash1));
        }
    }
}

namespace OmahaBot.Tests.Core
{
    using NUnit.Framework;
    using OmahaBot.Core;
    using UnitTestUtil;

    [TestFixture]
    public class OmahaHandLowEvaluatorTests
    {
        [Test]
        public void Evaluate_A3QK_237TJ_A2357()
        {
            Card[] handCards1 = CardHelper.CreateHandFromString("As 3s Qs Ks");
            Card[] handCards2 = CardHelper.CreateHandFromString("As 4s Qs Ks");
            Card[] commonCards = CardHelper.CreateHandFromString("2s 5s 7s Ts Js");

            uint best1 = OmahaHandLowEvaluator.Evaluate(handCards1, commonCards);
            uint best2 = OmahaHandLowEvaluator.Evaluate(handCards2, commonCards);

            Assert.Greater(best1, best2);
        }

        [Test]
        public void Evaluate_A234_A235Q_A2345()
        {
            Card[] handCards1 = CardHelper.CreateHandFromString("As 2s 3s 4s");
            Card[] handCards2 = CardHelper.CreateHandFromString("Ad 6d Qd Kd");
            Card[] commonCards = CardHelper.CreateHandFromString("Ac 2c 3c 4c 5c");

            uint best1 = OmahaHandLowEvaluator.Evaluate(handCards1, commonCards);
            uint best2 = OmahaHandLowEvaluator.Evaluate(handCards2, commonCards);

            Assert.Greater(best1, best2);
        }

        [Test]
        public void Evaluate_A234_56QKK_LowNotPossible()
        {
            Card[] handCards = CardHelper.CreateHandFromString("As 2s 3s 4s");
            Card[] commonCards = CardHelper.CreateHandFromString("5s 6s Qs Ks Kd");

            uint value = OmahaHandLowEvaluator.Evaluate(handCards, commonCards);

            Assert.AreEqual(0, value);
        }

        [Test]
        public void Evaluate_A234_A23KQ_LowNotPossible()
        {
            Card[] handCards = CardHelper.CreateHandFromString("As 2s 3s 4s");
            Card[] commonCards = CardHelper.CreateHandFromString("Ac 2c 3c Kc Qc");

            uint value = OmahaHandLowEvaluator.Evaluate(handCards, commonCards);

            Assert.AreEqual(0, value);
        }

        [Test]
        public void Evaluate_89TJ_23456_LowNotPossible()
        {
            Card[] hands = CardHelper.CreateHandFromString("8s 9s 10s Js");
            Card[] common = CardHelper.CreateHandFromString("2c 3c 4c 5c 6c");

            uint value = OmahaHandLowEvaluator.Evaluate(hands, common);

            Assert.AreEqual(0, value);
        }
    }
}

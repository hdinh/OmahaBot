namespace OmahaBot.Tests.Core
{
    using NUnit.Framework;
    using UnitTestUtil;

    [TestFixture]
    public class OmahaHandHighEvaluatorTests
    {
        [Test]
        public void Evaluate_2345_Q34JK_3344K()
        {
            OmahaEvalAssert.AreEqual("2s 3s 4s 5s", "Qd 3c 4c Js Ks", "3s 3c 4s 4c Ks");
        }

        [Test]
        public void Evaluate_JJJJ_AAAQQ_AAAJJ()
        {
            OmahaEvalAssert.AreEqual("Js Jc Jd Jh", "As Ac Ad Qh Qd", "As Ac Ad Jc Jh");
        }
    }
}

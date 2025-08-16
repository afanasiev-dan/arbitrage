using Arbitrage.Core.Calclucation;

namespace Arbitrage.MSTests
{
    [TestClass]
    public sealed class MathCupsTests
    {
        [TestMethod]
        public void ExchangeRateSpread_Test()
        {
            decimal a = 37.27m;
            decimal b = 35.56m;
            decimal expected = 4.81m;

            decimal result = MathCups.ExchangeRateSpread(b, a);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]

        public void AvgPricePos_Test()
        {
            decimal[] mas = { 1, 2, 3 };
            decimal expected = 2;

            decimal result = MathCups.AvgPricePos(mas);
            Assert.AreEqual(expected, result);
        }
    }
}

using Arbitrage.Core.Calclucation;

namespace Arbitrage.MSTests
{
    [TestClass]
    public sealed class CupsCalculationTest
    {
        [TestMethod]
        public void ExchangeRateSpred_Test()
        {
            decimal[] shortPos = { 1, 2, 3, 6 };
            decimal[] longPos = { 1, 2, 3 };
            decimal expected = 50;

            decimal result = CupsCalculation.SpredCalculationBetweenTwoExchanges(longPos, shortPos);

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void CalculatingTotalVolumeAndAvgPrice_Test()
        {
            decimal[] avgPrice = { 1, 2, 3};
            decimal[] totalVolume = { 100, 150, 200 };
            (decimal, decimal) expected = (450, 2);

            (decimal, decimal) result = CupsCalculation.CalculatingTotalVolumeAndAvgPrice(totalVolume, avgPrice);

            Assert.AreEqual(expected, result);
        }
    }
}

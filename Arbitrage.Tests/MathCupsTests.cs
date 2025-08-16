using Arbitrage.Core.Calclucation;
using Xunit;

namespace Arbitrage.Tests
{
    public class MathCupsTests
    {
        [Fact]
        public void ExchangeRateSpread_Test()
        {
            decimal a = 37.27m;
            decimal b = 35.56m;
            decimal expected = 4.81m;

            decimal result = MathCups.ExchangeRateSpread(b, a);

            Assert.Equal(expected, result);
        }
    }
}

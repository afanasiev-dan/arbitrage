using Arbitrage.Symbols.Presentation.Dto.CurrencyPair;

namespace DataSocketService.Model
{
    public class CurrencyPairBook
    {
        public CurrencyPairResponceDto Info;
        public Book Book;

        public CurrencyPairBook(CurrencyPairResponceDto info)
        {
            Info = info;
            Book = new Book();
        }
    }

    public class Book
    {
        public List<(decimal price, decimal volume)> Asks = new();
        public List<(decimal price, decimal volume)> Bids = new();
        public DateTime Time;

        public void ChangeDt(DateTime time)
        {
            Time = time;
        }
    }
}

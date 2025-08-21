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
}

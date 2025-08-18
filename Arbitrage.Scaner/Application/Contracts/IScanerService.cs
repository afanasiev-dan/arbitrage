using Arbitrage.Scaner.Domain.Entities;
using Arbitrage.Scaner.Presentation.Dto;

namespace Arbitrage.Scaner.Application.Contracts
{
    public interface IScanerService
    {
        Task<IEnumerable<ScanerModel>> GetScaners();
        Task<bool> AddScaners(IEnumerable<ScanerAddDataRequestDto> scanerModels);
    }
}
using Arbitrage.Scaner.Domain.Entities;

namespace Arbitrage.Scaner.Domain.Contracts;

public interface IScanerRepository{
  Task<IEnumerable<ScanerModel>> GetScaners();
  Task<bool> AddScaners(IEnumerable<ScanerModel> scanerModels);
}

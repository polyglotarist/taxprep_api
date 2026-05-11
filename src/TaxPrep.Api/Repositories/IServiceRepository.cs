using TaxPrep.Api.Models;
namespace TaxPrep.Api.Repositories;

public interface IServiceRepository
{
    Task<bool> CodeExistsAsync(string code, CancellationToken ct = default);
    Task<Service> AddAsync(Service service, CancellationToken ct = default);
    Task<Service?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Service>> ListAsync(CancellationToken ct = default);
}

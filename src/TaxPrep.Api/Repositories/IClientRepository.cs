using TaxPrep.Api.Models;
namespace TaxPrep.Api.Repositories;

public interface IClientRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task<Client> AddAsync(Client client, CancellationToken ct = default);
    Task<Client?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Client>> ListAsync(CancellationToken ct = default);
    Task DeleteAsync(Client client, CancellationToken ct = default);
}

using TaxPrep.Api.Models;
namespace TaxPrep.Api.Repositories;

public interface IEngagementRepository
{
    Task<bool> ClientExistsAsync(int clientId, CancellationToken ct = default);
    Task<bool> ServiceExistsAsync(int serviceId, CancellationToken ct = default);
    Task<bool> ExistsAsync(int clientId, int serviceId, short taxYear, CancellationToken ct = default);
    Task<Engagement?> GetAsync(int id, CancellationToken ct = default);
    Task<Engagement> AddAsync(Engagement e, CancellationToken ct = default);
    Task UpdateAsync(Engagement e, CancellationToken ct = default);
    Task DeleteAsync(Engagement e, CancellationToken ct = default);
    IQueryable<Engagement> Query();
}

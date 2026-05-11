using Microsoft.EntityFrameworkCore;
using TaxPrep.Api.Data;
using TaxPrep.Api.Models;

namespace TaxPrep.Api.Repositories;

public class EngagementRepository : IEngagementRepository
{
    private readonly TaxPrepDbContext _db;
    public EngagementRepository(TaxPrepDbContext db) => _db = db;

    public Task<bool> ClientExistsAsync(int clientId, CancellationToken ct = default)
        => _db.Clients.AnyAsync(x => x.Id == clientId, ct);

    public Task<bool> ServiceExistsAsync(int serviceId, CancellationToken ct = default)
        => _db.Services.AnyAsync(x => x.Id == serviceId, ct);

    public Task<bool> ExistsAsync(int clientId, int serviceId, short taxYear, CancellationToken ct = default)
        => _db.Engagements.AnyAsync(e => e.ClientId == clientId && e.ServiceId == serviceId && e.TaxYear == taxYear, ct);

    public Task<Engagement?> GetAsync(int id, CancellationToken ct = default)
        => _db.Engagements.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Engagement> AddAsync(Engagement e, CancellationToken ct = default)
    { _db.Engagements.Add(e); await _db.SaveChangesAsync(ct); return e; }

    public async Task UpdateAsync(Engagement e, CancellationToken ct = default)
    { _db.Engagements.Update(e); await _db.SaveChangesAsync(ct); }

    public async Task DeleteAsync(Engagement e, CancellationToken ct = default)
    { _db.Engagements.Remove(e); await _db.SaveChangesAsync(ct); }

    public IQueryable<Engagement> Query() => _db.Engagements.AsNoTracking();
}

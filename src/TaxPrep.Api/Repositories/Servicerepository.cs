using Microsoft.EntityFrameworkCore;
using TaxPrep.Api.Data;
using TaxPrep.Api.Models;

namespace TaxPrep.Api.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly TaxPrepDbContext _db;
    public ServiceRepository(TaxPrepDbContext db) => _db = db;

    public Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
        => _db.Services.AnyAsync(s => s.Code == code, ct);

    public async Task<Service> AddAsync(Service service, CancellationToken ct = default)
    { _db.Services.Add(service); await _db.SaveChangesAsync(ct); return service; }

    public Task<Service?> GetAsync(int id, CancellationToken ct = default)
        => _db.Services.FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<Service>> ListAsync(CancellationToken ct = default)
        => _db.Services.AsNoTracking().OrderBy(s => s.Code).ToListAsync(ct);
}

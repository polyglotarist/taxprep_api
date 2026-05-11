using Microsoft.EntityFrameworkCore;
using TaxPrep.Api.Data;
using TaxPrep.Api.Models;

namespace TaxPrep.Api.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly TaxPrepDbContext _db;
    public ClientRepository(TaxPrepDbContext db) => _db = db;

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        => _db.Clients.AnyAsync(c => c.Email == email, ct);

    public async Task<Client> AddAsync(Client client, CancellationToken ct = default)
    { _db.Clients.Add(client); await _db.SaveChangesAsync(ct); return client; }

    public Task<Client?> GetAsync(int id, CancellationToken ct = default)
        => _db.Clients.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<List<Client>> ListAsync(CancellationToken ct = default)
        => _db.Clients.AsNoTracking().OrderBy(c => c.FullName).ToListAsync(ct);

    public async Task DeleteAsync(Client client, CancellationToken ct = default)
    { _db.Clients.Remove(client); await _db.SaveChangesAsync(ct); }
}

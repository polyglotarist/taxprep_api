using TaxPrep.Api.Dtos;
using TaxPrep.Api.Mapping;
using TaxPrep.Api.Models;
using TaxPrep.Api.Repositories;

namespace TaxPrep.Api.Services;

public class ClientService
{
    private readonly IClientRepository _repo;
    public ClientService(IClientRepository repo) => _repo = repo;

    public async Task<(bool Ok, string? Error, Client? Created)> CreateAsync(CreateClientDto dto, CancellationToken ct = default)
    {
        if (await _repo.EmailExistsAsync(dto.Email, ct))
            return (false, "Email already exists.", null);

        var created = await _repo.AddAsync(dto.ToEntity(), ct);
        return (true, null, created);
    }
}

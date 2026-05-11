using TaxPrep.Api.Dtos;
using TaxPrep.Api.Mapping;
using TaxPrep.Api.Models;
using TaxPrep.Api.Repositories;

namespace TaxPrep.Api.Services;

public class ServiceCatalogService
{
    private readonly IServiceRepository _repo;
    public ServiceCatalogService(IServiceRepository repo) => _repo = repo;

    public async Task<(bool Ok, string? Error, Service? Created)> CreateAsync(CreateServiceDto dto, CancellationToken ct = default)
    {
        if (await _repo.CodeExistsAsync(dto.Code, ct))
            return (false, "Service code already exists.", null);

        var created = await _repo.AddAsync(dto.ToEntity(), ct);
        return (true, null, created);
    }
}

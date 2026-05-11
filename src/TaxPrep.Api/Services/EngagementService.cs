using TaxPrep.Api.Dtos;
using TaxPrep.Api.Models;
using TaxPrep.Api.Repositories;

namespace TaxPrep.Api.Services;

public class EngagementService
{
    private readonly IEngagementRepository _repo;
    public EngagementService(IEngagementRepository repo) => _repo = repo;

    public async Task<(bool Ok, string? Error, Engagement? Created)> CreateAsync(CreateEngagementDto dto, CancellationToken ct = default)
    {
        if (!await _repo.ClientExistsAsync(dto.ClientId, ct))  return (false, "Client not found.", null);
        if (!await _repo.ServiceExistsAsync(dto.ServiceId, ct)) return (false, "Service not found.", null);
        if (await _repo.ExistsAsync(dto.ClientId, dto.ServiceId, dto.TaxYear, ct))
            return (false, "Engagement already exists for that client/service/year.", null);

        var e = new Engagement { ClientId = dto.ClientId, ServiceId = dto.ServiceId, TaxYear = dto.TaxYear, Status = "Draft" };
        var created = await _repo.AddAsync(e, ct);
        return (true, null, created);
    }

    public async Task<(bool Ok, string? Error)> UpdateAsync(int id, string? status, DateTime? signedAtUtc, CancellationToken ct = default)
    {
        var e = await _repo.GetAsync(id, ct);
        if (e is null) return (false, "Engagement not found.");
        if (e.Status == "Filed") return (false, "Filed engagements cannot be modified.");

        if (status is not null) e.Status = status;
        if (signedAtUtc is not null) e.SignedAtUtc = signedAtUtc;

        await _repo.UpdateAsync(e, ct);
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> DeleteAsync(int id, CancellationToken ct = default)
    {
        var e = await _repo.GetAsync(id, ct);
        if (e is null) return (false, "Engagement not found.");
        if (e.Status == "Filed") return (false, "Filed engagements cannot be deleted.");
        await _repo.DeleteAsync(e, ct);
        return (true, null);
    }
}

using System.ComponentModel.DataAnnotations;

namespace TaxPrep.Api.Dtos;

public record CreateEngagementDto(
    [property: Range(1, int.MaxValue)] int ClientId,
    [property: Range(1, int.MaxValue)] int ServiceId,   // usually 1 (1040)
    [property: Range(1900, 3000)] short TaxYear
);

public record UpdateEngagementDto(string? Status, DateTime? SignedAtUtc);

public record EngagementResponseDto(int Id, int ClientId, int ServiceId, short TaxYear, string Status, DateTime? SignedAtUtc);

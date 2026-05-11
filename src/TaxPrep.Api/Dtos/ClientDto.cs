using System.ComponentModel.DataAnnotations;

namespace TaxPrep.Api.Dtos;

public record CreateClientDto(
    [property: Required, StringLength(120)] string FullName,
    [property: Required, EmailAddress, StringLength(200)] string Email,
    [property: StringLength(40)] string? Phone
);

public record UpdateClientDto(
    [property: StringLength(120)] string? FullName,
    [property: EmailAddress, StringLength(200)] string? Email,
    [property: StringLength(40)] string? Phone
);

public record ClientResponseDto(int Id, string FullName, string Email, string? Phone, DateTime CreatedAtUtc);

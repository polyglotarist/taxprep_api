using TaxPrep.Api.Dtos;
using TaxPrep.Api.Models;

namespace TaxPrep.Api.Mapping;

public static class DtoMappers
{
    public static Client ToEntity(this CreateClientDto dto) => new()
    { FullName = dto.FullName, Email = dto.Email, Phone = dto.Phone };

    public static void Apply(this UpdateClientDto dto, Client c)
    {
        if (dto.FullName is not null) c.FullName = dto.FullName;
        if (dto.Email    is not null) c.Email = dto.Email;
        if (dto.Phone    is not null) c.Phone = dto.Phone;
    }

    public static ClientResponseDto ToDto(this Client c) =>
        new(c.Id, c.FullName, c.Email, c.Phone, c.CreatedAtUtc);

    public static Service ToEntity(this CreateServiceDto dto) => new()
    { Code = dto.Code, BaseFee = dto.BaseFee };

    public static ServiceResponseDto ToDto(this Service s) =>
        new(s.Id, s.Code, s.BaseFee);

    public static Engagement ToEntity(this CreateEngagementDto dto) => new()
    { ClientId = dto.ClientId, ServiceId = dto.ServiceId, TaxYear = dto.TaxYear, Status = "Draft" };

    public static void Apply(this UpdateEngagementDto dto, Engagement e)
    {
        if (dto.Status is not null) e.Status = dto.Status;
        if (dto.SignedAtUtc is not null) e.SignedAtUtc = dto.SignedAtUtc;
    }

    public static EngagementResponseDto ToDto(this Engagement e) =>
        new(e.Id, e.ClientId, e.ServiceId, e.TaxYear, e.Status, e.SignedAtUtc);
}

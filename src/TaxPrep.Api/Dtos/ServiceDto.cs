using System.ComponentModel.DataAnnotations;

namespace TaxPrep.Api.Dtos;

public record CreateServiceDto(
    [property: Required, StringLength(20)] string Code,
    [property: Range(0, 1_000_000)] decimal BaseFee
);

public record ServiceResponseDto(int Id, string Code, decimal BaseFee);

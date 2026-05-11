using System.ComponentModel.DataAnnotations;

namespace TaxPrep.Api.Dtos;

public static class ValidationExtensions
{
    public static bool TryValidate<T>(this T model, out Dictionary<string, string[]> errors)
    {
        var ctx = new ValidationContext(model!);
        var results = new List<ValidationResult>();
        var ok = Validator.TryValidateObject(model!, ctx, results, true);

        errors = results
            .SelectMany(r => r.MemberNames.DefaultIfEmpty(string.Empty),
                        (r, member) => new { member, r.ErrorMessage })
            .GroupBy(x => x.member)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage ?? "Invalid").ToArray());

        return ok;
    }
}


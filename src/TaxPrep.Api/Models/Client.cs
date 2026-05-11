namespace TaxPrep.Api.Models;

public class Client
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public ICollection<Engagement> Engagements { get; set; } = new List<Engagement>();
}

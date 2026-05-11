namespace TaxPrep.Api.Models;

public class Engagement
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int ServiceId { get; set; }     // will reference the single 1040 row
    public short TaxYear { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime? SignedAtUtc { get; set; }
    public Client? Client { get; set; }
    public Service? Service { get; set; }
}

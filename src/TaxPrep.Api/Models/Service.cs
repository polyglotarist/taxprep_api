namespace TaxPrep.Api.Models;

public class Service
{
    public int Id { get; set; }
    public string Code { get; set; } = "1040";
    public decimal BaseFee { get; set; }
    public ICollection<Engagement> Engagements { get; set; } = new List<Engagement>();
}

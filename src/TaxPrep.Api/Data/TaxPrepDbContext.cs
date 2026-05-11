using Microsoft.EntityFrameworkCore;
using TaxPrep.Api.Models;

namespace TaxPrep.Api.Data;

public class TaxPrepDbContext : DbContext
{
    public TaxPrepDbContext(DbContextOptions<TaxPrepDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Engagement> Engagements => Set<Engagement>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var cs = Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                    ?? "Server=localhost,1433;Database=TaxPrepDb;User Id=sa;Password=Password123;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(cs);
        }
    }


    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Client>(e =>
        {
            e.Property(p => p.FullName).HasMaxLength(120).IsRequired();
            e.Property(p => p.Email).HasMaxLength(200).IsRequired();
            e.HasIndex(p => p.Email).IsUnique();
            e.Property(p => p.Phone).HasMaxLength(40);
            e.Property(p => p.CreatedAtUtc).IsRequired();
        });

        model.Entity<Service>(e =>
        {
            e.Property(p => p.Code).HasMaxLength(20).IsRequired();
            e.HasIndex(p => p.Code).IsUnique();
            e.Property(p => p.BaseFee).HasColumnType("decimal(10,2)");
        });

        model.Entity<Engagement>(e =>
        {
            e.Property(p => p.Status).HasMaxLength(20).IsRequired(); // Draft|InReview|Filed|Cancelled
            e.HasAlternateKey(p => new { p.ClientId, p.ServiceId, p.TaxYear }); // unique per year
            e.HasOne(p => p.Client).WithMany(c => c.Engagements)
                .HasForeignKey(p => p.ClientId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Service).WithMany(s => s.Engagements)
                .HasForeignKey(p => p.ServiceId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(p => new { p.ClientId, p.TaxYear });
            e.HasIndex(p => new { p.ServiceId, p.TaxYear });
        });

        // Seed single simple offering: 1040 flat fee
        model.Entity<Service>().HasData(new Service { Id = 1, Code = "1040", BaseFee = 250.00m });
    }
}

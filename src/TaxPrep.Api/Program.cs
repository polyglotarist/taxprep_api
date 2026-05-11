using Microsoft.EntityFrameworkCore;
using Serilog;
using TaxPrep.Api.Data;
using TaxPrep.Api.Dtos;
using TaxPrep.Api.Mapping;
using TaxPrep.Api.Repositories;
using TaxPrep.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ----- Serilog -----
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

// ----- Swagger -----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new() { Title = "TaxPrep API", Version = "v1" })
);

// ----- Connection string (robust resolution) -----
// Order: appsettings.json -> env var -> local file (optional) -> hardcoded dev fallback
string? cs =
    builder.Configuration.GetConnectionString("Default")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

if (string.IsNullOrWhiteSpace(cs))
{
    // Optional: let devs put a full connection string in a local file next to the project
    var filePath = Path.Combine(builder.Environment.ContentRootPath, "ConnectionString.env");
    if (File.Exists(filePath))
        cs = File.ReadAllText(filePath).Trim();
}

// FINAL fallback (dev only). Keep in sync with your container's SA password.
cs ??= "Server=localhost,1433;Database=TaxPrepDb;User Id=sa;Password=Password123;TrustServerCertificate=True";

// ----- EF Core -----
builder.Services.AddDbContext<TaxPrepDbContext>(opt => opt.UseSqlServer(cs));

// ----- DI -----
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IEngagementRepository, EngagementRepository>();

builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<ServiceCatalogService>();
builder.Services.AddScoped<EngagementService>();

var app = builder.Build();

// ----- Swagger UI -----
app.UseSwagger();
app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "TaxPrep v1"));

// ----- Health -----
app.MapGet("/", () => Results.Ok(new { ok = true, service = "TaxPrep.Api" }));

// -------- Clients (CRUD) --------
var clients = app.MapGroup("/clients").WithTags("Clients");

clients.MapPost("/", async (ClientService svc, CreateClientDto dto, CancellationToken ct) =>
{
    if (!dto.TryValidate(out var errors)) return Results.ValidationProblem(errors);
    var (ok, err, created) = await svc.CreateAsync(dto, ct);
    return ok ? Results.Created($"/clients/{created!.Id}", created!.ToDto())
              : Results.Conflict(err);
})
.WithSummary("Create client").WithOpenApi();

clients.MapGet("/", async (IClientRepository repo, CancellationToken ct) =>
    Results.Ok((await repo.ListAsync(ct)).Select(c => c.ToDto())))
.WithSummary("List clients").WithOpenApi();

clients.MapGet("/{id:int}", async (IClientRepository repo, int id, CancellationToken ct) =>
{
    var c = await repo.GetAsync(id, ct);
    return c is null ? Results.NotFound() : Results.Ok(c.ToDto());
})
.WithSummary("Get client by id").WithOpenApi();

clients.MapPatch("/{id:int}", async (TaxPrepDbContext db, int id, UpdateClientDto dto, CancellationToken ct) =>
{
    var c = await db.Clients.FindAsync([id], ct);
    if (c is null) return Results.NotFound();
    dto.Apply(c);
    await db.SaveChangesAsync(ct);
    return Results.Ok(c.ToDto());
})
.WithSummary("Update client").WithOpenApi();

clients.MapDelete("/{id:int}", async (IClientRepository repo, int id, CancellationToken ct) =>
{
    var c = await repo.GetAsync(id, ct);
    if (c is null) return Results.NotFound();
    await repo.DeleteAsync(c, ct);
    return Results.NoContent();
})
.WithSummary("Delete client").WithOpenApi();

// -------- Services (simple catalog) --------
var services = app.MapGroup("/services").WithTags("Services");

services.MapGet("/", async (IServiceRepository repo, CancellationToken ct) =>
    Results.Ok(await repo.ListAsync(ct)))
.WithSummary("List services").WithOpenApi();

services.MapGet("/{id:int}", async (IServiceRepository repo, int id, CancellationToken ct) =>
{
    var s = await repo.GetAsync(id, ct);
    return s is null ? Results.NotFound() : Results.Ok(s.ToDto());
})
.WithSummary("Get service").WithOpenApi();

services.MapPost("/", async (ServiceCatalogService svc, CreateServiceDto dto, CancellationToken ct) =>
{
    if (!dto.TryValidate(out var errors)) return Results.ValidationProblem(errors);
    var (ok, err, created) = await svc.CreateAsync(dto, ct);
    return ok ? Results.Created($"/services/{created!.Id}", created!.ToDto())
              : Results.Conflict(err);
})
.WithSummary("Create service (usually 1040)").WithOpenApi();

// -------- Engagements (m-m with payload) --------
var engagements = app.MapGroup("/engagements").WithTags("Engagements");

engagements.MapGet("/", (IEngagementRepository repo, int? clientId, int? serviceId, short? year) =>
{
    var q = repo.Query();
    if (clientId is not null) q = q.Where(e => e.ClientId == clientId);
    if (serviceId is not null) q = q.Where(e => e.ServiceId == serviceId);
    if (year is not null) q = q.Where(e => e.TaxYear == year);
    return Results.Ok(q.Select(e => e.ToDto()).OrderBy(e => e.TaxYear).ToList());
})
.WithSummary("List engagements").WithOpenApi();

engagements.MapGet("/{id:int}", async (IEngagementRepository repo, int id, CancellationToken ct) =>
{
    var e = await repo.GetAsync(id, ct);
    return e is null ? Results.NotFound() : Results.Ok(e.ToDto());
})
.WithSummary("Get engagement").WithOpenApi();

engagements.MapPost("/", async (EngagementService svc, CreateEngagementDto dto, CancellationToken ct) =>
{
    if (!dto.TryValidate(out var errors)) return Results.ValidationProblem(errors);
    var (ok, err, created) = await svc.CreateAsync(dto, ct);
    return ok ? Results.Created($"/engagements/{created!.Id}", created!.ToDto())
              : Results.BadRequest(err);
})
.WithSummary("Create engagement (client ↔ service for a tax year)").WithOpenApi();

engagements.MapPatch("/{id:int}", async (EngagementService svc, int id, UpdateEngagementDto dto, CancellationToken ct) =>
{
    var (ok, err) = await svc.UpdateAsync(id, dto.Status, dto.SignedAtUtc, ct);
    return ok ? Results.Ok(new { ok = true }) : Results.BadRequest(err);
})
.WithSummary("Update engagement (status/signature)").WithOpenApi();

engagements.MapDelete("/{id:int}", async (EngagementService svc, int id, CancellationToken ct) =>
{
    var (ok, err) = await svc.DeleteAsync(id, ct);
    return ok ? Results.NoContent() : Results.BadRequest(err);
})
.WithSummary("Delete engagement").WithOpenApi();

app.Run();

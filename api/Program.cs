using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Allow Angular dev origin
builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:4200")
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
}

app.UseHttpsRedirection();
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/api/applications", async ([FromForm] ApplicationForm form) =>
{
    // Basic required checks (mirror the job form vibe)
    var errors = new List<string>();
    if (string.IsNullOrWhiteSpace(form.FirstName)) errors.Add("FirstName is required.");
    if (string.IsNullOrWhiteSpace(form.LastName)) errors.Add("LastName is required.");
    if (string.IsNullOrWhiteSpace(form.Email)) errors.Add("Email is required.");
    if (form.Resume is null || form.Resume.Length == 0) errors.Add("Resume is required.");

    if (errors.Any()) return Results.BadRequest(new { errors });

    // Validate file types
    var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".doc", ".docx", ".txt", ".rtf" };

    string? resumePath = null, coverPath = null;
    Directory.CreateDirectory("uploads");

    if (form.Resume is not null)
    {
        var ext = Path.GetExtension(form.Resume.FileName);
        if (!allowed.Contains(ext))
            return Results.BadRequest(new { errors = new[] { "Invalid resume file type." }});

        var target = Path.Combine("uploads", $"{Guid.NewGuid()}{ext}");
        using var s = File.Create(target);
        await form.Resume.CopyToAsync(s);
        resumePath = target;
    }

    if (form.CoverLetter is not null && form.CoverLetter.Length > 0)
    {
        var ext = Path.GetExtension(form.CoverLetter.FileName);
        if (!allowed.Contains(ext))
            return Results.BadRequest(new { errors = new[] { "Invalid cover letter file type." }});

        var target = Path.Combine("uploads", $"{Guid.NewGuid()}{ext}");
        using var s = File.Create(target);
        await form.CoverLetter.CopyToAsync(s);
        coverPath = target;
    }

    // Echo back (pretend create)
    return Results.Ok(new
    {
        id = Guid.NewGuid().ToString(),
        form.FirstName,
        form.LastName,
        form.Email,
        form.Phone,
        form.LegalFirstName,
        form.LegalLastName,
        form.Suffix,
        form.LinkedIn,
        form.Website,
        form.City,
        form.State,
        form.HowFound,
        form.ReferredBy,
        form.AuthorizedUSWork,
        form.NeedSponsorship,
        form.OfficeComply,
        form.RestrictiveCovenant,
        form.WorkedBefore,
        form.InterestReason,
        form.Gender,
        form.HispanicLatino,
        form.VeteranStatus,
        form.DisabilityStatus,
        resumeSaved = resumePath,
        coverSaved = coverPath,
        submittedAt = DateTimeOffset.UtcNow
    });
});

app.Run();

public class ApplicationForm
{
    // Basics
    public string? FirstName { get; set; }
    public string? LastName  { get; set; }
    public string? Email     { get; set; }
    public string? Phone     { get; set; }

    // Legal names / suffix
    public string? LegalFirstName { get; set; }
    public string? LegalLastName  { get; set; }
    public string? Suffix         { get; set; }

    // Links
    public string? LinkedIn { get; set; }
    public string? Website  { get; set; }

    // Location
    public string? City  { get; set; }
    public string? State { get; set; }

    // How discovered / referral
    public string? HowFound   { get; set; }
    public string? ReferredBy { get; set; }

    // Eligibility & policy
    public string? AuthorizedUSWork    { get; set; }
    public string? NeedSponsorship     { get; set; }
    public string? OfficeComply        { get; set; }
    public string? RestrictiveCovenant { get; set; }
    public string? WorkedBefore        { get; set; }

    // Motivation
    public string? InterestReason { get; set; }

    // Voluntary self-ID
    public string? Gender         { get; set; }
    public string? HispanicLatino { get; set; }
    public string? VeteranStatus  { get; set; }
    public string? DisabilityStatus { get; set; }

    // Files
    public IFormFile? Resume      { get; set; }
    public IFormFile? CoverLetter { get; set; }
}

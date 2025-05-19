using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var ci = System.Globalization.CultureInfo.InvariantCulture;
// Koneksi ke MySQL
var connectionString = "Server=localhost;Database=calendar_db;User=root;Password=";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySQL(connectionString));
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
// Tambahkan layanan CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() // Mengizinkan semua asal
              .AllowAnyHeader() // Mengizinkan semua header
              .AllowAnyMethod(); // Mengizinkan semua metode HTTP (GET, POST, PUT, DELETE, dll.)
    });
});
 
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

builder.Services.AddHttpClient();

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Request.EnableBuffering();
    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
    var body = await reader.ReadToEndAsync();
    context.Request.Body.Position = 0;
    Console.WriteLine($"Request Path: {context.Request.Path}, Body: {body}");
    await next();
});

app.UseCors("AllowAllOrigins");

// Endpoint untuk menambahkan event baru
app.MapGet("/events", async (
        ApplicationDbContext db,
        int page  = 1,
        int pageSize = 10) =>
{
    // --- ambil data dasar (tanpa join rumit) ---
    var slice = await db.Events
                        .OrderBy(e => e.EventId)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

    var list = new List<EventDto>();

    foreach (var ev in slice)
    {
        string label = ev.Frequency switch
        {
            "Weekly" => string.Join(", ",
                           await db.WeeklyEventEntries
                                   .Where(w => w.EventId == ev.EventId)
                                   .Select(w => w.DayOfWeek)
                                   .Distinct()
                                   .ToListAsync()),

            "Monthly" => ev.EventDate != null
                          ? ev.EventDate.Value.Day.ToString("00")
                          : (await db.MonthlyEventEntries
                                    .Where(m => m.EventId == ev.EventId)
                                    .Select(m => m.DayOfMonth)
                                    .FirstOrDefaultAsync())
                                    .ToString("00"),

            "Yearly"  => ev.EventDate != null
                          ? ev.EventDate.Value.ToString("dd MMM", ci)
                          : (await db.YearlyEventEntries
                                    .Where(y => y.EventId == ev.EventId)
                                    .Select(y => new { y.DayOfMonth, y.Month })
                                    .FirstOrDefaultAsync()) is { } yEntry
                                ? $"{yEntry.DayOfMonth:00} {yEntry.Month}"
                                : "",

            _ => ""
        };

        list.Add(new EventDto(ev.EventId, ev.EventName, ev.Frequency,
                              ev.EventDate, label));
    }

    var total = await db.Events.CountAsync();
    return Results.Ok(new { events = list, totalCount = total, page, pageSize });
});

app.MapPost("/events", async (Event req, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(req.EventName))
        return Results.BadRequest("EventName wajib diisi.");

    req.Frequency = req.Frequency?.Trim();

    var allowed = new[] { "Weekly", "Monthly", "Yearly" };
    if (!allowed.Contains(req.Frequency))
        return Results.BadRequest("Frequency harus Weekly, Monthly, atau Yearly.");

    if ((req.Frequency == "Monthly" || req.Frequency == "Yearly") && req.EventDate is null)
        return Results.BadRequest("EventDate wajib diisi untuk Monthly / Yearly.");

    if (req.Frequency == "Weekly")
        req.EventDate = null;
    req.CreatedAt = req.UpdatedAt = DateTime.UtcNow;

    db.Events.Add(req);
    await db.SaveChangesAsync(); 
    db.AuditLogs.Add(new AuditLog
    {
        Action    = "Create",
        Entity    = "Event",
        EntityId  = req.EventId,
        User      = "admin",
        Timestamp = DateTime.UtcNow,
        Changes   = $"Create '{req.EventName}' ({req.Frequency})"
    });
    await db.SaveChangesAsync();
    return Results.Created($"/events/{req.EventId}", new { req.EventId });
});


// Endpoint untuk mengupdate event berdasarkan ID (Menambahkan riwayat sebelum update)
app.MapPut("/events/{id}", async (int id, Event req, ApplicationDbContext db) =>
{
    var ev = await db.Events.FindAsync(id);
    if (ev is null) return Results.NotFound("Event tidak ditemukan.");

    db.EventHistories.Add(new EventHistory
    {
        EventId    = ev.EventId,
        EventName  = ev.EventName,
        EventDate  = ev.EventDate,
        Frequency  = ev.Frequency,
        Notes      = ev.Notes,
        ActionDate = DateTime.Now,
        ActionType = "Update",
        Changes    = "Update"
    });
    await db.SaveChangesAsync();

    ev.EventName = req.EventName;
    ev.EventDate = req.EventDate;     // nullable
    ev.Frequency = req.Frequency;
    ev.Notes     = req.Notes;
    ev.UpdatedAt = DateTime.Now;
    await db.SaveChangesAsync();

    return Results.Ok(ev);
});

// Endpoint untuk mendapatkan semua events
// app.MapGet("/events", async (ApplicationDbContext db, int page = 1, int pageSize = 10) =>
// {
//     var list = await db.Events.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
//     var total = await db.Events.CountAsync();
//     return Results.Ok(new { Events = list, TotalCount = total, Page = page, PageSize = pageSize });
// });

// app.MapGet("/events/{id}", async (int id, ApplicationDbContext db) =>
//     await db.Events.FindAsync(id) is Event ev ? Results.Ok(ev) : Results.NotFound());

// app.MapDelete("/events/{id}", async (int id, ApplicationDbContext db) =>
// {
//     if (await db.Events.FindAsync(id) is not Event ev) return Results.NotFound();
//     db.Events.Remove(ev);
//     await db.SaveChangesAsync();
//     return Results.NoContent();
// });

// Endpoint untuk menambahkan entri kalender (weekly, monthly, yearly)
app.MapPost("/calendar", async (CalendarEntry entry, ApplicationDbContext dbContext) =>
{
    dbContext.CalendarEntries.Add(entry);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/calendar/{entry.CalendarId}", entry);
});

// Endpoint untuk mendapatkan semua entri kalender
app.MapGet("/calendar", async (int page, int pageSize, ApplicationDbContext dbContext) =>
{
    var calendarEntries = await dbContext.CalendarEntries
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();
    
    var totalCount = await dbContext.CalendarEntries.CountAsync();

    return Results.Ok(new 
    {
        CalendarEntries = calendarEntries,
        TotalCount = totalCount,
        Page = page,
        PageSize = pageSize
    });
});

/* ========== WEEKLY ========== */
app.MapPost("/weekly-events", async (WeeklyEventEntry w, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(w.DayOfWeek))
        return Results.BadRequest("DayOfWeek wajib diisi.");

    if (await db.Events.FindAsync(w.EventId) is null)
        return Results.NotFound("Event tidak ditemukan.");

    foreach (var raw in w.DayOfWeek.Split(',', StringSplitOptions.RemoveEmptyEntries))
        db.WeeklyEventEntries.Add(new WeeklyEventEntry
        {
            EventId   = w.EventId,
            DayOfWeek = raw.Trim().Substring(0, 3),
            Notes     = w.Notes
        });

    await db.SaveChangesAsync();
    return Results.Created($"/weekly-events/event/{w.EventId}", new { w.EventId });
});

/* ========== MONTHLY ========== */
app.MapPost("/monthly-events", async (MonthlyEventEntry m, ApplicationDbContext db) =>
{
    if (await db.Events.FindAsync(m.EventId) is null)
        return Results.NotFound("Event tidak ditemukan.");

    if (!DateTime.TryParseExact(m.Month, "MMM", ci, DateTimeStyles.None, out _))
        return Results.BadRequest("Month harus format \"Jan\", \"Feb\", ...");

    db.MonthlyEventEntries.Add(new MonthlyEventEntry
    {
        EventId    = m.EventId,
        DayOfMonth = m.DayOfMonth,
        Month      = m.Month,
        Notes      = m.Notes
    });
    await db.SaveChangesAsync();
    return Results.Created($"/monthly-events/event/{m.EventId}", new { m.EventId });
});

/* ========== YEARLY ========== */
app.MapPost("/yearly-events", async (YearlyEventEntry y, ApplicationDbContext db) =>
{
    if (await db.Events.FindAsync(y.EventId) is null)
        return Results.NotFound("Event tidak ditemukan.");

    if (!DateTime.TryParseExact(y.Month, "MMM", ci, DateTimeStyles.None, out _))
        return Results.BadRequest("Month harus format \"Jan\", \"Feb\", ...");

    db.YearlyEventEntries.Add(new YearlyEventEntry
    {
        EventId    = y.EventId,
        Month      = y.Month,
        DayOfMonth = y.DayOfMonth,
        Notes      = y.Notes
    });
    await db.SaveChangesAsync();
    return Results.Created($"/yearly-events/event/{y.EventId}", new { y.EventId });
});

/* ========== WEEKLY – GET SEMUA ========== */
app.MapGet("/weekly-events", async (ApplicationDbContext db) =>
    await db.WeeklyEventEntries
            .Select(w => new { w.WeeklyEventId, w.EventId, w.DayOfWeek, w.Notes })
            .ToListAsync());

/* ========== MONTHLY – GET SEMUA ========== */
app.MapGet("/monthly-events", async (ApplicationDbContext db) =>
    await db.MonthlyEventEntries
            .Select(m => new { m.MonthlyEventId, m.EventId, m.DayOfMonth, m.Month, m.Notes })
            .ToListAsync());

/* ========== YEARLY – GET SEMUA ========== */
app.MapGet("/yearly-events", async (ApplicationDbContext db) =>
    await db.YearlyEventEntries
            .Select(y => new { y.YearlyEventId, y.EventId, y.DayOfMonth, y.Month, y.Notes })
            .ToListAsync());

/* ---------- DELETE schedule by EventId, gunakan EF Core 7+ ExecuteDeleteAsync() ---------- */
// WEEKLY DELETE
app.MapDelete("/weekly-events/{eventId:int}", async (int eventId, ApplicationDbContext db) =>
{
    var rows = await db.WeeklyEventEntries.Where(w => w.EventId == eventId).ToListAsync();
    if (!rows.Any()) return Results.NoContent(); // return 204 jika tidak ada data (bukan 404)
    db.WeeklyEventEntries.RemoveRange(rows);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// MONTHLY DELETE
app.MapDelete("/monthly-events/{eventId:int}", async (int eventId, ApplicationDbContext db) =>
{
    var rows = await db.MonthlyEventEntries.Where(m => m.EventId == eventId).ToListAsync();
    if (!rows.Any()) return Results.NoContent();
    db.MonthlyEventEntries.RemoveRange(rows);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// YEARLY DELETE
app.MapDelete("/yearly-events/{eventId:int}", async (int eventId, ApplicationDbContext db) =>
{
    var rows = await db.YearlyEventEntries.Where(y => y.EventId == eventId).ToListAsync();
    if (!rows.Any()) return Results.NoContent();
    db.YearlyEventEntries.RemoveRange(rows);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


// Endpoint untuk melakukan Undo
app.MapPost("/undo/{eventId}", async (int eventId, ApplicationDbContext dbContext) =>
{
    // Ambil riwayat terakhir untuk event tersebut
    var lastHistory = await dbContext.EventHistories
        .Where(eh => eh.EventId == eventId)
        .OrderByDescending(eh => eh.ActionDate)
        .FirstOrDefaultAsync();

    if (lastHistory == null)
    {
        return Results.NotFound("No history found for Undo.");
    }

    // Mengembalikan event ke status sebelumnya
    var eventItem = await dbContext.Events.FindAsync(eventId);
    if (eventItem != null)
    {
        eventItem.EventName = lastHistory.EventName;
        eventItem.Frequency = lastHistory.Frequency;
        eventItem.Notes = lastHistory.Notes;
        eventItem.UpdatedAt = DateTime.Now;

        dbContext.EventHistories.Remove(lastHistory); // Hapus riwayat yang sudah dipulihkan
        await dbContext.SaveChangesAsync();

        return Results.Ok(eventItem);
    }

    return Results.NotFound("Event not found.");
});

// Endpoint untuk melakukan Redo
app.MapPost("/redo/{eventId}", async (int eventId, ApplicationDbContext dbContext) =>
{
    // Ambil riwayat redo terakhir untuk event tersebut
    var redoHistory = await dbContext.EventHistories
        .Where(eh => eh.EventId == eventId && eh.ActionType == "Undo")
        .OrderByDescending(eh => eh.ActionDate)
        .FirstOrDefaultAsync();

    if (redoHistory == null)
    {
        return Results.NotFound("No history found for Redo.");
    }

    // Mengembalikan event ke status redo
    var eventItem = await dbContext.Events.FindAsync(eventId);
    if (eventItem != null)
    {
        eventItem.EventName = redoHistory.EventName;
        eventItem.Frequency = redoHistory.Frequency;
        eventItem.Notes = redoHistory.Notes;
        eventItem.UpdatedAt = DateTime.Now;

        dbContext.EventHistories.Remove(redoHistory); // Hapus riwayat redo yang sudah dipulihkan
        await dbContext.SaveChangesAsync();

        return Results.Ok(eventItem);
    }

    return Results.NotFound("Event not found.");
});

// for wa
// Tambah nomor WA baru
string NormalizePhoneNumber(string phone)
{
    if (string.IsNullOrEmpty(phone))
        return phone;

    var digits = new string(phone.Where(char.IsDigit).ToArray());
    if (digits.StartsWith("0"))
        digits = "62" + digits.Substring(1);

    return digits;
}

app.MapPost("/wa-users", async (WhatsAppUser user, ApplicationDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(user.PhoneNumber))
        return Results.BadRequest("Nomor WA wajib diisi.");

    user.PhoneNumber = NormalizePhoneNumber(user.PhoneNumber);

    db.WhatsAppUsers.Add(user);
    await db.SaveChangesAsync();
    return Results.Created($"/wa-users/{user.Id}", user);
});


// Ambil daftar nomor WA
app.MapGet("/wa-users", async (ApplicationDbContext db) =>
{
    var list = await db.WhatsAppUsers.OrderBy(u => u.Id).ToListAsync();
    return Results.Ok(list);
});

app.MapPost("/send-wa/{userId}", async (int userId, SendWaRequest req, ApplicationDbContext db, IHttpClientFactory httpClientFactory) =>
{
    var user = await db.WhatsAppUsers.FindAsync(userId);
    if (user == null) return Results.NotFound("Nomor WA tidak ditemukan.");

    if (string.IsNullOrWhiteSpace(req.Message))
        return Results.BadRequest("Pesan wajib diisi.");

    // Qontak API configuration
    const string qontakApiUrl = "https://service-chat.qontak.com/api/open/v1/broadcasts/whatsapp/direct";
    const string bearerToken = "1Bs4cNxWFLUWUEd-3WSUKJOOmfeis8z4VrHU73v6_1Q"; // Use your actual token here
    const string messageTemplateId = "300d84f2-d962-4451-bc27-870fb99d18e7"; // Your template id
    const string channelIntegrationId = "662f9fcb-7e2b-4c1a-8eda-9aeb4a388004"; // Your channel integration id

    // Prepare request payload according to your curl example
    var payload = new
    {
        to_number = user.PhoneNumber.Replace("+", ""), // remove + if any, Qontak expects without +
        to_name = string.IsNullOrWhiteSpace(user.Name) ? "User" : user.Name,
        message_template_id = messageTemplateId,
        channel_integration_id = channelIntegrationId,
        language = new { code = "id" },
        parameters = new
        {
            body = new[]
            {
                new { key = "1", value = "full_name", value_text = user.Name ?? "User" },
                new { key = "2", value = "messagetext", value_text = req.Message }
            }
        }
    };

    var httpClient = httpClientFactory.CreateClient();
    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

    var jsonPayload = JsonSerializer.Serialize(payload);
    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

    try
    {
        var response = await httpClient.PostAsync(qontakApiUrl, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            // Return Qontak API response for tracking
            return Results.Ok(new { Message = "Pesan terkirim via Qontak WhatsApp API", Response = responseContent });
        }
        else
        {
            return Results.Json(new { Error = "Gagal mengirim pesan", Details = responseContent }, statusCode: (int)response.StatusCode);
        }
    }
    catch (Exception ex)
    {
        return Results.Json(new { Error = "Terjadi kesalahan saat mengirim pesan", Details = ex.Message }, statusCode: 500);
    }
});
// Edit nomor WA by id
app.MapPut("/wa-users/{id}", async (int id, WhatsAppUser updatedUser, ApplicationDbContext db) =>
{
    var user = await db.WhatsAppUsers.FindAsync(id);
    if (user == null) return Results.NotFound("Nomor WA tidak ditemukan.");

    // Normalisasi nomor juga saat update
    string NormalizePhoneNumber(string phone)
    {
        if (string.IsNullOrEmpty(phone))
            return phone;

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        if (digits.StartsWith("0"))
            digits = "62" + digits.Substring(1);
        return digits;
    }

    user.Name = updatedUser.Name;
    user.PhoneNumber = NormalizePhoneNumber(updatedUser.PhoneNumber);

    await db.SaveChangesAsync();
    return Results.Ok(user);
});

// Delete nomor WA by id
app.MapDelete("/wa-users/{id}", async (int id, ApplicationDbContext db) =>
{
    var user = await db.WhatsAppUsers.FindAsync(id);
    if (user == null) return Results.NotFound("Nomor WA tidak ditemukan.");

    db.WhatsAppUsers.Remove(user);
    await db.SaveChangesAsync();
    return Results.NoContent();
});



app.Run();


// DbContext class untuk entitas Event dan CalendarEntry
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Event> Events { get; set; }
    public DbSet<CalendarEntry> CalendarEntries { get; set; }
    public DbSet<WeeklyEventEntry> WeeklyEventEntries { get; set; }
    public DbSet<MonthlyEventEntry> MonthlyEventEntries { get; set; }
    public DbSet<YearlyEventEntry> YearlyEventEntries { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<EventHistory> EventHistories { get; set; }
    public DbSet<WhatsAppUser> WhatsAppUsers { get; set; }




    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarEntry>().HasKey(c => c.CalendarId);
        modelBuilder.Entity<WeeklyEventEntry>().HasKey(w => w.WeeklyEventId);
        modelBuilder.Entity<MonthlyEventEntry>().HasKey(m => m.MonthlyEventId);
        modelBuilder.Entity<YearlyEventEntry>().HasKey(y => y.YearlyEventId);

        // Set default value for CreatedAt and UpdatedAt
        modelBuilder.Entity<CalendarEntry>()
            .Property(c => c.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<CalendarEntry>()
            .Property(c => c.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<WeeklyEventEntry>()
            .Property(w => w.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<WeeklyEventEntry>()
            .Property(w => w.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MonthlyEventEntry>()
            .Property(m => m.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<MonthlyEventEntry>()
            .Property(m => m.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<YearlyEventEntry>()
            .Property(y => y.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<YearlyEventEntry>()
            .Property(y => y.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        base.OnModelCreating(modelBuilder);
    }
}


// Model untuk Event
public class Event
{
    public int       EventId    { get; set; }
    public string    EventName  { get; set; } = string.Empty;
    public DateTime? EventDate  { get; set; }          // nullable
    public string    Frequency  { get; set; } = string.Empty;
    public string?   Notes      { get; set; }
    public DateTime? CreatedAt  { get; set; }
    public DateTime? UpdatedAt  { get; set; }
}


// Model untuk Calendar Entry
public class CalendarEntry
{
    public int CalendarId { get; set; }  // Primary Key
    public int EventId { get; set; }
    public DateTime EventDate { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public string Location { get; set; }
    public string Notes { get; set; }

    // Menambahkan properti CreatedAt dan UpdatedAt
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Event Event { get; set; }
}

// Model untuk Weekly Event Entries
// Weekly
public class WeeklyEventEntry
{
    public int      WeeklyEventId { get; set; }
    public int      EventId       { get; set; }
    public string   DayOfWeek     { get; set; }  // WAJIB
    public TimeSpan? StartTime    { get; set; }
    public TimeSpan? EndTime      { get; set; }
    public string?  Location      { get; set; }  // ⬅️ nullable
    public string?  Notes         { get; set; }  // ⬅️ nullable
    public DateTime? CreatedAt    { get; set; }
    public DateTime? UpdatedAt    { get; set; }
    public Event    Event         { get; set; }
}

// Monthly
public class MonthlyEventEntry
{
    public int      MonthlyEventId { get; set; }
    public int      EventId        { get; set; }
    public int      DayOfMonth     { get; set; }
    public string   Month          { get; set; }
    public TimeSpan? StartTime     { get; set; }
    public TimeSpan? EndTime       { get; set; }
    public string?  Location       { get; set; } // ⬅️
    public string?  Notes          { get; set; } // ⬅️
    public DateTime? CreatedAt     { get; set; }
    public DateTime? UpdatedAt     { get; set; }
    public Event    Event          { get; set; }
}

// Yearly
public class YearlyEventEntry
{
    public int      YearlyEventId { get; set; }
    public int      EventId       { get; set; }
    public string   Month         { get; set; }
    public int      DayOfMonth    { get; set; }
    public TimeSpan? StartTime    { get; set; }
    public TimeSpan? EndTime      { get; set; }
    public string?  Location      { get; set; } // ⬅️
    public string?  Notes         { get; set; } // ⬅️
    public DateTime? CreatedAt    { get; set; }
    public DateTime? UpdatedAt    { get; set; }
    public Event    Event         { get; set; }
}


public class AuditLog
{
    public int AuditLogId { get; set; }
    public string Action { get; set; } // (Create, Update, Delete)
    public string Entity { get; set; } // Nama entitas, misalnya "Event"
    public int EntityId { get; set; } // ID entitas yang diubah
    public string User { get; set; } // Nama pengguna atau ID yang melakukan perubahan
    public DateTime Timestamp { get; set; } // Waktu perubahan
    public string Changes { get; set; } // Deskripsi perubahan yang terjadi
}

public class EventHistory
{
    public int       EventHistoryId { get; set; }
    public int       EventId        { get; set; }
    public string    EventName      { get; set; } = string.Empty;
    public DateTime? EventDate      { get; set; }      // nullable
    public string    Frequency      { get; set; } = string.Empty;
    public string?   Notes          { get; set; }
    public DateTime  ActionDate     { get; set; }
    public string    ActionType     { get; set; } = string.Empty;
    public string    Changes        { get; set; } = string.Empty;
}

public class WhatsAppUser
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;     // Optional nama user
    public string PhoneNumber { get; set; } = string.Empty; // Nomor WA, wajib +628xxx dst.
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class SendWaRequest
{
    public string Message { get; set; }
}


// DTO utk response
public record EventDto(
    int       EventId,
    string    EventName,
    string    Frequency,
    DateTime? EventDate,
    string    TimeLabel
);



using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

var sharedSecret = "mysupersecret";
var lokiEndpoint = "http://localhost:3100/loki/api/v1/push";
var httpClient = new HttpClient();

var knownProcessHashes = new HashSet<string>
{
    // Дополнить оригинальными SHA256 хешами стандартных приложений
    "E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855" // placeholder
};

var students = new Dictionary<string, StudentStatus>();

app.MapPost("/register", async ([FromBody] RegisterPayload payload, HttpRequest request) =>
{
    if (!ValidateSignature(request, sharedSecret, payload)) return Results.Unauthorized();
    students[payload.Name] = new StudentStatus { Name = payload.Name, LastSeen = DateTime.UtcNow };
    await LogToLoki(payload.Name, "register", "OK");
    return Results.Ok();
});

app.MapPost("/heartbeat", async ([FromBody] HeartbeatPayload payload, HttpRequest request) =>
{
    if (!ValidateSignature(request, sharedSecret, payload)) return Results.Unauthorized();
    if (students.TryGetValue(payload.Name, out var status))
    {
        status.LastSeen = DateTime.UtcNow;
    }
    await LogToLoki(payload.Name, "heartbeat", payload.Time.ToString("O"));
    return Results.Ok();
});

app.MapPost("/active-window", async ([FromBody] ActiveWindowPayload payload, HttpRequest request) =>
{
    if (!ValidateSignature(request, sharedSecret, payload)) return Results.Unauthorized();
    await LogToLoki(payload.Name, "active-window", payload.Title);
    return Results.Ok();
});

app.MapPost("/process-list", async ([FromBody] ProcessListPayload payload, HttpRequest request) =>
{
    if (!ValidateSignature(request, sharedSecret, payload)) return Results.Unauthorized();
    var suspicious = payload.Processes.Where(p =>
        p.Hash == "" ||
        !knownProcessHashes.Contains(p.Hash) ||
        (p.Path != null && !p.Path.StartsWith("/usr/bin") && !p.Path.StartsWith("C:\\Windows"))
    ).ToList();

    if (suspicious.Any())
    {
        foreach (var proc in suspicious)
        {
            await LogToLoki(payload.Name, "suspicious-process", $"{proc.ProcessName} {proc.Path} {proc.Hash}");
        }
    }
    else
    {
        await LogToLoki(payload.Name, "process-list", "OK");
    }
    return Results.Ok();
});

app.MapPost("/network", async ([FromBody] NetworkPayload payload, HttpRequest request) =>
{
    if (!ValidateSignature(request, sharedSecret, payload)) return Results.Unauthorized();
    await LogToLoki(payload.Name, "external-ip", payload.ExternalIp);

    foreach (var iface in payload.Interfaces)
    {
        if (iface.IsSuspicious || iface.DnsSuspicious)
        {
            await LogToLoki(payload.Name, "suspicious-network", $"{iface.Name} {iface.Description}");
        }
    }
    return Results.Ok();
});

app.Run("http://0.0.0.0:5000");

// ==== Логирование в Grafana Loki ====
async Task LogToLoki(string student, string label, string message)
{
    var payload = new
    {
        streams = new[]
        {
            new
            {
                stream = new { student, label },
                values = new[] { new[] { (((DateTimeOffset)DateTime.UtcNow).ToUnixTimeMilliseconds() * 1_000_000).ToString(), message } }
            }
        }
    };

    try
    {
        var response = await httpClient.PostAsJsonAsync(lokiEndpoint, payload);
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[LOKI] Ошибка: {response.StatusCode} {await response.Content.ReadAsStringAsync()}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("[LOKI] Исключение при отправке: " + ex.Message);
    }
}

bool ValidateSignature(HttpRequest req, string secret, object payload)
{
    if (!req.Headers.TryGetValue("X-Agent-Timestamp", out var ts) ||
        !req.Headers.TryGetValue("X-Agent-Signature", out var sig)) return false;

    var name = req.Query["name"].ToString();
    var json = JsonSerializer.Serialize(payload);
    var toSign = $"{name}:{ts}:{json}";

    using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(toSign));
    var expectedSig = Convert.ToHexString(hash);

    return expectedSig.Equals(sig.ToString(), StringComparison.OrdinalIgnoreCase);
}


record RegisterPayload(string Name);
record HeartbeatPayload(string Name, DateTime Time);
record ActiveWindowPayload(string Name, string Title, DateTime Time);
record NetworkPayload(string Name, List<NetworkInterfaceInfo> Interfaces, string ExternalIp, List<string> ActiveConnections, DateTime Time);
record NetworkInterfaceInfo(string Name, string Description, string Type, string MacAddress, List<string> DnsAddresses, List<string> GatewayAddresses, bool IsSuspicious, bool DnsSuspicious);
record ProcessListPayload(string Name, List<ProcessInfo> Processes, DateTime Time);
record ProcessInfo(string ProcessName, string Path, string Hash);

class StudentStatus
{
    public string Name { get; set; } = "";
    public DateTime LastSeen { get; set; }
}

# 🌍 Blocked Countries API

A .NET Core Web API to manage blocked countries (permanent & temporary), validate IP addresses using external geolocation APIs, and log access attempts — all using in-memory storage (no database).

---

## 📦 Features

- ✅ Block/Unblock countries by country code  
- ✅ Temporarily block a country for a specific duration  
- ✅ Lookup IP location details  
- ✅ Check if IP is from a blocked country  
- ✅ Log all access attempts with full info  
- ✅ Paginated endpoints for logs and countries  
- ✅ Background service to clean up expired temporal blocks  

---

## 🛠 Tech Stack

- ASP.NET Core 7/8/9 Web API  
- `HttpClient` + `Newtonsoft.Json`  
- In-Memory storage with `ConcurrentDictionary` and `ConcurrentBag`  
- Swagger for API Documentation  
- xUnit + Moq for unit testing  

---

## ⚙️ Configuration

### Add your API Key to `appsettings.json`:

```json
"IpApiSettings": {
  "BaseUrl": "https://api.ipgeolocation.io/ipgeo",
  "ApiKey": "YOUR_API_KEY"
}
```

---

## 🚀 Running the Project

```bash
dotnet restore
dotnet run
```

Navigate to:  
`https://localhost:{port}/swagger`

---

## 🔐 API Endpoints

| Method | Endpoint                                   | Description                      |
|--------|--------------------------------------------|----------------------------------|
| GET    | `/api/ip/lookup?ipAddress={ip}`            | Get geolocation info for an IP   |
| GET    | `/api/ip/check-block`                       | Check if the caller's IP is blocked |
| GET    | `/api/ip/blocked-attempts?page=1&pageSize=10` | Get paginated access logs      |
| POST   | `/api/countries/block?countryCode=US`      | Block a country permanently      |
| POST   | `/api/countries/temporal-block`             | Temporarily block a country      |
| GET    | `/api/countries/blocked?page=1&pageSize=10&search=us` | Get blocked countries list |
| DELETE | `/api/countries/block/{countryCode}`       | Unblock a country                |

---

## 🧪 Unit Testing

Tested with xUnit and Moq.

### Covered Cases:
- ✅ Country is blocked → IP is blocked  
- ✅ Country is not blocked → IP allowed  
- ✅ IP lookup fails → returns 500  
- ✅ No IP → returns 400  

Run tests:  
```bash
dotnet test
```

---

## 🔄 Background Service

A background task runs every 5 minutes to:  
- Remove expired temporary blocks  

Registered as a HostedService:  
```csharp
builder.Services.AddHostedService<BlockCleanupService>();
```

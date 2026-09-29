# TaskMngBack

Görev/proje yönetimi için .NET 8 Web API (controller tabanlı). Veritabanı PostgreSQL (EF Core + Npgsql), dosyalar Cloudflare R2'de, kimlik doğrulama JWT. Railway'de Docker ile yayınlanıyor.

## Mimari

- `Controllers/` → `Services/` (+ `Services/Interfaces/`) → `Repositories/` (+ `Repositories/Interfaces/`) → `Data/AppDbContext.cs`
- `DTOs/<Alan>/` istek/yanıt modelleri, `Models/` entity'ler, `Models/Enums/` enum'lar
- `Configuration/` strongly-typed ayarlar (`JwtSettings`, `R2Settings`, `NotificationSettings`)
- `Authorization/` izin tabanlı yetkilendirme (`Permission:<anahtar>` policy'leri, ör. `Permission:projects.manage`)
- `BackgroundServices/DeadlineNotificationService` süreli görev kontrolü ve bildirim üretir
- `Middleware/ExceptionMiddleware` hataları JSON'a çevirir
- Yeni servis/repository `Program.cs` içinde DI'a `AddScoped` ile kaydedilir.

## Ortamlar ve yapılandırma

| Ortam | Veritabanı | Yapılandırma kaynağı |
|---|---|---|
| Local (Development) | Postgres server'daki **TEST** veritabanı | User Secrets (`secrets.json`, repo dışında) |
| Canlı (Railway, Production) | Aynı server'daki **PROD** veritabanı | Railway Variables |

- Postgres server'ında üç veritabanı var: varsayılan `railway` (dokunulmaz), `TEST`, `PROD`. İsimler büyük/küçük harfe duyarlı.
- Secret'lar (bağlantı dizesi, JWT anahtarı, R2 anahtarları) **asla** repoya, `appsettings*.json`'a ya da commit'e girmez.
- Ortam değişkeninde iç içe anahtarlar `__` ile yazılır: `JwtSettings:SecretKey` → `JwtSettings__SecretKey`.
- CORS origin'leri dizi olduğu için indeksli verilir: `Cors__AllowedOrigins__0`, `__1`, `__2` (virgülle ayırma çalışmaz, sonda `/` olmaz). Railway'de tanımlanınca `appsettings.json`'daki localhost değerini ezer.
- Production'da Swagger kapalıdır (yalnızca Development'ta açılır).

## Railway dağıtımı

- Kökteki `Dockerfile` çok aşamalı build yapar. Uygulama Railway'in verdiği `PORT`'ta dinler (yoksa 8080). TLS'i Railway sağlar, container yalnızca HTTP dinler; "HTTPS port bulunamadı" uyarısı zararsızdır.
- Postgres aynı Railway projesinde olduğu için **internal** adres kullanılır:
  `Host=${{Postgres.PGHOST}};Port=${{Postgres.PGPORT}};Database=PROD;Username=${{Postgres.PGUSER}};Password=${{Postgres.PGPASSWORD}}`
  (`Postgres` yerine servisin gerçek adı). Internal adres (`*.railway.internal`) yerelden çözülmez.
- Migration'lar uygulama açılışında otomatik uygulanır (`Program.cs` içinde `Database.Migrate()`). Yeni migration eklemek yeterli, PROD'a elle uygulamak gerekmez.
- Admin kullanıcısı (`admin@taskmanager.local`) ve rol/izin verileri migration'larla gelir. Canlıya ilk çıkışta admin şifresi değiştirilmelidir.

## Komutlar

```powershell
dotnet run                                   # local (Development, TEST veritabanı)
dotnet ef migrations add <Ad>                # yeni migration
dotnet ef database update                    # migration'ı local (TEST) veritabanına uygula
docker build -t taskmngback .                # imajı yerelde dene
```

## Postman collection (zorunlu kural)

Collection: `postman/TaskMngBack.postman_collection.json`. Tüm endpoint'ler klasörlere ayrılmış ve POST/PUT/PATCH istekleri örnek body içerir.

**Yeni bir endpoint eklendiğinde (ya da mevcut bir endpoint'in route'u, DTO'su, enum/doğrulama kuralı değiştiğinde) Postman collection buna göre güncellenecek.**

- İlgili klasöre isteği ekle; body'yi DTO'dan üret (enum değerleri ve doğrulama kuralları ortada olsun).
- Create isteklerinde yanıttaki id'yi collection değişkenine yazan test scriptini koy (mevcut isteklerdeki `save(...)` kalıbı gibi); `int` alanlar body'de tırnaksız `{{degisken}}` olarak yazılır, `Guid`/`string` alanlar tırnaklı.
- Yeni değişken gerekirse collection'ın `variable` listesine ekle.
- Kaldırılan endpoint'in isteğini de collection'dan sil.
- Collection'a gizli bilgi (şifre, token) yazma; `adminPassword` boş kalır.

## Bilinen noktalar

- `POST /api/auth/register` herkese açıktır (`AllowAnonymous`). Kullanıcıları yalnızca admin oluşturacaksa kapatılmalı.
- `Authorization` policy'leri `Permission:<anahtar>` biçimindedir; mevcut izinler: `departments.manage`, `projects.manage`, `roles.manage`, `statuses.manage`, `tasks.assign`, `users.manage`.
- Dosya yükleme akışı: `attachments/presign` → istemci dosyayı dönen `uploadUrl`'e PUT eder → `attachments/confirm`.

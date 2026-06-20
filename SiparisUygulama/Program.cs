using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using SiparisUygulama.Data;
using SiparisUygulama.Hubs;
using SiparisUygulama.Models;
using SiparisUygulama.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. MySQL bağlantısı
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SiparisDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// 2. Data Protection — container restart'ta session kaybolmasın
var keysPath = new DirectoryInfo("/home/dataprotection-keys");
try { keysPath.Create(); } catch { }
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(keysPath)
    .SetApplicationName("SiparisUygulama");

// 3. Session desteği
builder.Services.AddSession(opts => {
    opts.Cookie.IsEssential = true;
    opts.IdleTimeout = TimeSpan.FromHours(8);
});

// 3. AI Servisi HTTP Client
var aiServiceUrl = builder.Configuration["AiService:BaseUrl"] ?? "http://localhost:8000";
builder.Services.AddHttpClient<AiServiceClient>(c =>
{
    c.BaseAddress = new Uri(aiServiceUrl);
    c.Timeout = TimeSpan.FromSeconds(10);
});

// 4. MVC + SignalR servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

// 4. Ortam kontrolü
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Home/HttpError/{0}");

// 5. Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Session burada aktif edilmeli
app.UseAuthorization();

// 6. Varsayılan yönlendirme: Giriş ekranı
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Giris}/{action=Index}/{id?}");

app.MapHub<SiparisHub>("/siparisHub");

// DB migrations & seed
try
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SiparisDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    // Retry logic: Azure SQL soğuk başlangıçta yavaş olabilir
    for (int attempt = 1; attempt <= 5; attempt++)
    {
        try
        {
            db.Database.EnsureCreated();
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Kullanici') AND name = 'Avatar')
                ALTER TABLE Kullanici ADD Avatar NVARCHAR(50) NULL");
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Restoran') AND name = 'MinSiparisAmount')
                ALTER TABLE Restoran ADD MinSiparisAmount DECIMAL(10,2) NULL");
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'Restoran') AND name = 'TeslimatUcreti')
                ALTER TABLE Restoran ADD TeslimatUcreti DECIMAL(10,2) NULL");
            db.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Favori') AND type = 'U')
                BEGIN
                    CREATE TABLE Favori (
                        FavoriId INT IDENTITY(1,1) PRIMARY KEY,
                        KullaniciId INT NOT NULL,
                        RestaurantId INT NOT NULL,
                        EklenmeTarihi DATETIME2 NOT NULL DEFAULT GETDATE(),
                        CONSTRAINT FK_Favori_Kullanici FOREIGN KEY (KullaniciId) REFERENCES Kullanici(KullaniciId) ON DELETE CASCADE,
                        CONSTRAINT FK_Favori_Restoran FOREIGN KEY (RestaurantId) REFERENCES Restoran(RestaurantId) ON DELETE CASCADE,
                        CONSTRAINT UQ_Favori UNIQUE (KullaniciId, RestaurantId)
                    )
                END");
            DbSeeder.Seed(db);
            logger.LogInformation("DB başlatıldı.");
            break;
        }
        catch (Exception ex) when (attempt < 5)
        {
            logger.LogWarning("DB bağlantı denemesi {Attempt}/5 başarısız: {Error}", attempt, ex.Message);
            Thread.Sleep(5000 * attempt);
        }
    }
}
catch (Exception ex)
{
    var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
    loggerFactory.CreateLogger<Program>().LogError(ex, "DB başlatılamadı, uygulama devam ediyor.");
}

app.Run();
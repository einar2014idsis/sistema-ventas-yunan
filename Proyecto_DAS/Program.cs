using Microsoft.AspNetCore.Server.IIS;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Proyecto_DAS.Data;
using Proyecto_DAS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

// Registrar servicios
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BoletaService>();
builder.Services.AddScoped<ReporteVentasService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<CarritoService>();
builder.Services.AddScoped<DashboardVentasService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("StringSQLServer"));
});

// Configurar timeouts para subida de archivos
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800; // 50 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 52428800; // 50 MB
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
else
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ClientePanel}/{action=Index}/{id?}");

app.Run();

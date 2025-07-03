
using ECommerce.API.Middleware;
using HackerPlatform.Infrastructure.Data;
using HackerPlatform.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// ✅ Add services to the container.
builder.Services.AddControllers();




var app = builder.Build();


#region UpdateDataBase

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var appDbContext = services.GetRequiredService<AppDbContext>();
        appDbContext.Database.Migrate(); // Apply migrations for ApplicationDbContext

        //var identityDbContext = services.GetRequiredService<ApplicationIdentityDbContext>();
        //identityDbContext.Database.Migrate(); // Apply migrations for ApplicationIdentityDbContext
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations.");
    }
}
#endregion
// ✅ قبل ما يبدأ السيرفر، ننفذ الـ Seed
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<AppDbContext>();
    await DbInitializer.SeedAsync(db);
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();


app.Run();

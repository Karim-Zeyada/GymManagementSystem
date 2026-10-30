using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Data.Seeder;
using GymManagementSystem.DAL.Repositories.Plans;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IPlanRepository, PlanRepository>();

builder.Services.AddDbContext<GymDbContext>(options =>
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await using var scope = app.Services.CreateAsyncScope();
var dbContext = scope.ServiceProvider
        .GetRequiredService<GymDbContext>();
await DatabaseSeeder.SeedAllAsync(dbContext);

app.Run();

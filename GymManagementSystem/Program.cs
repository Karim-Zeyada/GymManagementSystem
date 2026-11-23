using GymManagementSystem.BLL.Services.Members;
using GymManagementSystem.BLL.Services.Plans;
using GymManagementSystem.BLL.Services.Trainers;
using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Data.Seeder;
using GymManagementSystem.DAL.Repositories.Generic;
using GymManagementSystem.DAL.Repositories.Members;
using GymManagementSystem.DAL.Repositories.Plans;
using GymManagementSystem.DAL.Repositories.Trainers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Repositories Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();

// Services Registration
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();

builder.Services.AddDbContext<GymDbContext>(options =>
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));  
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider
            .GetRequiredService<GymDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAllAsync(dbContext);
}

app.Run();

using GymManagementSystem.BLL.Mapping;
using GymManagementSystem.BLL.Services.Analytics;
using GymManagementSystem.BLL.Services.Attachments;
using GymManagementSystem.BLL.Services.Bookings;
using GymManagementSystem.BLL.Services.Members;
using GymManagementSystem.BLL.Services.Memberships;
using GymManagementSystem.BLL.Services.Plans;
using GymManagementSystem.BLL.Services.Sessions;
using GymManagementSystem.BLL.Services.Trainers;
using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Data.Seeder;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Bookings;
using GymManagementSystem.DAL.Repositories.Generic;
using GymManagementSystem.DAL.Repositories.Members;
using GymManagementSystem.DAL.Repositories.Memberships;
using GymManagementSystem.DAL.Repositories.Plans;
using GymManagementSystem.DAL.Repositories.Sessions;
using GymManagementSystem.DAL.Repositories.Trainers;
using GymManagementSystem.DAL.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// AutoMapper Registration
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

// Repositories Registration
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPlanRepository, PlanRepository>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IMembershipRepository, MembershipRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Unit of Work Registration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services Registration
builder.Services.AddScoped<IPlanService, PlanService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();

// Database Context Registration
builder.Services.AddDbContext<GymDbContext>(options =>
{ 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));  
    options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
});

// Identity Registration
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<GymDbContext>()
.AddDefaultTokenProviders();

// Cookie Authentication Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
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

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAllAsync(dbContext, roleManager, userManager);
}

app.Run();

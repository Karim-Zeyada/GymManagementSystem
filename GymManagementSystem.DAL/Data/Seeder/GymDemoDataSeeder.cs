using GymManagementSystem.DAL.Data.Contexts;
using GymManagementSystem.DAL.Enums;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace GymManagementSystem.DAL.Data.Seeder;

public static class GymDemoDataSeeder
{
    public static async Task SeedDemoDataAsync(GymDbContext context)
    {
        // 1. Seed Trainers if none exist
        if (!await context.Trainers.AnyAsync())
        {
            var trainers = new List<Trainer>
            {
                new()
                {
                    Name = "Ahmed Shaker",
                    Email = "ahmed.shaker@gym.com",
                    Phone = "01011112222",
                    DateOfBirth = new DateOnly(1988, 5, 12),
                    Gender = Gender.Male,
                    Speciality = Speciality.GeneralFitness,
                    Address = new Address { BuildingNumber = 12, Street = "El-Tahrir St", City = "Cairo" }
                },
                new()
                {
                    Name = "Sarah Mansour",
                    Email = "sarah.mansour@gym.com",
                    Phone = "01122223333",
                    DateOfBirth = new DateOnly(1992, 9, 20),
                    Gender = Gender.Female,
                    Speciality = Speciality.Cardio,
                    Address = new Address { BuildingNumber = 45, Street = "Makram Ebeid", City = "Nasr City" }
                },
                new()
                {
                    Name = "Tarek Nour",
                    Email = "tarek.nour@gym.com",
                    Phone = "01233334444",
                    DateOfBirth = new DateOnly(1985, 3, 15),
                    Gender = Gender.Male,
                    Speciality = Speciality.Crossfit,
                    Address = new Address { BuildingNumber = 8, Street = "El-Gezira", City = "Zamalek" }
                }
            };

            await context.Trainers.AddRangeAsync(trainers);
            await context.SaveChangesAsync();
        }

        // 2. Seed Members if none exist
        if (!await context.Members.AnyAsync())
        {
            var members = new List<Member>
            {
                new()
                {
                    Name = "Omar Hassan",
                    Email = "omar.hassan@example.com",
                    Phone = "01099998888",
                    DateOfBirth = new DateOnly(1996, 7, 10),
                    Gender = Gender.Male,
                    JoinDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3)),
                    Address = new Address { BuildingNumber = 22, Street = "Abbas El-Akkad", City = "Cairo" },
                    HealthRecord = new HealthRecord
                    {
                        Height = 180,
                        Weight = 82,
                        BloodType = BloodType.OPositive,
                        MedicalConditions = "None. Cleared for high-intensity training."
                    }
                },
                new()
                {
                    Name = "Nourhan Adel",
                    Email = "nourhan.adel@example.com",
                    Phone = "01188887777",
                    DateOfBirth = new DateOnly(1999, 11, 24),
                    Gender = Gender.Female,
                    JoinDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-1)),
                    Address = new Address { BuildingNumber = 15, Street = "El-Nozha St", City = "Heliopolis" },
                    HealthRecord = new HealthRecord
                    {
                        Height = 165,
                        Weight = 58,
                        BloodType = BloodType.APositive,
                        MedicalConditions = "Mild asthma during extreme cardio."
                    }
                },
                new()
                {
                    Name = "Youssef Khaled",
                    Email = "youssef.khaled@example.com",
                    Phone = "01277776666",
                    DateOfBirth = new DateOnly(1994, 2, 18),
                    Gender = Gender.Male,
                    JoinDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)),
                    Address = new Address { BuildingNumber = 5, Street = "Ahmed Orabi", City = "Mohandessin" },
                    HealthRecord = new HealthRecord
                    {
                        Height = 175,
                        Weight = 78,
                        BloodType = BloodType.BPositive,
                        MedicalConditions = "Previous knee injury rehabilitated."
                    }
                }
            };

            await context.Members.AddRangeAsync(members);
            await context.SaveChangesAsync();
        }

        // 3. Seed Memberships if none exist
        var plans = await context.Plans.ToListAsync();
        var allMembers = await context.Members.ToListAsync();

        if (plans.Count != 0 && allMembers.Count != 0 && !await context.Memberships.AnyAsync())
        {
            var standardPlan = plans.FirstOrDefault(p => p.Name.Contains("Standard") || p.Name.Contains("Month")) ?? plans.First();
            var premiumPlan = plans.Last();

            var memberships = new List<Membership>
            {
                // Active membership for Omar
                new()
                {
                    MemberId = allMembers[0].Id,
                    PlanId = standardPlan.Id,
                    StartDate = DateTime.UtcNow.AddDays(-10),
                    EndDate = DateTime.UtcNow.AddDays(standardPlan.DurationDays - 10)
                },
                // Active membership for Nourhan
                new()
                {
                    MemberId = allMembers[1].Id,
                    PlanId = premiumPlan.Id,
                    StartDate = DateTime.UtcNow.AddDays(-5),
                    EndDate = DateTime.UtcNow.AddDays(premiumPlan.DurationDays - 5)
                },
                // Expired membership for Youssef
                new()
                {
                    MemberId = allMembers[2].Id,
                    PlanId = standardPlan.Id,
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    EndDate = DateTime.UtcNow.AddDays(-60)
                }
            };

            await context.Memberships.AddRangeAsync(memberships);
            await context.SaveChangesAsync();
        }

        // 4. Seed Sessions if none exist
        var categories = await context.Categories.ToListAsync();
        var allTrainers = await context.Trainers.ToListAsync();

        if (categories.Count != 0 && allTrainers.Count != 0 && !await context.Sessions.AnyAsync())
        {
            var sessions = new List<Session>
            {
                // Upcoming Session tomorrow morning
                new()
                {
                    Description = "High-Intensity Functional Strength and Cardio Workout",
                    Capacity = 15,
                    StartDate = DateTime.Today.AddDays(1).AddHours(10), // Tomorrow 10:00 AM
                    EndDate = DateTime.Today.AddDays(1).AddHours(11).AddMinutes(30), // Tomorrow 11:30 AM
                    TrainerId = allTrainers[0].Id,
                    CategoryId = categories[0].Id
                },
                // Upcoming Evening Session in 2 days
                new()
                {
                    Description = "Core Conditioning & Mobility Flow",
                    Capacity = 12,
                    StartDate = DateTime.Today.AddDays(2).AddHours(18), // Day after tomorrow 6:00 PM
                    EndDate = DateTime.Today.AddDays(2).AddHours(19).AddMinutes(15),
                    TrainerId = allTrainers.Count > 1 ? allTrainers[1].Id : allTrainers[0].Id,
                    CategoryId = categories.Count > 1 ? categories[1].Id : categories[0].Id
                },
                // Completed Session yesterday
                new()
                {
                    Description = "Full Body Resistance & Hypertrophy Foundation",
                    Capacity = 20,
                    StartDate = DateTime.Today.AddDays(-1).AddHours(16),
                    EndDate = DateTime.Today.AddDays(-1).AddHours(17).AddMinutes(30),
                    TrainerId = allTrainers[0].Id,
                    CategoryId = categories[0].Id
                }
            };

            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
        }

        // 5. Seed Bookings if none exist
        var allSessions = await context.Sessions.ToListAsync();
        if (allSessions.Count != 0 && allMembers.Count != 0 && !await context.Bookings.AnyAsync())
        {
            var bookings = new List<Booking>
            {
                // Booking for Omar in tomorrow's upcoming session
                new()
                {
                    MemberId = allMembers[0].Id,
                    SessionId = allSessions[0].Id,
                    Date = DateTime.Now.AddMinutes(10),
                    IsAttended = false
                },
                // Booking for Nourhan in tomorrow's upcoming session
                new()
                {
                    MemberId = allMembers[1].Id,
                    SessionId = allSessions[0].Id,
                    Date = DateTime.Now.AddMinutes(15),
                    IsAttended = false
                }
            };

            await context.Bookings.AddRangeAsync(bookings);
            await context.SaveChangesAsync();
        }
    }
}

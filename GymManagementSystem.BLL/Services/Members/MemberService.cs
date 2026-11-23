using GymManagementSystem.BLL.ViewModels.Members;
using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Models.ValueObjects;
using GymManagementSystem.DAL.Repositories.Members;

namespace GymManagementSystem.BLL.Services.Members;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<IEnumerable<MemberViewModel>> GetAllMembersAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(m => new MemberViewModel
        {
            Id = m.Id,
            Photo = m.Photo,
            Name = m.Name,
            Email = m.Email,
            Gender = m.Gender,
            Phone = m.Phone
        }).ToList();
    }

    public async Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int id)
    {
        var member = await _memberRepository.GetWithDetailsAsync(id);
        if (member == null) return null;

        var activeMembership = member.Memberships
            .OrderByDescending(ms => ms.EndDate)
            .FirstOrDefault();

        var addressString = member.Address != null
            ? $"{member.Address.BuildingNumber} · {member.Address.Street} · {member.Address.City}"
            : string.Empty;

        return new MemberDetailsViewModel
        {
            Id = member.Id,
            Photo = member.Photo,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            Gender = member.Gender,
            DateOfBirth = member.DateOfBirth,
            Address = addressString,
            PlanName = activeMembership?.Plan?.Name,
            MembershipStartDate = activeMembership?.StartDate,
            MembershipEndDate = activeMembership?.EndDate,
            HasHealthRecord = member.HealthRecord != null,
            HealthRecordId = member.HealthRecord?.Id
        };
    }

    public async Task<HealthRecordDetailsViewModel?> GetHealthRecordDetailsAsync(int memberId)
    {
        var member = await _memberRepository.GetWithHealthRecordAsync(memberId);
        if (member?.HealthRecord == null) return null;

        return new HealthRecordDetailsViewModel
        {
            MemberId = member.Id,
            MemberName = member.Name,
            Height = member.HealthRecord.Height,
            Weight = member.HealthRecord.Weight,
            BloodType = member.HealthRecord.BloodType.ToString(),
            Note = member.HealthRecord.MedicalConditions
        };
    }

    public async Task<EditMemberViewModel?> GetMemberForEditAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) return null;

        return new EditMemberViewModel
        {
            Id = member.Id,
            Photo = member.Photo,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone,
            BuildingNumber = member.Address?.BuildingNumber ?? 0,
            Street = member.Address?.Street ?? string.Empty,
            City = member.Address?.City ?? string.Empty
        };
    }

    public async Task<DeleteMemberViewModel?> GetMemberForDeleteAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) return null;

        return new DeleteMemberViewModel
        {
            Id = member.Id,
            Name = member.Name,
            Email = member.Email,
            Phone = member.Phone
        };
    }

    public async Task<(bool Success, string? ErrorMessage)> CreateMemberAsync(CreateMemberViewModel model)
    {
        // 1. Email uniqueness
        if (await _memberRepository.IsEmailExistsAsync(model.Email))
        {
            return (false, "A member with this email address already exists.");
        }

        // 2. Phone uniqueness
        if (await _memberRepository.IsPhoneExistsAsync(model.Phone))
        {
            return (false, "A member with this phone number already exists.");
        }

        // 3. Age validation (must be at least 16 years old)
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - model.DateOfBirth.Year;
        if (model.DateOfBirth > today.AddYears(-age)) age--;
        if (age < 16)
        {
            return (false, "Member must be at least 16 years old to register.");
        }

        // 4. Map Entity
        var member = new Member
        {
            Name = model.Name.Trim(),
            Email = model.Email.Trim(),
            Phone = model.Phone.Trim(),
            DateOfBirth = model.DateOfBirth,
            Gender = model.Gender,
            JoinDate = today,
            CreatedAt = DateTime.UtcNow,
            Address = new Address
            {
                BuildingNumber = model.BuildingNumber,
                Street = model.Street.Trim(),
                City = model.City.Trim()
            },
            HealthRecord = new HealthRecord
            {
                Height = model.Height,
                Weight = model.Weight,
                BloodType = model.BloodType,
                MedicalConditions = model.Note?.Trim(),
                CreatedAt = DateTime.UtcNow
            }
        };

        _memberRepository.Add(member);
        await _memberRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateMemberAsync(EditMemberViewModel model)
    {
        var member = await _memberRepository.GetByIdAsync(model.Id);
        if (member == null)
        {
            return (false, "Member not found.");
        }

        // Email uniqueness check excluding current member
        if (await _memberRepository.IsEmailExistsAsync(model.Email, model.Id))
        {
            return (false, "This email address is already in use by another user.");
        }

        // Phone uniqueness check excluding current member
        if (await _memberRepository.IsPhoneExistsAsync(model.Phone, model.Id))
        {
            return (false, "This phone number is already in use by another user.");
        }

        member.Email = model.Email.Trim();
        member.Phone = model.Phone.Trim();
        member.UpdatedAt = DateTime.UtcNow;

        if (member.Address == null)
        {
            member.Address = new Address();
        }
        member.Address.BuildingNumber = model.BuildingNumber;
        member.Address.Street = model.Street.Trim();
        member.Address.City = model.City.Trim();

        _memberRepository.Update(member);
        await _memberRepository.SaveChangesAsync();

        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteMemberAsync(int id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null)
        {
            return (false, "Member not found.");
        }

        // Check if member has active bookings
        if (await _memberRepository.HasActiveBookingsAsync(id))
        {
            return (false, "Cannot delete member with active session reservations.");
        }

        _memberRepository.Delete(member);
        await _memberRepository.SaveChangesAsync();

        return (true, null);
    }
}

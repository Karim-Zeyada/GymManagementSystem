using GymManagementSystem.DAL.Models;
using GymManagementSystem.DAL.Repositories.Generic;

namespace GymManagementSystem.DAL.Repositories.Members;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetWithHealthRecordAsync(int id);
    Task<Member?> GetWithDetailsAsync(int id);
    Task<bool> IsEmailExistsAsync(string email, int? excludeMemberId = null);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeMemberId = null);
    Task<bool> HasActiveBookingsAsync(int memberId);
    Task<int> GetActiveMembersCountAsync(DateTime now);
    Task<int> GetTotalCountAsync();
}

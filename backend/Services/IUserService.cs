using MyApp.Models;

namespace MyApp.Services
{
    public interface IUserService
    {
        Task<(IEnumerable<User> items, int total)> GetUsersAsync(int page, int pageSize, string? titleFilter, string? sort, string? order);
        Task<User?> GetByIdAsync(int id);
        Task<bool> UpdateCompletedAsync(int id, bool completed);
        Task<(int added, int skipped, List<string> messages)> SyncFromExternalAsync();
    }
}
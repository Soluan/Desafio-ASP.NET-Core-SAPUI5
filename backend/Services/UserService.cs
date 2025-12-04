using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        public UserService(AppDbContext db) => _db = db;

        public async Task<(IEnumerable<User> items, int total)> GetUsersAsync(int page, int pageSize, string? titleFilter, string? sort, string? order)
        {
            var query = _db.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(titleFilter))
                query = query.Where(t => t.Title.Contains(titleFilter));
            bool asc = string.Equals(order, "asc", StringComparison.OrdinalIgnoreCase);
            query = sort?.ToLower() switch
            {
                "title" => asc ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "id" => asc ? query.OrderBy(t => t.ID) : query.OrderByDescending(t => t.ID),
                _ => query.OrderBy(t => t.ID)
            };
            var total = await query.CountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, total);
        }

        public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);

        public async Task<bool> UpdateCompletedAsync(int id, bool completed)
        {
            var todo = await _db.Users.FindAsync(id);
            if (todo == null) return false;
            if (!completed)
            {
                var incompleteCount = await _db.Users.CountAsync(t => t.UserID == todo.UserID && !t.Completed && t.ID != id);
                if (incompleteCount >= 5)
                    throw new InvalidOperationException($"Usuário {todo.UserID} já tem 5 tarefas incompletas. Não é permitido ter mais que 5.");
            }
            todo.Completed = completed;
            _db.Users.Update(todo);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<(int added, int skipped, List<string> messages)> SyncFromExternalAsync()
        {
            var client = new HttpClient();
            var url = "https://jsonplaceholder.typicode.com/todos";
            var resp = await client.GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var content = await resp.Content.ReadAsStringAsync();
            var external = System.Text.Json.JsonSerializer.Deserialize<List<User>>(content, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<User>();

            int added = 0, skipped = 0;
            var messages = new List<string>();
            foreach (var ext in external)
            {
                var existing = await _db.Users.FindAsync(ext.ID);
                if (existing != null)
                {
                    existing.Title = ext.Title;
                    existing.Completed = ext.Completed;
                    existing.UserID = ext.UserID;
                    _db.Users.Update(existing);
                }
                else
                {
                    if (!ext.Completed)
                    {
                        var incompleteCount = await _db.Users.CountAsync(t => t.UserID == ext.UserID && !t.Completed);
                        if (incompleteCount >= 5)
                        {
                            skipped++;
                            messages.Add($"Pulou id {ext.ID} (user {ext.UserID}) - excederia 5 incompletas.");
                            continue;
                        }
                    }
                    await _db.Users.AddAsync(ext);
                    added++;
                }
            }
            await _db.SaveChangesAsync();
            return (added, skipped, messages);
        }
    }
}
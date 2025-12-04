using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Data;
using MyApp.Models;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.Single(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>)
            );
            services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("TestsDb"));

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            db.Database.EnsureCreated();

            if (!db.Users.Any())
            {
                db.Users.AddRange(
                    new User { ID = 1, UserID = 1, Title = "t1", Completed = false },
                    new User { ID = 2, UserID = 1, Title = "t2", Completed = false },
                    new User { ID = 3, UserID = 1, Title = "t3", Completed = false },
                    new User { ID = 4, UserID = 1, Title = "t4", Completed = false },
                    new User { ID = 5, UserID = 1, Title = "t5", Completed = false },
                    new User { ID = 6, UserID = 1, Title = "t6", Completed = true }
                );
                db.SaveChanges();
            }
        });
    }
}

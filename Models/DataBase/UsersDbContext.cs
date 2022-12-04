using Microsoft.EntityFrameworkCore;

namespace QualificationSite.Models.DataBase;

public class UsersDbContext : DbContext
{
    public UsersDbContext(DbContextOptions<UsersDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Profile> Profiles { get; set; }
    public DbSet<User> Users { get; set; }
}
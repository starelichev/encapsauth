namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    private readonly IConfiguration Configuration;

    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var connectionString = "Server=localhost;Database=enematicsys;User=root;Password=c3g4mgggbM!;";
        // in memory database used for simplicity, change to a real db for production applications
        options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}
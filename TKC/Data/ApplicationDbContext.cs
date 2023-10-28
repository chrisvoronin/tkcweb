using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TKC.Models;

namespace TKC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Music> Musics { get; set; }
    public DbSet<Sermon> Sermons { get; set; }
    public DbSet<ShortTake> ShortTakes { get; set; }
    public DbSet<AppSettingModel> AppSettings { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Music>(entity =>
        {
            entity.ToTable("Music");
        });

        modelBuilder.Entity<Sermon>(entity =>
        {
            entity.ToTable("Sermons");
        });

        modelBuilder.Entity<ShortTake>(entity =>
        {
            entity.ToTable("ShortTakes");
        });

        modelBuilder.Entity<AppSettingModel>(entity =>
        {
            entity.ToTable("AppSettings");
        });
    }

}


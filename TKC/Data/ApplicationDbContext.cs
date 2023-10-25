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
            entity.ToTable("Sermon");
        });

        modelBuilder.Entity<ShortTake>(entity =>
        {
            entity.ToTable("ShortTake");
        });

        modelBuilder.Entity<AppSettingModel>(entity =>
        {
            entity.ToTable("AppSettings");
        });
    }

    // Method to get all music records
    public List<Music> GetAllMusic()
    {
        return Musics.ToList();
    }

    // Method to get music by ID
    public Music? GetMusicById(int id)
    {
        return Musics.FirstOrDefault(m => m.Id == id);
    }

}


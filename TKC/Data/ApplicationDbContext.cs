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
    public DbSet<HTMLContent> HTMLContents { get; set; }
    public DbSet<Staff> Employees { get; set; }
    public DbSet<ResourceGroup> ResourceGroups { get; set; }
    public DbSet<ResourceItem> Resources { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ResourceGroup>(entity =>
        {
            entity.ToTable("ResourceGroups");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<ResourceItem>()
            .ToTable("Resources")
            .HasOne(ri => ri.ResourceGroup)
            .WithMany(rg => rg.Items)
            .HasForeignKey(ri => ri.GroupId);


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

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.ToTable("Staff");
        });

        modelBuilder.Entity<HTMLContent>(entity =>
        {
            entity.ToTable("HTMLContent");
        });

        modelBuilder.Entity<AppSettingModel>(entity =>
        {
            entity.ToTable("AppSettings");
            entity.HasKey(e => new { e.Key });
        });
    }

}


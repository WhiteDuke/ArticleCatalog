using ArticleCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticleCatalog.DataAccess;

public class ArticlesDbContext : DbContext
{
    public ArticlesDbContext(DbContextOptions<ArticlesDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName($"{nameof(Tag)}_{nameof(Tag.Name)}_Unique");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CreatedDate).IsRequired().HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedDate).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.Property(e => e.ArticleId).IsRequired();
            entity.Property(e => e.TagId).IsRequired();
            entity.HasIndex(e => new { e.ArticleId, e.TagId })
                .IsUnique()
                .HasDatabaseName($"{nameof(ArticleTag)}_{nameof(ArticleTag.ArticleId)}_Unique");

            entity.HasOne(e => e.Article)
                    .WithMany(e => e.ArticleTags)
                    .HasForeignKey(e => e.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                .WithMany(e => e.ArticleTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
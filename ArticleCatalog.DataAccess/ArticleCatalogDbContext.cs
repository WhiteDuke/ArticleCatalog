using ArticleCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArticleCatalog.DataAccess;

public sealed class ArticleCatalogDbContext : DbContext
{
    private const int MaxLength256 = 256;
    private const int MaxLength1024 = 1024;
    
    public ArticleCatalogDbContext(DbContextOptions<ArticleCatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Article> Articles { get; set; }

    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<ArticleTag> ArticleTags { get; set; }
    
    public DbSet<Section> Sections { get; set; }
    
    public DbSet<SectionTag> SectionTags { get; set; }

    public DbSet<SectionArticle> SectionArticles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>(entity =>
        {
            entity.Property(e => e.Name).IsRequired().HasMaxLength(MaxLength256);

            entity.HasIndex(e => e.Name)
                .IsUnique()
                .HasDatabaseName($"{nameof(Tag)}_{nameof(Tag.Name)}_Unique");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(MaxLength256);
            entity.Property(e => e.CreatedDate).IsRequired().HasColumnType("timestamp with time zone");
            entity.Property(e => e.UpdatedDate).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<ArticleTag>(entity =>
        {
            entity.Property(e => e.ArticleId).IsRequired();
            entity.Property(e => e.TagId).IsRequired();
            entity.Property(e => e.Order).IsRequired();

            entity.HasIndex(e => new { e.ArticleId, e.TagId })
                .IsUnique()
                .HasDatabaseName($"{nameof(ArticleTags)}_{nameof(ArticleTag.ArticleId)}_{nameof(ArticleTag.TagId)}_Unique");

            entity.HasOne(e => e.Article)
                    .WithMany(e => e.ArticleTags)
                    .HasForeignKey(e => e.ArticleId)
                    .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                .WithMany(e => e.ArticleTags)
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.Property(e => e.Title).IsRequired().HasMaxLength(MaxLength1024);
        });

        modelBuilder.Entity<SectionTag>(entity =>
        {
            entity.Property(e => e.SectionId).IsRequired();
            entity.Property(e => e.TagId).IsRequired();
            
            entity.HasIndex(e => new { e.SectionId , e.TagId })
                .IsUnique()
                .HasDatabaseName($"{nameof(SectionTags)}_{nameof(SectionTag.SectionId)}_{nameof(SectionTag.TagId)}_Unique");

            entity.HasOne(e => e.Section)
                .WithMany(e => e.SectionTags)
                .HasForeignKey(x => x.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SectionArticle>(entity =>
        {
            entity.Property(e => e.ArticleId).IsRequired();
            entity.Property(e => e.SectionId).IsRequired();

            entity.HasIndex(e => new { e.SectionId , e.ArticleId })
                .IsUnique()
                .HasDatabaseName($"{nameof(SectionArticles)}_{nameof(SectionArticle.SectionId)}_{nameof(SectionArticle.ArticleId)}_Unique");

            entity.HasOne(e => e.Article)
                .WithMany(e => e.SectionArticles)
                .HasForeignKey(e => e.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Section)
                .WithMany(e => e.SectionArticles)
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
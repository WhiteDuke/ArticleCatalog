using System.Collections.Generic;

namespace ArticleCatalog.Domain.Entities;

public class Section
{
    public int Id { get; set; }

    public string Title { get; set; }

    public List<SectionArticle> SectionArticles { get; set; } = [];

    public List<SectionTag> SectionTags { get; set; } = [];
}
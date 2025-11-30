namespace ArticleCatalog.Domain.Entities;

public class SectionTag
{
    public int Id { get; set; }
    
    public int SectionId { get; set; }
    
    public Section Section { get; set; }
    
    public int TagId { get; set; }
    
    public Tag Tag { get; set; }
}
using System.Collections.Generic;
using System.Text;

namespace ArticleCatalog.Service.Helpers;

public static class SectionHelper
{
    public static string GetSectionTitle(ICollection<string> tags)
    {
        if (tags.Count == 0)
        {
            return "Без тэгов";
        }
        
        var sb = new StringBuilder();
        foreach (var tag in tags)
        {
            if (sb.Length > 0)
            {
                sb.Append(',');
            }

            sb.Append(tag);

            if (sb.Length >= GlobalConstants.SectionTitleLength)
            {
                break;
            }
        }

        return sb.Length >= GlobalConstants.SectionTitleLength 
            ? sb.ToString(0, GlobalConstants.SectionTitleLength)
            : sb.ToString();
    } 
}
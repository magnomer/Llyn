using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LOutlineNote
{
    public static void LOutlineNoteAppend(StringBuilder page, string? markdown)
    {
        ArgumentNullException.ThrowIfNull(page);

        string canon = LMarkdown.LMarkdownNormalize(markdown);

        if (canon.Length == 0)
        {
            return;
        }

        bool fenced = false;

        foreach (string line in canon.Split('\n'))
        {
            string bare = line.TrimStart();

            if (bare.StartsWith("```", StringComparison.Ordinal))
            {
                fenced = !fenced;
            }
            else if (!fenced && bare.StartsWith('#'))
            {
                int level = bare.Length - bare.TrimStart('#').Length;

                if (level <= 6 && (level == bare.Length || bare[level] == ' '))
                {
                    page.Append('#', Math.Min(level + 2, 6)).Append(bare[level..]).Append('\n');
                    continue;
                }
            }

            page.Append(line).Append('\n');
        }

        page.Append('\n');
    }
}

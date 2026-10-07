using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryPhonology
{
    public static void LLiveryPhonologyAppend(StringBuilder sheet, LLiveryLanguage language, Func<long, string> note)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(note);

        if (language.LLiveryLanguagePronunciation.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"llyn-phonology\">\n\n")
            .Append(LLiveryRime.LLiveryRowFormat([string.Empty, string.Empty]))
            .Append(LLiveryRime.LLiveryRowFormat(["---", "---"]));
        foreach (LCatalogPronunciation row in language.LLiveryLanguagePronunciation)
        {
            LEntry entry = row.LCatalogPronunciationEntry;
            sheet.Append(LLiveryRime.LLiveryRowFormat(
            [
                LLiveryEtymology.LLiveryLinkFormat(entry.LEntryHeadword, entry.LEntryId, note),
                LLiveryHeader.LLiveryTextFormat(row.LCatalogPronunciationText),
            ]));
        }

        sheet.Append("\n</div>\n\n");
    }
}

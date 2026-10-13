using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LSheetLineage
{
    internal static void LSheetIncomingAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelIncoming) is LPortraitSection incoming)
        {
            LSheetSection.LSheetSectionAppend(sheet, incoming);
        }
    }

    internal static void LSheetEtymologyAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelEtymology) is LPortraitSection etymology)
        {
            LSheetSection.LSheetSectionAppend(sheet, etymology);
        }
    }

    internal static void LSheetAnnotationAppend(StringBuilder sheet, LPortraitPage portrait, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(label);

        if (LSheetSection.LSheetSectionFind(portrait, label.LPortraitLabelNote) is LPortraitSection note)
        {
            LSheetSection.LSheetSectionAppend(sheet, note);
        }
    }
}

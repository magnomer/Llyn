using System;
using System.Collections.Generic;

namespace Llyn.Core;

public static class LPortraitText
{
    public static string LPortraitTextRead(LStateValue? value, string mark)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value.LStateValueState switch
        {
            _ when value.LStateValueUnreadable => value.LStateValueShow(),
            LState.LStateSpecified => value.LStateValueShow(),
            LState.LStateUnknown => mark,
            _ => string.Empty,
        };
    }

    public static string LPortraitTitleRead(LStateValue value, string vacant, string mark)
    {
        ArgumentNullException.ThrowIfNull(vacant);

        string text = LPortraitTextRead(value, mark);
        return text.Length > 0 ? text : vacant;
    }

    public static IReadOnlyList<string> LPortraitTextRead(LPortraitPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        List<string> shown = [];
        LPortraitTextAdd(shown, page.LPortraitPageTitle);
        LPortraitTextAdd(shown, page.LPortraitPageLanguage);
        LPortraitTextAdd(shown, page.LPortraitPageLine);
        LPortraitTextAdd(shown, page.LPortraitPageChip);
        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            shown.AddRange(LPortraitTextRead(section));
        }

        return shown;
    }

    public static IReadOnlyList<string> LPortraitTextRead(LPortraitSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        List<string> shown = [];
        if (section.LPortraitSectionRole
            is LPortraitRole.LPortraitRoleBand or LPortraitRole.LPortraitRoleCard or LPortraitRole.LPortraitRoleKind)
        {
            LPortraitTextAdd(shown, section.LPortraitSectionHeading);
        }

        LPortraitTextAdd(shown, section.LPortraitSectionLine);
        LPortraitTextAdd(shown, section.LPortraitSectionNote);
        LPortraitTextAdd(shown, section.LPortraitSectionChip);

        foreach (LPortraitLink link in section.LPortraitSectionLink)
        {
            LPortraitTextAdd(shown, link.LPortraitLinkHeadword);
            LPortraitTextAdd(shown, link.LPortraitLinkLanguage);
        }

        foreach (LPortraitMedia video in section.LPortraitSectionVideo)
        {
            LPortraitTextAdd(shown, video.LPortraitMediaLocation);
            LPortraitTextAdd(shown, video.LPortraitMediaSpan);
        }

        foreach (LPortraitSection child in section.LPortraitSectionChild)
        {
            shown.AddRange(LPortraitTextRead(child));
        }

        return shown;
    }

    private static void LPortraitTextAdd(List<string> shown, string text)
    {
        if (text.Length > 0)
        {
            shown.Add(text);
        }
    }

    private static void LPortraitTextAdd(List<string> shown, IReadOnlyList<string> chips)
    {
        foreach (string chip in chips)
        {
            LPortraitTextAdd(shown, chip);
        }
    }

    private static void LPortraitTextAdd(List<string> shown, IReadOnlyList<LPortraitLine> lines)
    {
        foreach (LPortraitLine line in lines)
        {
            LPortraitTextAdd(shown, line.LPortraitLineLabel);
            LPortraitTextAdd(shown, line.LPortraitLineText);
        }
    }
}

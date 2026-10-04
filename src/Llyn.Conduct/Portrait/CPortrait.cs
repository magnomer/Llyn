using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

internal static class CPortrait
{
    private static readonly string[] LPortraitLabelKeys =
    [
        "Display.Unknown",
        "Display.MeaningSingle",
        "Display.MeaningPlural",
        "Display.CollocationSingle",
        "Display.Collocation",
        "Display.Translated",
        "Display.Note",
        "Portrait.Form",
        "Portrait.Paradigm",
        "Frequency.Title",
        "Portrait.Glyph",
        "Display.Script",
        "Display.Fanqie",
        "Portrait.Example",
        "Portrait.Gloss",
        "Reference.Title",
        "Portrait.Mention",
        "Portrait.Etymology",
        "Portrait.Situation",
        "Portrait.Register",
        "Portrait.Translation",
        "Portrait.Tag",
    ];

    internal static IReadOnlyList<CPortraitChoice> LPortraitChoiceRead()
    {
        List<CPortraitChoice> choices = LPortraitPort.LEngineMediumRead()
            .Select(static row => LPortraitChoiceCreate(
                row.Item1, row.Item2.Replace("|", string.Empty, StringComparison.Ordinal), row.Item3))
            .ToList();
        if (choices.Count == 0)
        {
            return [LPortraitChoiceCreate(LPortraitMedium.LPortraitMediumMarkup, string.Empty, true)];
        }

        int chosen = Math.Max(0, choices.FindIndex(static choice => choice.CPortraitChoiceChosen));
        return choices
            .Select((choice, index) => choice with { CPortraitChoiceChosen = index == chosen })
            .ToList();
    }

    private static CPortraitChoice LPortraitChoiceCreate(LPortraitMedium medium, string suffix, bool chosen)
    {
        return medium switch
        {
            LPortraitMedium.LPortraitMediumMarkup => new CPortraitChoice(
                "Export.Markup", suffix, chosen, CPortraitMedium.CPortraitMediumMarkup),
            LPortraitMedium.LPortraitMediumHtml => new CPortraitChoice(
                "Export.Html", suffix, chosen, CPortraitMedium.CPortraitMediumHtml),
            LPortraitMedium.LPortraitMediumMarkdown => new CPortraitChoice(
                "Export.Markdown", suffix, chosen, CPortraitMedium.CPortraitMediumMarkdown),
            LPortraitMedium.LPortraitMediumDocx => new CPortraitChoice(
                "Export.Docx", suffix, chosen, CPortraitMedium.CPortraitMediumDocx),
            LPortraitMedium.LPortraitMediumPdf => new CPortraitChoice(
                "Export.Pdf", suffix, chosen, CPortraitMedium.CPortraitMediumPdf),
            _ => throw new ArgumentOutOfRangeException(nameof(medium), medium, null),
        };
    }

    internal static async Task LPortraitFileExport(
        CEnvoy envoy, LSettingsPort settings, Func<string> fileRead, Func<string, LPortraitMedium, Task> export)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(fileRead);
        ArgumentNullException.ThrowIfNull(export);

        string file;
        try
        {
            file = fileRead();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Export.NameFailed", exception);
            return;
        }

        try
        {
            (string? path, CPortraitMedium format) = envoy.CEnvoyFileRead(file, LPortraitChoiceRead());
            if (path is null)
            {
                return;
            }

            await export(path, LPortraitMediumRead(format));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Export.Failed", exception);
        }
    }

    internal static async Task LPortraitTicketPrint(
        CEnvoy envoy, LSettingsPort settings, Func<LPressTicket, Task> print)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(print);

        try
        {
            if (envoy.CEnvoyTicketRead() is not CPressTicket ticket)
            {
                return;
            }

            await print(LPortraitTicketRead(ticket));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(envoy, settings, "Print.Failed", exception);
        }
    }

    internal static LPortraitMedium LPortraitMediumRead(CPortraitMedium medium)
    {
        return medium switch
        {
            CPortraitMedium.CPortraitMediumMarkup => LPortraitMedium.LPortraitMediumMarkup,
            CPortraitMedium.CPortraitMediumHtml => LPortraitMedium.LPortraitMediumHtml,
            CPortraitMedium.CPortraitMediumMarkdown => LPortraitMedium.LPortraitMediumMarkdown,
            CPortraitMedium.CPortraitMediumDocx => LPortraitMedium.LPortraitMediumDocx,
            CPortraitMedium.CPortraitMediumPdf => LPortraitMedium.LPortraitMediumPdf,
            _ => throw new ArgumentOutOfRangeException(nameof(medium), medium, null),
        };
    }

    internal static LPressTicket LPortraitTicketRead(CPressTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);

        return LPortraitPort.LEngineTicketRead(
            ticket.CPressTicketPrinter,
            ticket.CPressTicketWidth,
            ticket.CPressTicketHeight,
            ticket.CPressTicketLandscape,
            ticket.CPressTicketCopies,
            ticket.CPressTicketCollated,
            LPortraitSideRead(ticket.CPressTicketSide),
            LPortraitInkRead(ticket.CPressTicketInk));
    }

    private static LPressSide LPortraitSideRead(CPressSide side)
    {
        return side switch
        {
            CPressSide.CPressSideDefault => LPressSide.LPressSideDefault,
            CPressSide.CPressSideSingle => LPressSide.LPressSideSingle,
            CPressSide.CPressSideLong => LPressSide.LPressSideLong,
            CPressSide.CPressSideShort => LPressSide.LPressSideShort,
            _ => throw new ArgumentOutOfRangeException(nameof(side), side, null),
        };
    }

    private static LPressInk LPortraitInkRead(CPressInk ink)
    {
        return ink switch
        {
            CPressInk.CPressInkDefault => LPressInk.LPressInkDefault,
            CPressInk.CPressInkColor => LPressInk.LPressInkColor,
            CPressInk.CPressInkGray => LPressInk.LPressInkGray,
            _ => throw new ArgumentOutOfRangeException(nameof(ink), ink, null),
        };
    }

    internal static LPortraitLabel LPortraitLabelRead(LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        return LPortraitPort.LEngineLabelRead(LPortraitLabelKeys.Select(settings.LEngineTextRead).ToList());
    }

    internal static LPortraitLegend LPortraitLegendRead(LSettingsPort settings, string realm)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(realm);

        string[] keys =
        [
            "Display.Unknown",
            realm == "Example" ? "Example.Unwritten" : realm + ".Untitled",
            "Example.Unwritten",
            realm + ".UsageNone",
            realm + ".UsageOne",
            realm + ".UsageMany",
            "Example.Translation",
            "Reference.Title",
            "Source.Author",
            "Source.Year",
            "Source.Url",
            "Source.Note",
            "Situation.Description",
        ];
        Dictionary<string, string> kinds = [];
        foreach (string kind in LPortraitPort.LEngineKindRead())
        {
            kinds[kind] = settings.LEngineTextRead(kind);
        }

        return LPortraitPort.LEngineLegendRead(keys.Select(settings.LEngineTextRead).ToList(), kinds);
    }
}

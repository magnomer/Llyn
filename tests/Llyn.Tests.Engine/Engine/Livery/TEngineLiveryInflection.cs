using System.Text.RegularExpressions;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineLiveryInflection
{
    [Fact]
    public void LiveryFormat_InflectionView_WritesExpandedTableWithHeaderLabelAndForm()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "hablar", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to speak", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LParadigmTable collapsed = TInterfaceInflection.TParadigmTableCreate(
            [],
            [TInterfaceInflection.TParadigmLineCreate(string.Empty, "Inflection.Short", [TLiveryFormCreate("ha")])]);
        LParadigmTable expanded = TInterfaceInflection.TParadigmTableCreate(
            ["Inflection.FirstSingular"],
            [
                TInterfaceInflection.TParadigmLineCreate(
                    "Inflection.Indicative", "Inflection.Present", [TLiveryFormCreate("hablo")]),
            ]);
        page = page with { LLiveryPageInflection = TInterfaceInflection.TParadigmViewCreate(collapsed, expanded) };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        int details = body.IndexOf("<details class=\"llyn-inflection-full\">", StringComparison.Ordinal);
        int close = body.IndexOf("</details>\n", StringComparison.Ordinal);
        Assert.InRange(details, 0, close);
        string full = body[details..close];
        Assert.Contains("<div class=\"llyn-inflection-box\">\n<details", body, StringComparison.Ordinal);
        Assert.Contains(
            "<summary><span class=\"llyn-inflection-switch-short\">Paradigm.Short</span>"
            + "<span class=\"llyn-inflection-switch-long\">Paradigm.Full</span></summary>\n",
            full,
            StringComparison.Ordinal);
        Assert.Contains("<table class=\"llyn-inflection\">\n", full, StringComparison.Ordinal);
        Assert.Contains(
            "<tr><th></th><th></th><th>Inflection.FirstSingular</th></tr>\n", full, StringComparison.Ordinal);
        Assert.Contains(
            "<tr><td class=\"llyn-group\">Inflection.Indicative</td>"
            + "<td class=\"llyn-label\">Inflection.Present</td><td>hablo</td></tr>\n",
            full,
            StringComparison.Ordinal);
        Assert.Contains("</table>\n</div>\n\n", body, StringComparison.Ordinal);
        Assert.DoesNotContain("Inflection.Short", full, StringComparison.Ordinal);
        Assert.Contains(
            "</details>\n<table class=\"llyn-inflection llyn-inflection-short\">\n"
            + "<tr><td class=\"llyn-group\"></td><td class=\"llyn-label\">Inflection.Short</td><td>ha</td></tr>\n"
            + "</table>\n</div>\n\n",
            body,
            StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_EmptyCollapsedSheet_WritesOneTableWithoutSwitch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "comer", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to eat", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LParadigmTable collapsed = TInterfaceInflection.TParadigmTableCreate([], []);
        LParadigmTable expanded = TInterfaceInflection.TParadigmTableCreate(
            [],
            [
                TInterfaceInflection.TParadigmLineCreate(
                    string.Empty, "Inflection.Present", [TLiveryFormCreate("como")]),
            ]);
        page = page with { LLiveryPageInflection = TInterfaceInflection.TParadigmViewCreate(collapsed, expanded) };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<div class=\"llyn-inflection-box\">\n<table class=\"llyn-inflection\">\n"
            + "<tr><td class=\"llyn-group\"></td><td class=\"llyn-label\">Inflection.Present</td><td>como</td></tr>\n"
            + "</table>\n</div>\n\n",
            body,
            StringComparison.Ordinal);
        Assert.Single(Regex.Matches(body, "<table class=\"llyn-inflection"));
        Assert.DoesNotContain("<details class=\"llyn-inflection-full\">", body, StringComparison.Ordinal);
        Assert.DoesNotContain("llyn-inflection-short", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_IdenticalSheets_WritesOneTableWithoutSwitch()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "vivir", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to live", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LParadigmTable table = TInterfaceInflection.TParadigmTableCreate(
            [],
            [
                TInterfaceInflection.TParadigmLineCreate(
                    string.Empty, "Inflection.Present", [TLiveryFormCreate("vivo")]),
            ]);
        page = page with { LLiveryPageInflection = TInterfaceInflection.TParadigmViewCreate(table, table) };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Single(Regex.Matches(body, "<table"));
        Assert.DoesNotContain("<details class=\"llyn-inflection-full\">", body, StringComparison.Ordinal);
        Assert.Contains("<div class=\"llyn-inflection-box\">\n<table class=\"llyn-inflection\">\n", body,
            StringComparison.Ordinal);
        Assert.Contains("</table>\n</div>\n\n", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_MissingAndMarkedForms_RenderMutedGlyphWithTipAndMarkedRun()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "tener", "English", string.Empty, string.Empty, [TInterface.TCardCreate("to have", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));
        LParadigmTable table = TInterfaceInflection.TParadigmTableCreate(
            [],
            [
                TInterfaceInflection.TParadigmLineCreate(string.Empty, "Inflection.Present",
                [
                    TInterfaceInflection.TParadigmFormCreate(
                        "tengo",
                        [TInterfaceInflection.TInflectionMarkCreate(0, 4)],
                        null,
                        4),
                    TInterfaceInflection.TParadigmFormCreate("…", [], "Paradigm.Lost"),
                    TInterfaceInflection.TParadigmFormCreate("—", [], "Paradigm.Unknown"),
                ]),
            ]);
        page = page with { LLiveryPageInflection = TInterfaceInflection.TParadigmViewCreate(table, table) };

        string body = TInterface.TLiveryFormat(page, static key => key).LLiveryNoteBody;

        Assert.Contains(
            "<td><span class=\"llyn-marked\">teng</span><span class=\"llyn-cut\">-</span>o</td>",
            body,
            StringComparison.Ordinal);
        Assert.Contains(
            "<td><span class=\"llyn-muted\" title=\"Paradigm.Lost\">…</span></td>", body, StringComparison.Ordinal);
        Assert.Contains(
            "<td><span class=\"llyn-muted\" title=\"Paradigm.Unknown\">—</span></td>", body, StringComparison.Ordinal);
    }

    [Fact]
    public void LiveryFormat_NullInflectionView_WritesNoInflectionTable()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "water", "English", string.Empty, string.Empty, [TInterface.TCardCreate("a liquid", 1)], []));
        LLiveryPage page = Assert.IsType<LLiveryPage>(engine.TLiveryRead(entry.LEntryId));

        string body = TInterface.TLiveryFormat(page with { LLiveryPageInflection = null }, static key => key)
            .LLiveryNoteBody;

        Assert.DoesNotContain("llyn-inflection", body, StringComparison.Ordinal);
    }

    private static LParadigmForm TLiveryFormCreate(string text) =>
        TInterfaceInflection.TParadigmFormCreate(text, [], null);
}

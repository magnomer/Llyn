using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDisplaySound
{
    [Fact]
    public void DisplayGlyphRead_HanjaRow_AnswersTheRowReadyToShow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        wing.CWingEntryOpen(TDisplayKoreanSave(engine).LEntryId);

        CLecternGlyph glyph = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();

        Assert.True(glyph.CLecternGlyphShown);
        Assert.Equal("Scheme.Hanja", glyph.CLecternGlyphKey);
        Assert.Equal("Hanja", glyph.CLecternGlyphName);
        Assert.Equal(
            [new CGlyphCell("漢", "Classical Chinese", true), new CGlyphCell("字", "Classical Chinese", true)],
            glyph.CLecternGlyphCells);
        Assert.Equal("Batang, Malgun Gothic", glyph.CLecternGlyphFont.CFontFamily);
    }

    [Fact]
    public void DisplayFontRead_ShownEntryOrNothingShown_ReadsThePackFontOfTheShownLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CFont blank = wing.CWingDisplay.CDisplaySound.CDisplayFontRead(CFontRole.CFontRoleHeadword);
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);

        CDisplaySound sound = wing.CWingDisplay.CDisplaySound;

        Assert.Equal(new CFont(null, null, CFontSlant.CFontSlantTheme), blank);
        Assert.Equal(
            new CFont("Segoe UI", 40, CFontSlant.CFontSlantTheme),
            sound.CDisplayFontRead(CFontRole.CFontRoleHeadword));
        Assert.Equal(
            new CFont("Georgia, Segoe UI", 15, CFontSlant.CFontSlantItalic),
            sound.CDisplayFontRead(CFontRole.CFontRoleGloss));
    }

    [Fact]
    public void DisplayGlyphRead_LanguageWithoutGlyphOrNothingShown_AnswersTheHiddenRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CLecternGlyph blank = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();
        wing.CWingEntryOpen(TDisplayEnglishSave(engine).LEntryId);

        CLecternGlyph glyph = wing.CWingDisplay.CDisplaySound.CDisplayGlyphRead();

        Assert.False(blank.CLecternGlyphShown);
        Assert.Empty(blank.CLecternGlyphCells);
        Assert.False(glyph.CLecternGlyphShown);
        Assert.Empty(glyph.CLecternGlyphName);
    }

    [Fact]
    public void DisplayGlyphOpen_LinkedCell_RaisesTheCharacterEntryForTheLibrary()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CWing wing = TDisplayWingPrepare(atelier, asked);
        List<long> opened = [];
        wing.CWingDisplay.CDisplaySound.CDisplayRowChosen += (tab, entry) =>
        {
            Assert.Equal("Library", tab);
            opened.Add(entry);
        };

        bool shown = wing.CWingDisplay.CDisplaySound.CDisplayGlyphOpen("漢", "Classical Chinese");
        bool refused = wing.CWingDisplay.CDisplaySound.CDisplayGlyphOpen(" ", "Classical Chinese");

        Assert.True(shown);
        Assert.True(Assert.Single(opened) > 0);
        Assert.False(refused);
        Assert.Equal(["Glyph.OpenFailed"], asked);
    }

    [Fact]
    public void DisplayTranscriptionRead_GlyphAndEmptyRows_ListsOnlyOtherFilledRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        Assert.Empty(wing.CWingDisplay.CDisplaySound.CDisplayTranscriptionRead());
        wing.CWingEntryOpen(TDisplayKoreanSave(engine).LEntryId);

        IReadOnlyList<CTranscriptionDraft> rows = wing.CWingDisplay.CDisplaySound.CDisplayTranscriptionRead();

        CTranscriptionDraft row = Assert.Single(rows);
        Assert.Equal("Revised Romanization", row.CTranscriptionDraftScheme);
        Assert.Equal("hanja", row.CTranscriptionDraftText);
    }

    [Fact]
    public void DisplayReflexRead_StoredEntry_AnswersWrittenRowsWithTheirAnchors()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        CWing wing = TDisplayWingPrepare(atelier, []);
        CLecternReflex blank = wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();
        LEntry kindle = engine.TEngineEntrySave(TDisplayDraftCreate(
            "kindle",
            "English",
            [
                TInterface.TReflexDraftCreate("Korean", string.Empty, "a"),
                TInterface.TReflexDraftCreate("Korean", string.Empty, " "),
            ]));
        wing.CWingEntryOpen(kindle.LEntryId);

        CLecternReflex reflex = wing.CWingDisplay.CDisplaySound.CDisplayReflexRead();

        Assert.Empty(blank.CLecternReflexRows);
        CReflex row = Assert.Single(reflex.CLecternReflexRows);
        Assert.Equal("a", row.CReflexText);
        Assert.True(row.CReflexLead);
        Assert.False(reflex.CLecternReflexAnchor.CLecternAnchorOffered);
        Assert.Equal(string.Empty, reflex.CLecternReflexAnchor.CLecternAnchorTexts[row.CReflexId]);
        Assert.False(reflex.CLecternReflexPending);
    }

    [Fact]
    public void DisplayReflexResonate_ShownDraftWithoutRows_ReadsTheStoredRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDisplay display = new TEditorFixture(TInterfaceEditor.TEditorCreate(engine)).TEditorFixtureDisplay;
        LEntry kindle = engine.TEngineEntrySave(
            TDisplayDraftCreate("kindle", "English", [TInterface.TReflexDraftCreate("Korean", string.Empty, "a")]));
        display.LDisplayRule.LDisplaySound.TDisplaySoundShow(
            kindle.LEntryId, TDisplayDraftCreate("kindle", "English", []));
        CLecternReflex shown = display.CDisplaySound.CDisplayReflexRead();

        CLecternReflex reloaded = display.CDisplaySound.CDisplayReflexResonate();

        Assert.Empty(shown.CLecternReflexRows);
        Assert.Equal("a", Assert.Single(reloaded.CLecternReflexRows).CReflexText);
    }

    internal static CWing TDisplayWingPrepare(CAtelier atelier, List<string> asked)
    {
        CWing wing = CWing.CWingCreate(atelier, TEnvoyFake.TEnvoyCreate(false, asked), true);
        return wing;
    }

    internal static LEntryDraft TDisplayDraftCreate(
        string headword, string language, IReadOnlyList<LReflexDraft> reflexes) =>
        TInterface.TEntryDraftCreate(
            headword,
            language,
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a thing", 1)],
            [],
            reflexes: reflexes);

    internal static LEntry TDisplayEnglishSave(LEngine engine) =>
        engine.TEngineEntrySave(TDisplayDraftCreate("water", "English", []));

    internal static LEntry TDisplayKoreanSave(LEngine engine) =>
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "한자",
            "Korean",
            string.Empty,
            string.Empty,
            [TInterface.TCardCreate("a character", 1)],
            [],
            transcriptions:
            [
                TInterface.TTranscriptionDraftCreate("Revised Romanization", "hanja"),
                TInterface.TTranscriptionDraftCreate("Hanja", "漢字"),
                TInterface.TTranscriptionDraftCreate("Yale", string.Empty),
            ]));
}

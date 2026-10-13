using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TAnthologyCitation
{
    [Fact]
    public void AnthologyDraftRead_TranscriptCitingASource_CarriesItsLine()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        Assert.Equal(string.Empty, anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleCitation);

        anthology.CAnthologyCitationSet(notes.LReferenceId);

        CExample? held = anthology.TAnthologyDraftRead(desk.TDeskRead());
        Assert.Equal(notes.LReferenceId, held?.CExampleSource);
        Assert.Equal("Field Notes", held?.CExampleCitation);
    }

    [Fact]
    public void AnthologyCitationSet_TypedTitle_CitesTheResolvedSource()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        anthology.CAnthologyCitationSet("Field Notes");

        Assert.Equal(notes.LReferenceId, anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleSource);
    }

    [Fact]
    public void AnthologyCitationSet_EngineFails_ShowsTheCreateFailureAndKeepsTheCitation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        List<string> asked = [];
        CEnvoy envoy = TEnvoyFake.TEnvoyCreate(false, asked);
        CDesk desk = TInterfaceCitation.TDeskFailCreate(engine, envoy, "Example", "Corpus", CSubject.CSubjectExample);
        desk.CDeskStart(null);
        CAnthology anthology = TInterfaceConductPanel.TAnthologyCreate(atelier, desk, envoy);

        anthology.CAnthologyCitationSet("Field Notes");

        Assert.Equal(["Reference.CreateFailed"], asked);
        Assert.Null(anthology.TAnthologyDraftRead(desk.TDeskRead())?.CExampleSource);
    }

    [Fact]
    public void AnthologyCitationRead_PartOfTitle_OffersTheSourceSplit()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        LReference notes = engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);

        CProffer offer = anthology.CAnthologyCitationRead(" field ");

        Assert.True(offer.CProfferShown);
        CProfferRow row = Assert.Single(offer.CProfferRows);
        Assert.Equal(notes.LReferenceId, row.CProfferRowId);
        Assert.Equal(string.Empty, row.CProfferRowLead);
        Assert.Equal("Field", row.CProfferRowMark);
        Assert.Equal(" Notes", row.CProfferRowTail);
        Assert.Equal(string.Empty, row.CProfferRowCount);
    }

    [Fact]
    public void AnthologyCitationRead_BlankWordOrNoTranscript_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);

        CProffer unheld = anthology.CAnthologyCitationRead("field");
        desk.CDeskStart(null);
        CProffer blank = anthology.CAnthologyCitationRead("   ");

        Assert.False(unheld.CProfferShown);
        Assert.Empty(unheld.CProfferRows);
        Assert.False(blank.CProfferShown);
        Assert.Empty(blank.CProfferRows);
    }

    [Fact]
    public void AnthologyCitationRead_CitedByline_OffersNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine);
        engine.TEngineCitationCreate("Field Notes");
        CAnthology anthology = TAnthology.TAnthologyPrepare(engine, atelier, out CDesk desk);
        desk.CDeskStart(null);
        anthology.CAnthologyCitationSet("Field Notes");
        string byline = anthology.TAnthologyDraftRead(desk.TDeskRead())!.CExampleCitation;

        Assert.Empty(anthology.CAnthologyCitationRead(byline).CProfferRows);
    }
}

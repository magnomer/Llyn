using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenureField
{
    [Fact]
    public void HeadwordSet_Typed_WritesHeadword()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);

        tenure.TTenureHeadwordSet("ember");

        Assert.Equal("ember", tenure.TTenureRead()!.LDraftContent.LEntryDraftHeadword);
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
    }

    [Fact]
    public void NoteSet_TrailingBreaks_WritesTrimmedNote()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);

        tenure.TTenureNoteSet("warm\r\n\n");

        Assert.Equal("warm", tenure.TTenureRead()!.LDraftContent.LEntryDraftNote);
    }

    [Fact]
    public void LanguageSet_Empty_KeepsLanguage()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);
        tenure.TTenureLanguageSet("English");

        tenure.TTenureLanguageSet(string.Empty);

        Assert.Equal("English", tenure.TTenureRead()!.LDraftContent.LEntryDraftLanguage);
    }

    [Fact]
    public void SpeechSet_Typed_WritesSpeeches()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);
        tenure.TTenureLanguageSet("English");

        Assert.True(tenure.TTenureSpeechSet(" noun ").LSpeechOfferShown);
        tenure.TTenurePersist();

        Assert.Equal(
            "noun", Assert.Single(tenure.TTenureRead()!.LDraftContent.LEntryDraftSpeeches).LSpeechDraftName);
    }

    [Fact]
    public void IpaSet_Typed_WritesPrimaryIpa()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);

        tenure.TTenureIpaSet("ˈɛmbə");

        Assert.Equal(
            "ˈɛmbə", tenure.TTenureRead()!.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftIpa);
    }

    [Fact]
    public void RespellingSet_Typed_WritesPrimaryRespelling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LTenure tenure = TTenureFieldStart(engine);

        tenure.TTenureRespellingSet("EM-buh");

        Assert.Equal(
            "EM-buh",
            tenure.TTenureRead()!.LDraftContent.LEntryDraftPronunciations[0].LPronunciationDraftRespelling);
    }

    private static LTenure TTenureFieldStart(LEngine engine)
    {
        engine.TEngineDelaySet(0);
        return engine.TEngineTenureStart("test", LSubject.LSubjectEntry, null);
    }
}

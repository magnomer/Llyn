using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TTenureLanguage
{
    [Fact]
    public void Language_ReadsHeldDraftAndAnswersEmptyAfterCancel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate("water", "English", "", "", [], []));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);
        Assert.Equal(engine.TEngineFlaggedCheck("English"), tenure.TTenureFlaggedCheck());
        Assert.Equal(engine.TEngineVarietyRead("English"), tenure.TTenureVarietyRead());
        Assert.Equal(tenure.TTenureFlaggedCheck(), engine.TEngineFlaggedCheck(tenure.TTenureRead()!.LDraftContent));

        tenure.TTenureCancel();
        Assert.False(tenure.TTenureFlaggedCheck());
        Assert.Empty(tenure.TTenureVarietyRead());
        Assert.False(engine.TEngineFlaggedCheck(TInterface.TEntryDraftCreate("", "", "", "", [], [])));
    }

    [Fact]
    public void TenureSchemeRead_TwoRows_MarksTheSchemeTheOtherRowHolds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LEntry entry = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "香港",
            "Cantonese",
            "",
            "",
            [],
            [],
            transcriptions:
            [
                TInterface.TTranscriptionDraftCreate("Jyutping", "hoeng1 gong2"),
                TInterface.TTranscriptionDraftCreate("Yale", "hēung góng"),
            ]));
        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectEntry, entry.LEntryId);
        long jyutping = tenure.TTenureRead()!.LDraftContent.LEntryDraftTranscriptions[0].LTranscriptionDraftId;

        IReadOnlyList<LSchemeRow> rows = tenure.TTenureSchemeRead(jyutping);

        Assert.Equal(["Jyutping", "Yale"], rows.Select(static row => row.LSchemeRowName));
        Assert.Equal([false, true], rows.Select(static row => row.LSchemeRowTaken));
        tenure.TTenureCancel();
        Assert.Empty(tenure.TTenureSchemeRead(jyutping));
    }
}

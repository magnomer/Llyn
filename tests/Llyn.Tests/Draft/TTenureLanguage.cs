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
}

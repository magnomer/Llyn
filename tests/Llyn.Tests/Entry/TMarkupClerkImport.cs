using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupClerkImport
{
    [Fact]
    public void MarkupClerkImport_PairTree_ReExportsAnEqualTree()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LMarkupClerk markup = TInterface.TMarkupClerkCreate(rig);
        LMarkupClerkIntake intake = TInterface.TMarkupIntakeCreate(rig);
        LMarkupCargo cargo = markup.TMarkupClerkRead(TInterface.TMarkupSave(workspace, TInterface.TMarkupPair));

        LMarkupOutcome outcome = intake.TMarkupClerkImport(cargo, TMarkupIntakeCreate(cargo.LMarkupCargoEntry.Count));

        Assert.Empty(outcome.LMarkupOutcomeOmission);
        List<LMarkupEntry> exported = [];
        foreach (long id in workspace.TWorkspaceColumnRead("SELECT entry_id FROM entry ORDER BY entry_id;"))
        {
            exported.Add(Assert.IsType<LMarkupEntry>(markup.TMarkupClerkLoad(id)));
        }

        Assert.Equal(TInterface.TMarkupFormat(cargo.LMarkupCargoEntry), TInterface.TMarkupFormat(exported));
    }

    private static IReadOnlyList<LMarkupIntake> TMarkupIntakeCreate(int count)
    {
        List<LMarkupIntake> intakes = new(count);
        for (int index = 0; index < count; index++)
        {
            intakes.Add(TInterface.TMarkupIntakeCreate(index, LMarkupMode.LMarkupModeNew));
        }

        return intakes;
    }
}

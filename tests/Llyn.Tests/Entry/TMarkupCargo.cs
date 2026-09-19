using System;
using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupCargo
{
    private const string TMarkupTwo = """
        <llyn>
          <entry>
            <headword>ember</headword>
            <language>English</language>
          </entry>
          <entry>
            <headword>glow</headword>
            <language>English</language>
          </entry>
        </llyn>
        """;

    [Fact]
    public void MarkupRead_OversizeFile_RefusesMarkup()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = Path.Combine(workspace.TWorkspaceFolder, "huge.llx");
        using (FileStream stream = File.Create(path))
        {
            stream.SetLength(TInterface.TMarkupCeilingRead() + 1);
        }

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineMarkupRead(path));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.LRefusalReason);
    }

    [Fact]
    public void MarkupRead_TraversalLanguage_BlanksAndReportsOmission()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = workspace.TWorkspaceMarkupSave(TMarkupTwo.Replace(
            "<language>English</language>", "<language>..\\..\\x</language>", StringComparison.Ordinal));
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);

        Assert.Equal(2, cargo.LMarkupCargoOmission.Count);
        Assert.Contains("..", cargo.LMarkupCargoOmission[0].LMarkupOmissionText);
        Assert.Equal(2, cargo.LMarkupCargoOmission[0].LMarkupOmissionLine);
        Assert.All(cargo.LMarkupCargoEntry, entry => Assert.Equal(string.Empty, entry.LMarkupEntryLanguage));
    }

    [Fact]
    public void MarkupImport_DuplicateTarget_RefusesItem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry glow = engine.TEngineEntrySave(
            TInterface.TEntryDraftCreate("glow", "English", string.Empty, string.Empty, [], []));

        string path = workspace.TWorkspaceMarkupSave(TMarkupTwo);
        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineMarkupImport(
            engine.TEngineMarkupRead(path),
            [
                TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeReplace, glow.LEntryId),
                TInterface.TMarkupIntakeCreate(1, LMarkupMode.LMarkupModeMerge, glow.LEntryId),
            ]));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
        Assert.Equal("glow", Assert.Single(engine.TEngineEntryFind(string.Empty)).LEntryHeadword);
    }

    [Fact]
    public void MarkupImport_CargoOmissions_CarriesThemIntoOutcome()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        string path = workspace.TWorkspaceMarkupSave(TMarkupTwo.Replace(
            "<headword>glow", "<etymology>x</etymology><headword>glow", StringComparison.Ordinal));
        LMarkupCargo cargo = engine.TEngineMarkupRead(path);
        LMarkupOutcome outcome = engine.TEngineMarkupImport(
            cargo,
            [
                TInterface.TMarkupIntakeCreate(0, LMarkupMode.LMarkupModeNew),
                TInterface.TMarkupIntakeCreate(1, LMarkupMode.LMarkupModeNew),
            ]);

        Assert.Equal("<etymology>", Assert.Single(cargo.LMarkupCargoOmission).LMarkupOmissionText);
        Assert.Equal(cargo.LMarkupCargoOmission, outcome.LMarkupOutcomeOmission);
    }
}

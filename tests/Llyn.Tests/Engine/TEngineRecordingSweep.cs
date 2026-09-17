using System.IO;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineRecordingSweep
{
    [Fact]
    public void RecordingSweep_UnnamedFile_Deletes()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string orphan = TRecordingFileSave(workspace, "orphan.mp3");

        engine.TEngineRecordingSweep();

        Assert.False(File.Exists(orphan));
    }

    [Fact]
    public void RecordingSweep_StoredFile_Stays()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string stored = TRecordingFileSave(workspace, "kindle.mp3");
        engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "kindle", "English", string.Empty, string.Empty, [], [], stored, "Wiktionary"));

        engine.TEngineRecordingSweep();

        Assert.True(File.Exists(stored));
    }

    [Fact]
    public void RecordingSweep_DraftFile_Stays()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        string fetched = TRecordingFileSave(workspace, "ember.mp3");
        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineRequestApply(TInterface.TRequestAudioCreate(started.LDraftId, fetched, "Forvo"));

        engine.TEngineRecordingSweep();

        Assert.True(File.Exists(fetched));
    }

    private static string TRecordingFileSave(TWorkspace workspace, string name)
    {
        string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");
        Directory.CreateDirectory(folder);
        string file = Path.Combine(folder, name);
        File.WriteAllBytes(file, [0]);
        return file;
    }
}

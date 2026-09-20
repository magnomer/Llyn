using System.IO;
using System.Net;
using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TRecordingClerk
{
    private const string TRecordingClerkBody = "flat=https://example.test/flat.mp3";

    private const string TRecordingClerkPack =
        """
        { "audio": [
            { "name": "Flat", "attempts": [ { "urls": ["https://example.test/{word}"],
                "strategy": "regex", "match": "flat=(\\S+)", "group": 1 } ] } ] }
        """;

    [Fact]
    public async Task RecordingClerkFind_PackWithSource_EmitsStepsAndAnswersRecordings()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(TRecordingClerkPack);
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LRig rig = workspace.TWorkspaceRigCreate(
            TPronunciationHelper.TSourceClientCreate(TRecordingClerkBody, HttpStatusCode.OK));
        LRecordingClerk clerk = TInterface.TRecordingClerkCreate(rig);
        TListenerStub listener = TPronunciationHelper.TListenerCreate();

        IReadOnlyList<LRecording> found = await clerk.TRecordingClerkFind(
            "tomato", pack.TLanguageFixtureName, string.Empty, listener, CancellationToken.None);

        LRecording recording = Assert.Single(found);
        Assert.Equal("https://example.test/flat.mp3", recording.LRecordingAddress);
        Assert.Equal(recording.LRecordingAddress, Assert.Single(listener.TListenerStubRecordings).LRecordingAddress);
        Assert.Equal(1, listener.TListenerStubFinished);
    }

    [Fact]
    public void RecordingClerkSweep_UnnamedFile_DeletesItAndKeepsTheStoredOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LRecordingClerk clerk = TInterface.TRecordingClerkCreate(rig);
        string orphan = TRecordingFileSave(workspace, "orphan.mp3");
        string stored = TRecordingFileSave(workspace, "kindle.mp3");
        LEntry kindle = TInterface.TEntryClerkCreate(rig).TEntryClerkAdd(
            TInterface.TEntryCreate(0, "kindle", "English", 0, null, null));
        LPronunciation spoken = TInterface.TPronunciationSave(
            rig, TInterface.TPronunciationCreate(0, kindle.LEntryId, "ˈkɪndəl", []));
        TInterface.TAudioSave(rig, spoken.LPronunciationId, TInterface.TRecordingFormat(rig, stored), "Wiktionary");

        clerk.TRecordingClerkSweep();

        Assert.False(File.Exists(orphan));
        Assert.True(File.Exists(stored));
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
